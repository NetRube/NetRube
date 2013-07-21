using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetRube
{
	/// <summary>二进制序列化</summary>
	public class BinarySerialization
	{
		/// <summary>从二进制文件加载反序列化成对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="fileName">文件名</param>
		/// <returns>反序列化后的对象</returns>
		public static T Load<T>(string fileName)
		{
			if(!Utils.FileExists(fileName)) return default(T);

			using(var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var fmt = new BinaryFormatter();
				return (T)fmt.Deserialize(fs);
			}
		}

		/// <summary>从二进制数据反序列化成对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="data">要反序列化的数据</param>
		/// <returns>反序列化后的对象</returns>
		public static T Deserialize<T>(byte[] data)
		{
			if(data == null) return default(T);

			using(var ms = new MemoryStream(data))
			{
				var fmt = new BinaryFormatter();
				return (T)fmt.Deserialize(ms);
			}
		}

		/// <summary>保存要进行二进制序列化的对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="obj">要序列化的对象</param>
		/// <param name="fileName">文件名</param>
		/// <returns>指示是否保存成功</returns>
		public static bool Save<T>(T obj, string fileName)
		{
			if(obj == null || fileName.IsNullOrEmpty_()) return false;

			try
			{
				using(var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
				{
					var fmt = new BinaryFormatter();
					fmt.Serialize(fs, obj);
				}
				return true;
			}
			catch { return false; }
		}

		/// <summary>
		/// 将对象序列化为二进制数据
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="obj">要序列化的对象</param>
		/// <returns>序列化后的二进制数据</returns>
		public static byte[] Serialize<T>(T obj)
		{
			if(obj == null) return null;

			using(var ms = new MemoryStream())
			{
				var fmt = new BinaryFormatter();
				fmt.Serialize(ms, obj);
				ms.Position = 0;
				var data = new byte[ms.Length];
				ms.Read(data, 0, data.Length);
				return data;
			}
		}
	}
}