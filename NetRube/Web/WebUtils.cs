using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace NetRube.Web
{
	/// <summary>NetRube Web 实用操作类库</summary>
	public static class WebUtils
	{
		#region 文本操作
		/// <summary>为 HTML 内容添加干扰内容</summary>
		/// <param name="html">要添加干扰内容的 HTML</param>
		/// <param name="noise">干扰内容</param>
		/// <returns>返回在块标签结束前添加了指定干扰内容的 HTML</returns>
		public static string SetHtmlNoise(this string html, string noise)
		{
			if(html.IsNullOrEmpty_()) return string.Empty;
			if(noise.IsNullOrEmpty_()) return html;

			string _html = Regex.Replace(html, "[\r\n]", " ");
			return Regex.Replace(_html, "(</p>|<br />|</div>|</li>|</td>)", noise + "$1", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		/// <summary>移除 HTML 内容前后的空白换行</summary>
		/// <param name="html">要移除的 HTML</param>
		/// <returns>移除后的内容</returns>
		public static string TrimHtmlBreakLine(this string html)
		{
			if(html.IsNullOrEmpty_()) return string.Empty;
			string _html = Regex.Replace(html, "[\r\n]", " ");
			return Regex.Replace(_html, @"^\s*(<br />)*|\s*(<br />)*\s*$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		/// <summary>移除 HTML 标签、样式标签及内容、脚本标签及内容和空格</summary>
		/// <param name="html">要处理的 HTML 内容</param>
		/// <returns>移除后的内容</returns>
		public static string RemoveHtml(this string html)
		{
			if(html.IsNullOrEmpty_()) return string.Empty;
			string _html = Regex.Replace(html, STR.Concat(@"<style.*?>.*?</style>|<script.*?>.*?</script>|<.*?>|\s|&nbsp;|&#160;|", Separator.PageBreak, "|", Separator.PageTitle), " ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			return Regex.Replace(_html, " +?", " ", RegexOptions.Compiled);
		}

		/// <summary>编码换行符，以便在 HTML 中能够让非超文本内容能够换行</summary>
		/// <param name="str">要格式化的字符串</param>
		/// <returns>编码后的内容</returns>
		public static string HtmlBREncode(this string str)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;

			string _reval = str.Replace("\r\n", "<br />");
			_reval = _reval.Replace("\n", "<br />");
			_reval = _reval.Replace("\r", "<br />");
			return _reval;
		}

		/// <summary>JavaScript 脚本字符串编码</summary>
		/// <param name="str">要编码的字符串</param>
		/// <returns>编码后的内容</returns>
		public static string JsEncode(this string str)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;

			string _str = str.Replace(@"\", @"\\");
			_str = _str.Replace("'", @"\'");
			_str = _str.Replace("\"", "\\\"");
			_str = _str.Replace("\r", string.Empty);
			_str = _str.Replace("\n", string.Empty);
			return _str;
		}

		/// <summary>HTML 编码</summary>
		/// <param name="html">HTML 字符串</param>
		/// <returns>编码后的内容</returns>
		public static string HtmlEncode(this string html)
		{
			if(html.IsNullOrEmpty_()) return string.Empty;
			return HttpUtility.HtmlEncode(html);
		}

		/// <summary>HTML 解码</summary>
		/// <param name="html">HTML 字符串</param>
		/// <returns>解码后的内容</returns>
		public static string HtmlDecode(this string html)
		{
			if(html.IsNullOrEmpty_()) return string.Empty;
			return HttpUtility.HtmlDecode(html);
		}

		/// <summary>URL 编码</summary>
		/// <param name="url">URL 字符串</param>
		/// <returns>编码后的内容</returns>
		public static string UrlEncode(this string url)
		{
			if(url.IsNullOrEmpty_()) return string.Empty;
			return HttpUtility.UrlEncode(url);
		}

		/// <summary>URL 解码</summary>
		/// <param name="url">URL 字符串</param>
		/// <returns>解码后的内容</returns>
		public static string UrlDecode(this string url)
		{
			if(url.IsNullOrEmpty_()) return string.Empty;
			return HttpUtility.UrlDecode(url);
		}

		/// <summary>将一个字符串写入 HTTP 响应输出流</summary>
		/// <param name="page">此扩展程序的 Page 实例</param>
		/// <param name="s">要写入 HTTP 响应输出流的字符串</param>
		public static void W(this Page page, string s)
		{
			page.Response.Write(s);
		}

		/// <summary>将 Object 写入 HTTP 响应输出流</summary>
		/// <param name="page">此扩展程序的 Page 实例</param>
		/// <param name="obj">要写入 HTTP 响应输出流的 Object</param>
		public static void W(this Page page, object obj)
		{
			page.Response.Write(obj);
		}

		/// <summary>将一个字符写入 HTTP 响应输出流</summary>
		/// <param name="page">此扩展程序的 Page 实例</param>
		/// <param name="ch">要写入 HTTP 响应输出流的字符</param>
		public static void W(this Page page, char ch)
		{
			page.Response.Write(ch);
		}
		#endregion

		#region Cookies、Session操作
		#region Cookies
		#region 读取
		/// <summary>读取 Cookie</summary>
		/// <param name="name">要读取的 Cookie 名称</param>
		/// <returns>Cookie 值</returns>
		public static string GetCookie(string name)
		{
			if(name.IsNullOrEmpty_()) return string.Empty;
			HttpCookieCollection _cookies = HttpContext.Current.Request.Cookies;
			if(_cookies == null) return string.Empty;
			HttpCookie _cookie = _cookies.Get(name);
			if(_cookie == null) return string.Empty;
			return _cookie.Value;
		}

		/// <summary>读取 Cookie</summary>
		/// <param name="name">要读取的 Cookie 名称</param>
		/// <param name="key">要读取的 Cookie 键</param>
		/// <returns>Cookie 值</returns>
		public static string GetCookie(string name, string key)
		{
			if(name.IsNullOrEmpty_() || key.IsNullOrEmpty_()) return string.Empty;
			HttpCookieCollection _cookies = HttpContext.Current.Request.Cookies;
			if(_cookies == null) return string.Empty;
			HttpCookie _cookie = _cookies.Get(name);
			if(_cookie == null) return string.Empty;
			string _value = _cookie.Values.Get(key);
			if(_value.IsNullOrEmpty_()) return string.Empty;
			return _value;
		}
		#endregion

		#region 设置
		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="values">要设置的值</param>
		public static void SetCookie(string name, Dictionary<string, string> values)
		{
			if(name.IsNullOrEmpty_() || values.IsNullOrEmpty_()) return;
			if(HttpContext.Current.Request.Cookies == null) return;

			HttpCookie _cookie = HttpContext.Current.Request.Cookies.Get(name);
			if(_cookie == null)
				_cookie = new HttpCookie(name);
			foreach(KeyValuePair<string, string> _item in values)
				_cookie.Values.Set(_item.Key, _item.Value);

			HttpContext.Current.Response.AppendCookie(_cookie);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="values">要设置的值</param>
		/// <param name="expires">Cookie 过期时间</param>
		public static void SetCookie(string name, Dictionary<string, string> values, DateTime expires)
		{
			if(name.IsNullOrEmpty_() || values.IsNullOrEmpty_()) return;
			if(HttpContext.Current.Request.Cookies == null) return;

			HttpCookie _cookie = HttpContext.Current.Request.Cookies.Get(name);
			if(_cookie == null)
				_cookie = new HttpCookie(name);
			foreach(KeyValuePair<string, string> _item in values)
				_cookie.Values.Set(_item.Key, _item.Value);
			_cookie.Expires = expires;
			HttpContext.Current.Response.AppendCookie(_cookie);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="values">要设置的值</param>
		/// <param name="expires">Cookie 过期时间，分钟数</param>
		public static void SetCookie(string name, Dictionary<string, string> values, int expires)
		{
			DateTime _exp = DateTime.Now.AddMinutes((double)expires);
			SetCookie(name, values, _exp);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="value">要设置的值</param>
		public static void SetCookie(string name, string value)
		{
			if(name.IsNullOrEmpty_()) return;
			if(HttpContext.Current.Request.Cookies == null) return;

			HttpCookie _cookie = HttpContext.Current.Request.Cookies.Get(name);
			if(_cookie == null)
				_cookie = new HttpCookie(name);
			_cookie.Value = value;
			HttpContext.Current.Response.AppendCookie(_cookie);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="value">要设置的值</param>
		/// <param name="expires">Cookie 过期时间</param>
		public static void SetCookie(string name, string value, DateTime expires)
		{
			if(name.IsNullOrEmpty_()) return;
			if(HttpContext.Current.Request.Cookies == null) return;

			HttpCookie _cookie = HttpContext.Current.Request.Cookies.Get(name);
			if(_cookie == null)
				_cookie = new HttpCookie(name);
			_cookie.Value = value;
			_cookie.Expires = expires;
			HttpContext.Current.Response.AppendCookie(_cookie);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="value">要设置的值</param>
		/// <param name="expires">Cookie 过期时间，分钟数</param>
		public static void SetCookie(string name, string value, int expires)
		{
			DateTime _exp = DateTime.Now.AddMinutes((double)expires);
			SetCookie(name, value, _exp);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="key">要设置的 Cookie 键</param>
		/// <param name="value">要设置的值</param>
		public static void SetCookie(string name, string key, string value)
		{
			if(name.IsNullOrEmpty_() || key.IsNullOrEmpty_()) return;
			if(HttpContext.Current.Request.Cookies == null) return;

			HttpCookie _cookie = HttpContext.Current.Request.Cookies.Get(name);
			if(_cookie == null)
				_cookie = new HttpCookie(name);
			_cookie.Values.Set(key, value);
			HttpContext.Current.Response.AppendCookie(_cookie);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="key">要设置的 Cookie 键</param>
		/// <param name="value">要设置的值</param>
		/// <param name="expires">Cookie 过期时间</param>
		public static void SetCookie(string name, string key, string value, DateTime expires)
		{
			if(name.IsNullOrEmpty_() || key.IsNullOrEmpty_()) return;
			if(HttpContext.Current.Request.Cookies == null) return;

			HttpCookie _cookie = HttpContext.Current.Request.Cookies.Get(name);
			if(_cookie == null)
				_cookie = new HttpCookie(name);
			_cookie.Values.Set(key, value);
			_cookie.Expires = expires;
			HttpContext.Current.Response.AppendCookie(_cookie);
		}

		/// <summary>设置 Cookie 项</summary>
		/// <param name="name">要设置的 Cookie 名称</param>
		/// <param name="key">要设置的 Cookie 键</param>
		/// <param name="value">要设置的值</param>
		/// <param name="expires">Cookie 过期时间，分钟数</param>
		public static void SetCookie(string name, string key, string value, int expires)
		{
			DateTime _exp = DateTime.Now.AddMinutes((double)expires);
			SetCookie(name, key, value, _exp);
		}
		#endregion

		#region 删除
		/// <summary>删除 Cookie 项</summary>
		/// <param name="name">要删除的 Cookie 名称</param>
		public static void DelCookie(string name)
		{
			if(name.IsNullOrEmpty_()) return;
			if(HttpContext.Current.Request.Cookies == null) return;
			HttpCookie _cookie = HttpContext.Current.Request.Cookies.Get(name);
			if(_cookie == null) return;
			_cookie.Values.Clear();
			_cookie.Expires = DateTime.MinValue;
			HttpContext.Current.Response.AppendCookie(_cookie);
		}

		/// <summary>删除所有 Cookie</summary>
		public static void DelAllCookie()
		{
			HttpCookieCollection _cookies = HttpContext.Current.Request.Cookies;
			if(_cookies == null) return;
			_cookies.Clear();
		}
		#endregion
		#endregion

		#region Session
		#region 获取
		/// <summary>获取 Session</summary>
		/// <param name="key">要读取的键</param>
		/// <returns>Session 值</returns>
		public static string GetSession(string key)
		{
			if(key.IsNullOrEmpty_()) return string.Empty;
			if(HttpContext.Current.Session[key] == null) return string.Empty;
			return HttpContext.Current.Session[key].ToString();
		}
		#endregion

		#region 设置
		/// <summary>设置 Session 键值</summary>
		/// <param name="key">要设置的键</param>
		/// <param name="value">要设置的值</param>
		public static void SetSession(string key, object value)
		{
			if(key.IsNullOrEmpty_()) return;
			HttpContext.Current.Session[key] = value;
		}
		#endregion

		#region 删除
		/// <summary>删除 Session 会话项</summary>
		/// <param name="key">要删除的键</param>
		public static void DelSession(string key)
		{
			HttpContext.Current.Session.Remove(key);
		}

		/// <summary>
		/// 删除所有 Session 会话
		/// </summary>
		public static void DelAllSession()
		{
			HttpContext.Current.Session.Clear();
			HttpContext.Current.Session.Abandon();
		}
		#endregion
		#endregion
		#endregion

		#region IP 相关

		/// <summary>验证 IP 地址格式是否为 IPv4 或 IPv6</summary>
		/// <param name="ip">IP 地址</param>
		/// <returns>指示 IP 地址格式是否为 IPv4 或 IPv6</returns>
		public static bool IsIP(string ip)
		{
			byte ver = GetIPVer(ip);
			return ver == 4 || ver == 6;
		}

		/// <summary>验证 IP 地址是否为 IPv4</summary>
		/// <param name="ip">IP 地址</param>
		/// <returns>指示 IP 地址格式是否为 IPv4</returns>
		public static bool IsIPv4(string ip)
		{
			return GetIPVer(ip) == 4;
		}

		/// <summary>验证 IP 地址是否为 IPv6</summary>
		/// <param name="ip">IP 地址</param>
		/// <returns>指示 IP 地址格式是否为 IPv6</returns>
		public static bool IsIPv6(string ip)
		{
			return GetIPVer(ip) == 6;
		}

		/// <summary>获取 IP 地址版本。4 为 IPv4，6 为 IPv6，0 为非 IP</summary>
		/// <param name="ip">IP 地址</param>
		/// <returns>IP 地址版本。4 为 IPv4，6 为 IPv6，0 为非 IP</returns>
		public static byte GetIPVer(string ip)
		{
			if(ip.IsNullOrEmpty_()) return 0;

			IPAddress ipa;
			if(IPAddress.TryParse(ip, out ipa))
			{
				switch(ipa.AddressFamily)
				{
					case System.Net.Sockets.AddressFamily.InterNetwork:
						return 4;
					case System.Net.Sockets.AddressFamily.InterNetworkV6:
						return 6;
				}
			}
			return 0;
		}
		#endregion

		/// <summary>重启 ASP.Net 程序</summary>
		/// <returns>指示是否重启成功</returns>
		public static bool RestartApplication()
		{
			bool success = true;

			try { HttpRuntime.UnloadAppDomain(); }
			catch { success = false; }

			if(!success)
			{
				try
				{
					string cfgPath = HttpContext.Current.Request.PhysicalApplicationPath + @"\Web.config";
					System.IO.File.SetLastWriteTimeUtc(cfgPath, DateTime.UtcNow);
				}
				catch { success = false; }
			}

			return success;
		}
	}
}