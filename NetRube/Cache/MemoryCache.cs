using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetRube.Cache
{
	/// <summary>NetRube 内存缓存操作类</summary>
	public class MemoryCache : ICache
	{
		private static volatile MemoryCache INSTANCE = null;
		private static ConcurrentDictionary<string, CacheItem> CACHE;

		private MemoryCache()
		{
			if(CACHE == null)
				CACHE = new ConcurrentDictionary<string, CacheItem>();
		}

		/// <summary>获取缓存操作对象</summary>
		/// <value>缓存操作对象实例</value>
		public static MemoryCache Cache
		{
			get
			{
				if(INSTANCE == null)
					INSTANCE = new MemoryCache();
				return INSTANCE;
			}
		}

		#region 内部操作
		private int __Dels(IEnumerable<string> keys)
		{
			var count = 0;
			if(keys.IsNullOrEmpty_()) return count;

			foreach(var key in keys)
			{
				CacheItem cache;
				if(CACHE.TryRemove(key, out cache))
					count++;
			}

			return count;
		}

		private void DelExp()
		{
			this.__Dels(this.GetExpKeys());
		}

		private IEnumerable<string> GetExpKeys()
		{
			var now = DateTime.Now;
			foreach(var item in CACHE)
			{
				if(item.Value.ExpType == ExpType.FileDependency)
				{
					if(item.Value.ExpTime == File.GetLastWriteTime(item.Value.DepPath))
						continue;
				}
				else if(item.Value.ExpTime > now)
					continue;

				yield return item.Key;
			}
		}

		private IEnumerable<string> GetPrefixKeys(string prefix)
		{
			foreach(var item in CACHE)
				if(item.Key.StartsWith(prefix))
					yield return item.Key;
		}

		private DateTime GetExpTime(int expire)
		{
			var now = DateTime.Now;
			return expire > 0 ? now.AddMinutes(expire) : now.AddYears(1);
		}
		#endregion

		#region ICache 成员

		#region 获取
		/// <summary>获取</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public T Get<T>(string key)
		{
			if(key.IsNullOrEmpty_())
				return default(T);

			this.DelExp();

			CacheItem cache = null;
			if(!CACHE.TryGetValue(key, out cache))
				return default(T);

			if(cache.ExpType == ExpType.Relative)
			{
				cache.ExpTime = this.GetExpTime(cache.ExpSpan);

				CACHE[key] = cache;
			}
			return (T)cache.Data;
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
			if(key.IsNullOrEmpty_()) return;

			if(value == null)
			{
				this.Delete(key);
				return;
			}

			var cache = new CacheItem
			{
				Key = key,
				Data = value,
				DataType = typeof(T).FastGetName(),
				ExpTime = this.GetExpTime(expire),
				ExpType = absExpire ? ExpType.Absolute : ExpType.Relative,
				ExpSpan = expire
			};
			CACHE[key] = cache;
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
			if(key.IsNullOrEmpty_()) return;

			if(value == null || !File.Exists(filePath))
			{
				this.Delete(key);
				return;
			}

			var fileTime = File.GetLastWriteTime(filePath);
			if(fileTime <= DateTime.Now)
			{
				this.Delete(key);
				return;
			}

			var cache = new CacheItem
			{
				Key = key,
				Data = value,
				DataType = typeof(T).FastGetName(),
				ExpTime = fileTime,
				ExpType = ExpType.FileDependency,
				DepPath = filePath
			};
			CACHE[key] = cache;
		}
		#endregion

		#region 删除
		/// <summary>删除缓存对象</summary>
		/// <param name="key">要删除的缓存键</param>
		public void Delete(string key)
		{
			if(key.IsNullOrEmpty_()) return;
			CacheItem cache;
			CACHE.TryRemove(key, out cache);
		}

		/// <summary>删除所有缓存对象</summary>
		public void DeleteAll()
		{
			CACHE = new ConcurrentDictionary<string, CacheItem>();
		}

		/// <summary>删除以某字符串为前缀的缓存，并返回删除的缓存项数</summary>
		/// <param name="prefix">前缀</param>
		/// <returns>删除的缓存项数</returns>
		public int DeleteWithPrefix(string prefix)
		{
			if(prefix.IsNullOrEmpty_()) return 0;

			return __Dels(this.GetPrefixKeys(prefix));
		}
		#endregion

		/// <summary>返回缓存总项数</summary>
		/// <value>缓存总项数</value>
		public int Count
		{
			get { return CACHE.Count; }
		}

		/// <summary>返回所有缓存键</summary>
		/// <value>所有缓存键</value>
		public string[] AllKeys
		{
			get { return CACHE.Keys.ToArray(); }
		}

		/// <summary>返回所有缓存键和数据类型</summary>
		/// <value>所有缓存键和数据类型</value>
		public Dictionary<string, string> AllKeysWithDataType
		{
			get
			{
				var ls = new Dictionary<string, string>(CACHE.Count);
				foreach(var item in CACHE)
					ls.Add(item.Value.Key, item.Value.DataType);
				return ls;
			}
		}

		#endregion

		#region 内部类
		/// <summary>缓存项</summary>
		[Serializable]
		private class CacheItem
		{
			/// <summary>缓存键</summary>
			public string Key { get; set; }
			/// <summary>缓存数据</summary>
			public object Data { get; set; }
			/// <summary>缓存数据类型</summary>
			public string DataType { get; set; }
			/// <summary>缓存过期时间</summary>
			public DateTime ExpTime { get; set; }
			/// <summary>过期类型</summary>
			public ExpType ExpType { get; set; }
			/// <summary>缓存过期间隔。分钟</summary>
			public int ExpSpan { get; set; }
			/// <summary>依赖路径</summary>
			public string DepPath { get; set; }
		}

		/// <summary>过期类型</summary>
		private enum ExpType : byte
		{
			/// <summary>绝对时间</summary>
			Absolute,
			/// <summary>相对时间</summary>
			Relative,
			/// <summary>文件依赖项</summary>
			FileDependency
		}
		#endregion
	}
}