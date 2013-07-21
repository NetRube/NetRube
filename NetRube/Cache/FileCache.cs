using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetRube.Cache
{
	/// <summary>
	/// NetRube 文件缓存操作类
	/// </summary>
	public class FileCache : ICache
	{
		private static volatile FileCache __instance = null;
		private static Dictionary<string, CacheItem> __cacheItems;
		private static string __cachePath;
		private static string __cacheItemPath;
		private static object __lockHelper = new object();

		private FileCache()
		{
			if(Utils.IsInAspDotNet())
				__cachePath = Utils.GetMapPath(@"App_Data\Cache\");
			else
				__cachePath = Utils.GetMapPath(@"Cache\");

			Utils.AddDirectory(__cachePath);

			__cacheItemPath = Path.Combine(__cachePath, "CacheItems");

			if(Utils.FileExists(__cacheItemPath))
			{
				__cacheItems = this.__Get<Dictionary<string, CacheItem>>(__cacheItemPath);
			}
			else
			{
				this.__CreateCacheItemsFile();
			}

		}

		/// <summary>
		/// 获取缓存操作对象
		/// </summary>
		/// <value>缓存操作对象实例</value>
		public static FileCache Cache
		{
			get
			{
				if(null == __instance)
					__instance = new FileCache();
				return __instance;
			}
		}

		#region 内部操作

		private void __CreateCacheItemsFile()
		{
			lock(__lockHelper)
			{
				if(Directory.Exists(__cachePath))
				{
					Directory.Delete(__cachePath, true);
					Directory.CreateDirectory(__cachePath);
				}

				__cacheItems = new Dictionary<string, CacheItem>();

				__SaveCacheItemsFile();
			}
		}

		private string __GetPath(string key)
		{
			return Path.Combine(__cachePath, key);
		}

		/// <summary>
		/// 内部获取缓存内容
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">要获取的缓存文件路径</param>
		/// <returns></returns>
		private T __Get<T>(string path)
		{
			using(FileStream _fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				BinaryFormatter _bf = new BinaryFormatter();
				return (T)_bf.Deserialize(_fs);
			}
		}

		/// <summary>
		/// 内部设置缓存内容，并返回是否缓存成功
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">要保存的缓存文件路径</param>
		/// <param name="value">要保存的缓存值</param>
		/// <returns></returns>
		private void __Add<T>(string path, T value)
		{
			using(FileStream _fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
			{
				BinaryFormatter _bf = new BinaryFormatter();
				_bf.Serialize(_fs, value);
			}
		}

		/// <summary>
		/// 内部删除缓存文件
		/// </summary>
		/// <param name="path">要删除的缓存文件路径</param>
		private void __Delete(string path)
		{
			if(Utils.FileExists(path))
				File.Delete(path);
		}

		/// <summary>
		/// 内部批量删除缓存文件
		/// </summary>
		/// <param name="keys">要删除的缓存文件键集合</param>
		/// <returns></returns>
		private int __Deletes(IEnumerable<string> keys)
		{
			int _count = 0;
			if(keys.IsNullOrEmpty_()) return _count;

			ArrayBuffer<string> _array = new ArrayBuffer<string>(keys);
			string[] _keys = _array.ToArray();

			lock(__lockHelper)
			{
				foreach(string _key in _keys)
				{
					this.__Delete(this.__GetPath(_key));
					__cacheItems.Remove(_key);
					_count++;
				}
				this.__SaveCacheItemsFile();
			}
			return _count;
		}

		/// <summary>
		/// 内部删除过期缓存
		/// </summary>
		private void __DeleteExp()
		{
			this.__Deletes(this.__GetExpCache());
		}
		/// <summary>
		/// 内存获取过期缓存
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> __GetExpCache()
		{
			DateTime _now = DateTime.Now;
			foreach(KeyValuePair<string, CacheItem> _item in __cacheItems)
			{
				if(_item.Value.ExpTime > _now) continue;

				if(_item.Value.ExpType == ExpType.FileDependency)
				{
					if(Utils.FileExists(_item.Value.DepPath))
					{
						if(File.GetLastWriteTime(_item.Value.DepPath) <= _item.Value.ExpTime)
							continue;
					}
				}
				yield return _item.Key;
			}
		}

		/// <summary>
		/// 保存缓存项记录文件
		/// </summary>
		private void __SaveCacheItemsFile()
		{
			this.__Add<Dictionary<string, CacheItem>>(__cacheItemPath, __cacheItems);
		}

		#endregion

		#region ICache 成员

		#region 获取
		/// <summary>
		/// 获取缓存对象
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要获取的缓存键</param>
		/// <returns>缓存的数据</returns>
		public T Get<T>(string key)
		{
			if(string.IsNullOrEmpty(key))
				return default(T);

			this.__DeleteExp();

			CacheItem _item;
			if(!__cacheItems.TryGetValue(key, out _item))
				return default(T);
			string _path = this.__GetPath(key);
			if(Utils.FileExists(_path))
			{
				if(_item.ExpType == ExpType.Relative)
				{
					DateTime _expTime = DateTime.Now;
					if(_item.ExpSpan == 0)
						_expTime.AddYears(1);
					else
						_expTime.AddMinutes(_item.ExpSpan);

					this.__SaveCacheItemsFile();
				}

				return this.__Get<T>(_path);
			}
			else
			{
				__cacheItems.Remove(key);
				this.__SaveCacheItemsFile();
			}

			return default(T);
		}
		#endregion

		#region 添加
		/// <summary>
		/// 添加缓存对象，并指定缓存的过期类型和时间
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要添加的缓存键</param>
		/// <param name="value">要添加的缓存值</param>
		/// <param name="absExpire">是否绝对过期</param>
		/// <param name="expire">缓存过期时间，分钟数</param>
		public void Add<T>(string key, T value, bool absExpire = false, int expire = 0)
		{
			if(string.IsNullOrEmpty(key)) return;

			this.Delete(key);
			if(null == value) return;

			CacheItem _item = new CacheItem
			{
				Key = key,
				DataType = value.GetType().ToString(),
				ExpTime = expire == 0 ? DateTime.Now.AddYears(1) : DateTime.Now.AddMinutes(expire),
				ExpType = absExpire ? ExpType.Absolute : ExpType.Relative,
				ExpSpan = expire
			};
			lock(__lockHelper)
			{
				__cacheItems.Add(key, _item);

				string _path = this.__GetPath(key);
				this.__Add<T>(_path, value);
				this.__SaveCacheItemsFile();
			}
		}

		/// <summary>
		/// 添加缓存对象，并监视指定文件，当指定文件更改时缓存将自动过期
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要添加的缓存键</param>
		/// <param name="value">要添加的缓存值</param>
		/// <param name="filePath">要监视的文件路径。当该文件更改时，缓存将自动过期</param>
		public void Add<T>(string key, T value, string filePath)
		{
			if(string.IsNullOrEmpty(key)) return;

			this.Delete(key);
			if(null == value) return;

			if(!Utils.FileExists(filePath)) return;

			CacheItem _item = new CacheItem
			{
				Key = key,
				DataType = value.GetType().ToString(),
				ExpTime = File.GetLastWriteTime(filePath),
				ExpType = ExpType.FileDependency,
				DepPath = filePath
			};

			lock(__lockHelper)
			{
				__cacheItems.Add(key, _item);

				string _path = this.__GetPath(key);
				this.__Add<T>(_path, value);
				this.__SaveCacheItemsFile();
			}
		}
		#endregion

		#region 删除
		/// <summary>
		/// 删除缓存对象
		/// </summary>
		/// <param name="key">要删除的缓存键</param>
		public void Delete(string key)
		{
			if(string.IsNullOrEmpty(key)) return;
			if(!__cacheItems.ContainsKey(key)) return;

			lock(__lockHelper)
			{
				string _path = this.__GetPath(key);
				this.__Delete(_path);

				__cacheItems.Remove(key);
				this.__SaveCacheItemsFile();
			}
		}

		/// <summary>
		/// 删除所有缓存对象
		/// </summary>
		public void DeleteAll()
		{
			this.__CreateCacheItemsFile();
		}

		/// <summary>
		/// 删除以某字符串为前缀的缓存，并返回删除的缓存项数
		/// </summary>
		/// <param name="prefix">前缀</param>
		/// <returns>删除的缓存项数</returns>
		public int DeleteWithPrefix(string prefix)
		{
			if(string.IsNullOrEmpty(prefix)) return 0;

			return this.__Deletes(this.__GetPrefixCache(prefix));
		}
		/// <summary>
		/// 内存获取过期缓存
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> __GetPrefixCache(string prefix)
		{
			foreach(KeyValuePair<string, CacheItem> _item in __cacheItems)
			{
				if(_item.Key.StartsWith(prefix))
					yield return _item.Key;
			}
		}
		#endregion

		/// <summary>
		/// 返回缓存总项数
		/// </summary>
		/// <value>
		/// 缓存总项数
		/// </value>
		public int Count
		{
			get { return __cacheItems.Count; }
		}

		/// <summary>
		/// 返回所有缓存键
		/// </summary>
		/// <value>
		/// 所有缓存键
		/// </value>
		public string[] AllKeys
		{
			get
			{
				return __cacheItems.Keys.ToStrArray_();
			}
		}

		/// <summary>
		/// 返回所有缓存键和数据类型
		/// </summary>
		/// <value>
		/// 所有缓存键和数据类型
		/// </value>
		public Dictionary<string, string> AllKeysWithDataType
		{
			get
			{
				int _count = __cacheItems.Count;
				Dictionary<string, string> _dict = new Dictionary<string, string>(_count);
				foreach(KeyValuePair<string, CacheItem> _item in __cacheItems)
					_dict.Add(_item.Value.Key, _item.Value.DataType);
				return _dict;
			}
		}

		#endregion

		#region 内部类
		/// <summary>
		/// 缓存项
		/// </summary>
		[Serializable]
		private class CacheItem
		{
			/// <summary>
			/// 缓存键
			/// </summary>
			public string Key { get; set; }
			/// <summary>
			/// 缓存数据类型
			/// </summary>
			public string DataType { get; set; }
			/// <summary>
			/// 缓存过期时间
			/// </summary>
			public DateTime ExpTime { get; set; }
			/// <summary>
			/// 过期类型
			/// </summary>
			public ExpType ExpType { get; set; }
			/// <summary>
			/// 缓存过期间隔。分钟
			/// </summary>
			public int ExpSpan { get; set; }
			/// <summary>
			/// 依赖路径
			/// </summary>
			public string DepPath { get; set; }
		}
		/// <summary>
		/// 过期类型
		/// </summary>
		private enum ExpType : byte
		{
			/// <summary>
			/// 绝对时间
			/// </summary>
			Absolute,
			/// <summary>
			/// 相对时间
			/// </summary>
			Relative,
			/// <summary>
			/// 文件依赖项
			/// </summary>
			FileDependency
		}
		#endregion
	}
}