using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using WC = System.Web.Caching.Cache;

namespace NetRube.Cache
{
	/// <summary>NetRube Asp.net 缓存操作类</summary>
	public class WebCache : ICache
	{
		private static volatile WebCache __instance = null;
		private static volatile WC __cache;

		private WebCache()
		{
			__cache = HttpRuntime.Cache;
		}

		/// <summary>获取缓存操作对象</summary>
		/// <value>缓存操作对象实例</value>
		public static WebCache Cache
		{
			get
			{
				if(null == __instance)
					__instance = new WebCache();
				return __instance;
			}
		}

		#region ICache 成员

		#region 获取
		/// <summary>获取缓存对象</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要获取的缓存键</param>
		/// <returns>缓存的数据</returns>
		public T Get<T>(string key)
		{
			if(string.IsNullOrEmpty(key))
				return default(T);

			var _retval = __cache.Get(key);
			if(null == _retval)
				return default(T);
			return (T)_retval;
		}
		#endregion

		#region 添加
		/// <summary>添加缓存对象，并指定缓存的过期类型和时间</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要添加的缓存键</param>
		/// <param name="value">要添加的缓存值</param>
		/// <param name="absExpire">是否绝对过期</param>
		/// <param name="expire">缓存过期时间，分钟数</param>
		public void Add<T>(string key, T value, bool absExpire = false, int expire = 0)
		{
			if(string.IsNullOrEmpty(key)) return;

			__cache.Remove(key);
			if(null == value) return;

			if(absExpire)
			{
				DateTime _expTime = expire == 0 ? WC.NoAbsoluteExpiration : DateTime.Now.AddMinutes((double)expire);
				__cache.Insert(key, value, null, _expTime, WC.NoSlidingExpiration, CacheItemPriority.Normal, null);
			}
			else
			{
				TimeSpan _timeSpan = (expire == 0 ? WC.NoSlidingExpiration : new TimeSpan(0, expire, 0));
				__cache.Insert(key, value, null, WC.NoAbsoluteExpiration, _timeSpan, CacheItemPriority.Normal, null);
			}
		}

		/// <summary>添加缓存对象，并监视指定文件，当指定文件更改时缓存将自动过期，同时可以指定缓存的过期类型和时间</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要添加的缓存键</param>
		/// <param name="value">要添加的缓存值</param>
		/// <param name="filePath">要监视的文件路径。当该文件更改时，缓存将自动过期</param>
		/// <param name="absExpire">是否绝对过期</param>
		/// <param name="expire">缓存过期时间，分钟数</param>
		public void Add<T>(string key, T value, string filePath, bool absExpire = false, int expire = 0)
		{
			if(string.IsNullOrEmpty(key)) return;

			__cache.Remove(key);
			if(null == value) return;
			if(!Utils.FileExists(filePath)) return;

			using(CacheDependency _dependency = new CacheDependency(filePath, DateTime.Now))
			{
				if(absExpire)
				{
					DateTime _expTime = expire == 0 ? WC.NoAbsoluteExpiration : DateTime.Now.AddMinutes((double)expire);
					__cache.Insert(key, value, _dependency, _expTime, WC.NoSlidingExpiration, CacheItemPriority.Normal, null);
				}
				else
				{
					TimeSpan _timeSpan = (expire == 0 ? WC.NoSlidingExpiration : new TimeSpan(0, expire, 0));
					__cache.Insert(key, value, _dependency, WC.NoAbsoluteExpiration, _timeSpan, CacheItemPriority.Normal, null);
				}
			}
		}
		#endregion

		#region 删除
		/// <summary>删除缓存对象</summary>
		/// <param name="key">要删除的缓存键</param>
		public void Delete(string key)
		{
			if(string.IsNullOrEmpty(key)) return;
			__cache.Remove(key);
		}

		/// <summary>删除所有缓存对象</summary>
		public void DeleteAll()
		{
			IDictionaryEnumerator _enum = __cache.GetEnumerator();
			while(_enum.MoveNext())
				__cache.Remove(_enum.Key.ToString());
		}

		/// <summary>删除以某字符串为前缀的缓存，并返回删除的缓存项数</summary>
		/// <param name="prefix">前缀</param>
		/// <returns>删除的缓存项数</returns>
		public int DeleteWithPrefix(string prefix)
		{
			if(string.IsNullOrEmpty(prefix)) return 0;

			int _count = 0;
			string _key;
			IDictionaryEnumerator _enum = __cache.GetEnumerator();
			while(_enum.MoveNext())
			{
				_key = _enum.Key.ToString();
				if(_key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					__cache.Remove(_key);
					_count++;
				}
			}
			return _count;
		}
		#endregion

		/// <summary>返回缓存总项数</summary>
		/// <value>缓存总项数</value>
		public int Count
		{
			get { return __cache.Count; }
		}

		/// <summary>返回所有缓存键</summary>
		/// <value>所有缓存键</value>
		public string[] AllKeys
		{
			get
			{
				int _count = __cache.Count;
				string[] _keys = new string[_count];
				int _index = 0;
				IDictionaryEnumerator _enum = __cache.GetEnumerator();
				while(_enum.MoveNext())
				{
					_keys[_index] = _enum.Key.ToString();
					_index++;
				}
				return _keys;
			}
		}

		/// <summary>返回所有缓存键和数据类型</summary>
		/// <value>所有缓存键和数据类型</value>
		public Dictionary<string, string> AllKeysWithDataType
		{
			get
			{
				int _count = __cache.Count;
				string _key, _dataType;
				Dictionary<string, string> _dict = new Dictionary<string, string>(_count);
				IDictionaryEnumerator _enum = __cache.GetEnumerator();
				while(_enum.MoveNext())
				{
					_key = _enum.Key.ToString();
					_dataType = _enum.Value.GetType().FastGetName();
					_dict.Add(_key, _dataType);
				}
				return _dict;
			}
		}

		#endregion
	}
}