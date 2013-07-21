using System.Collections.Generic;

namespace NetRube.Cache
{
	/// <summary>NetRube 缓存操作接口</summary>
	public interface ICache
	{
		#region 获取
		/// <summary>获取缓存对象</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要获取的缓存键</param>
		/// <returns>缓存的数据</returns>
		T Get<T>(string key);
		#endregion

		#region 添加
		/// <summary>添加缓存对象，并指定缓存的过期类型和时间</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要添加的缓存键</param>
		/// <param name="value">要添加的缓存值</param>
		/// <param name="absExpire">是否绝对过期</param>
		/// <param name="expire">缓存过期时间，分钟数</param>
		void Add<T>(string key, T value, bool absExpire = false, int expire = 0);

		/// <summary>添加缓存对象，并监视指定文件，当指定文件更改时缓存将自动过期，同时可以指定缓存的过期类型和时间</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">要添加的缓存键</param>
		/// <param name="value">要添加的缓存值</param>
		/// <param name="filePath">要监视的文件路径。当该文件更改时，缓存将自动过期</param>
		/// <param name="absExpire">是否绝对过期</param>
		/// <param name="expire">缓存过期时间，分钟数</param>
		void Add<T>(string key, T value, string filePath, bool absExpire = false, int expire = 0);
		#endregion

		#region 删除
		/// <summary>删除缓存对象</summary>
		/// <param name="key">要删除的缓存键</param>
		void Delete(string key);

		/// <summary>删除所有缓存对象</summary>
		void DeleteAll();

		/// <summary>删除以某字符串为前缀的缓存，并返回删除的缓存项数</summary>
		/// <param name="prefix">前缀</param>
		/// <returns>删除的缓存项数</returns>
		int DeleteWithPrefix(string prefix);
		#endregion

		/// <summary>返回缓存总项数</summary>
		/// <value>缓存总项数</value>
		int Count { get; }

		/// <summary>返回所有缓存键</summary>
		/// <value>所有缓存键</value>
		string[] AllKeys { get; }

		/// <summary>返回所有缓存键和数据类型</summary>
		/// <value>所有缓存键和数据类型</value>
		Dictionary<string, string> AllKeysWithDataType { get; }
	}
}