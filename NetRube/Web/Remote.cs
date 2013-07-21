using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace NetRube.Web
{
	/// <summary>NetRube Web 远程内容处理类库</summary>
	public class Remote : IDisposable
	{
		/// <summary>以指定 URL 初始化一个新 <see cref="Remote" /> 实例。</summary>
		/// <param name="url">要连接的远程 URL</param>
		public Remote(string url)
		{
			this.Url = url;
			this.Referer = url;
			this.ContentType = "application/x-www-form-urlencoded";
			this.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1)";
		}

		// 数据缓存
		private byte[] DATA = null;
		private Stream STREAM = null;
		private string STRING = null;

		#region IDisposable 成员
		/// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
		public void Dispose()
		{
			this.ClearCache();
		}
		#endregion

		#region 属性

		/// <summary>获取请求的网址</summary>
		/// <value>请求的网址</value>
		public string Url { get; private set; }

		private string __Method = "GET";
		/// <summary>获取或设置是否以 POST 方法请求</summary>
		/// <value>如果为 POST 方法请求，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsPost
		{
			get { return this.__Method.Equals("POST", StringComparison.OrdinalIgnoreCase); }
			set { this.__Method = value ? "POST" : "GET"; }
		}

		/// <summary>获取或设置允许接收的内容</summary>
		/// <value>允许接收的内容</value>
		public string Accep { get; set; }

		/// <summary>获取或设置发送内容的类型</summary>
		/// <value>发送内容的类型</value>
		public string ContentType { get; set; }

		/// <summary>获取或设置发送给远程的来源网址</summary>
		/// <value>发送给远程的来源网址</value>
		public string Referer { get; set; }

		/// <summary>获取或设置发送的数据</summary>
		/// <value>发送的数据</value>
		public string PostData { get; set; }

		/// <summary>获取或设置远程使用的编码</summary>
		/// <value>远程使用的编码</value>
		public string Charset { get; set; }

		/// <summary>获取或设置 UserAgent 标头</summary>
		/// <value>UserAgent 标头</value>
		public string UserAgent { get; set; }

		/// <summary>获取或设置发送的 Cookie</summary>
		/// <value>Cookie</value>
		public CookieContainer Cookies { get; set; }

		#endregion

		#region 方法
		/// <summary>将远程数据内容保存到指定文件</summary>
		/// <param name="fileName">要保存的文件名</param>
		public void Save(string fileName)
		{
			this.GetData();
			if(this.DATA == null) return;

			using(var file = new FileStream(fileName, FileMode.Create))
			{
				file.Write(this.DATA, 0, this.DATA.Length);
				file.Flush();
			}
		}

		/// <summary>清除缓存。用于重新获取网络流数据</summary>
		public void ClearCache()
		{
			this.DATA = null;
			this.STRING = null;
			if(this.STREAM != null)
			{
				this.STREAM.Dispose();
				this.STREAM = null;
			}
		}

		/// <summary>获取远程网页内容</summary>
		/// <returns>远程网页的内容</returns>
		public string GetString()
		{
			if(this.STRING != null) return this.STRING;

			this.GetStream();
			if(this.STREAM == null) return string.Empty;

			var len = this.STREAM.Length;
			if(len == 0) return string.Empty;

			Encoding encode = this.Charset.IsNullOrEmpty_() ? Encoding.UTF8 : Encoding.GetEncoding(this.Charset);
			using(var reader = new StreamReader(this.STREAM, encode, true))
			{
				var i = 0;
				var data = new char[len];
				while(i < len)
				{
					i += reader.Read(data, i, 1024);
				}
				return new string(data);
			}
		}

		/// <summary>获取远程数据流</summary>
		/// <returns>远程数据流</returns>
		public Stream GetStream()
		{
			if(this.STREAM != null) return this.STREAM;
			this.GetData();
			if(this.DATA.IsNullOrEmpty_()) return null;
			this.STREAM = new MemoryStream(this.DATA);
			return this.STREAM;
		}

		/// <summary>获取远程数据</summary>
		/// <returns>远程数据</returns>
		/// <exception cref="NetRube.ArgumentNullOrEmptyException">指定的 URL 为 null 或 empty</exception>
		/// <exception cref="System.Net.WebException">指定的 URL 无法访问</exception>
		public byte[] GetData()
		{
			if(this.DATA != null) return this.DATA;

			if(string.IsNullOrEmpty(this.Url))
				throw new ArgumentNullOrEmptyException("Url");

			string _url = this.Url;
			bool _hasPostData = !string.IsNullOrEmpty(this.PostData);
			if(!_hasPostData) this.IsPost = false;
			bool _isPost = this.IsPost;

			if(!_isPost && _hasPostData)
			{
				string _connect = _url.Contains("?") ? "&" : "?";
				_url = string.Concat(this.Url, _connect, this.PostData);
			}

			HttpWebRequest _request = WebRequest.Create(_url) as HttpWebRequest;
			_request.ContentType = this.ContentType;
			_request.Method = this.__Method;
			_request.Referer = this.Referer;
			_request.UserAgent = this.UserAgent;
			if(!string.IsNullOrEmpty(this.Accep))
				_request.Accept = this.Accep;
			if(this.Cookies != null && this.Cookies.Count > 0)
				_request.CookieContainer = this.Cookies;
			Encoding _encode = string.IsNullOrEmpty(this.Charset) ? Encoding.UTF8 : Encoding.GetEncoding(this.Charset);

			if(_isPost && _hasPostData)
			{
				byte[] _data = _encode.GetBytes(this.PostData);
				int _len = _data.Length;
				_request.ContentLength = _len;
				using(Stream _dataStream = _request.GetRequestStream())
					_dataStream.Write(_data, 0, _len);
			}

			using(HttpWebResponse _response = _request.GetResponse() as HttpWebResponse)
			{
				int _statusCode = (int)_response.StatusCode;
				if(_statusCode < 200 || _statusCode >= 300)
					throw new WebException(Localization.Resources.UrlUnableAccess.F(_statusCode));

				using(var stream = _response.GetResponseStream())
				{
					var len = _response.ContentLength;
					if(len == 0) return new byte[0];

					var i = 0;
					var l = 1024;
					this.DATA = new byte[len];
					while(i < len)
					{
						if(i + l >= len)
							l = (int)(len - i);
						i += stream.Read(this.DATA, i, l);
					}

					return this.DATA;
				}
			}
		}

		/// <summary>以指定开始和结束标记方式并使用获取远程网页中的部分内容，如果不存在开始或结束字符串将返回空字符串</summary>
		/// <param name="startString">标记为开始的字符串</param>
		/// <param name="endString">标记为结束的字符串</param>
		/// <param name="include">是否包含标记的字符串</param>
		/// <returns>远程网页中的部分内容</returns>
		public string GetString(string startString, string endString, bool include)
		{
			if(string.IsNullOrEmpty(startString) || string.IsNullOrEmpty(endString))
				return string.Empty;

			string _remoteString = this.GetString();
			return _remoteString.GetSubstr_(startString, endString, include);
		}

		/// <summary>以指定开始和结束标记方式并使用获取远程网页中的部分内容，如果不存在开始或结束字符串将返回空字符串</summary>
		/// <param name="startString">标记为开始的字符串</param>
		/// <param name="endString">标记为结束的字符串</param>
		/// <returns>远程网页中的部分内容</returns>
		public string GetString(string startString, string endString)
		{
			return this.GetString(startString, endString, false);
		}

		/// <summary>以正则匹配方式获取远程网页中的部分内容</summary>
		/// <param name="pattern">正则匹配模式，只获取以“(?&lt;STR&gt;...)”显式命名的组内容</param>
		/// <returns>远程网页中的部分内容</returns>
		public string GetString(string pattern)
		{
			if(string.IsNullOrEmpty(pattern)) return string.Empty;

			string _remoteString = this.GetString();
			Match _match = Regex.Match(_remoteString, pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			if(_match.Success)
				return _match.Groups["STR"].Value;
			return string.Empty;
		}

		/// <summary>获取远程 XML 文件内容</summary>
		/// <param name="path">XML 文件路径或 URL</param>
		/// <returns>XML 文档</returns>
		public XmlDocument GetXml(string path)
		{
			return Utils.GetXml(path);
		}
		#endregion
	}
}
