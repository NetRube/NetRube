using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace NetRube
{
	/// <summary>XML 文件序列化</summary>
	public class XmlSerialization
	{
		/// <summary>从 XML 文件加载反序列化成对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="fileName">文件名</param>
		/// <returns>反序列化后的对象</returns>
		public static T Load<T>(string fileName)
		{
			if(!Utils.FileExists(fileName)) return default(T);

			using(FileStream _file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				XmlSerializer _xml = new XmlSerializer(typeof(T));
				return (T)_xml.Deserialize(_file);
			}
		}

		/// <summary>从 XML 文本反序列化成对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="xmlData">XML 数据</param>
		/// <returns>反序列化后的对象</returns>
		public static T Deserialize<T>(string xmlData)
		{
			if(string.IsNullOrEmpty(xmlData)) return default(T);

			return (T)Deserialize(xmlData, typeof(T));
		}

		/// <summary>从 XML 文本反序列化成对象</summary>
		/// <param name="xmlData">XML 数据</param>
		/// <param name="returnType">返回的对象类型</param>
		/// <returns>反序列化后的对象</returns>
		public static object Deserialize(string xmlData, Type returnType)
		{
			if(xmlData.IsNullOrEmpty_()) return null;

			using(TextReader _reader = new StringReader(xmlData))
				return new XmlSerializer(returnType).Deserialize(_reader);
		}

		/// <summary>保存要进行 XML 序列化的对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="obj">要序列化的对象</param>
		/// <param name="fileName">文件名</param>
		/// <returns>指示是否保存成功</returns>
		public static bool Save<T>(T obj, string fileName)
		{
			if(obj == null || fileName.IsNullOrEmpty_()) return false;

			try
			{
				using(FileStream _file = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
				{
					XmlSerializer _xml = new XmlSerializer(typeof(T));
					_xml.Serialize(_file, obj);
				}
				return true;
			}
			catch { return false; }
		}

		/// <summary>将对象序列化为 XML 格式文本</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="obj">要序列化的对象</param>
		/// <returns>序列化后的 XML 格式文本</returns>
		public static string Serialize<T>(T obj)
		{
			if(obj == null) return string.Empty;

			StringBuilder _sb = new StringBuilder();
			using(TextWriter _writer = new StringWriter(_sb))
			{
				XmlSerializer _xml = new XmlSerializer(typeof(T));
				_xml.Serialize(_writer, obj);
			}
			return _sb.ToString();
		}
	}
}