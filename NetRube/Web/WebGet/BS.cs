using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace NetRube.Web
{
	/// <summary>NetRube Web 客户端和远程数据获取类库</summary>
	public static partial class WebGet
	{
		// 服务器及浏览器

		/// <summary>获取服务器端变量</summary>
		/// <param name="name">变量名称</param>
		/// <returns>服务器端变量的值</returns>
		public static string GetServerVariables(string name)
		{
			if(string.IsNullOrEmpty(name)) return string.Empty;
			string _retval = HttpContext.Current.Request.ServerVariables.Get(name);
			return _retval ?? string.Empty;
		}

		/// <summary>获取客户端浏览器是否支持 GZIP 压缩</summary>
		/// <value>如果客户端浏览器支持 GZIP 压缩，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public static bool IsSupportGzip
		{
			get
			{
				string _value = GetServerVariables("HTTP_ACCEPT_ENCODING").ToUpperInvariant();
				return _value.Contains("GZIP");
			}
		}

		/// <summary>获取客户端浏览器语言</summary>
		/// <value>客户端浏览器语言</value>
		public static string ClientLanguane
		{
			get { return GetServerVariables("HTTP_ACCEPT_LANGUAGE"); }
		}

		/// <summary>获取客户端 IP 地址</summary>
		/// <value>客户端 IP 地址</value>
		public static string IP
		{
			get
			{
				string ip = "0.0.0.0";
				string HTTP_VIA = GetServerVariables("HTTP_VIA");
				string REMOTE_ADDR = GetServerVariables("REMOTE_ADDR");
				if(!HTTP_VIA.IsNullOrEmpty_())
				{
					string HTTP_X_FORWARDED_FOR = GetServerVariables("HTTP_X_FORWARDED_FOR");
					if(HTTP_X_FORWARDED_FOR.IsNullOrEmpty_())
					{
						string HTTP_CLIENT_IP = GetServerVariables("HTTP_CLIENT_IP");
						if(!HTTP_CLIENT_IP.IsNullOrEmpty_())
							ip = HTTP_CLIENT_IP;
						else if(!REMOTE_ADDR.IsNullOrEmpty_())
							ip = REMOTE_ADDR;
					}
					else
						ip = HTTP_X_FORWARDED_FOR;
				}
				else if(!REMOTE_ADDR.IsNullOrEmpty_())
					ip = REMOTE_ADDR;
				if(ip.IsNullOrEmpty_())
					ip = HttpContext.Current.Request.UserHostAddress;

				if(!WebUtils.IsIP(ip))
					ip = "0.0.0.0";
				return ip;
			}
		}

		/// <summary>获取当前域名</summary>
		/// <value>当前域名</value>
		public static string HostName
		{
			get
			{
				Uri _url = HttpContext.Current.Request.Url;
				if(_url.IsDefaultPort)
					return string.Concat("http://", _url.Host);
				return string.Concat("http://", _url.Host, ":", _url.Port.ToString());
			}
		}

		/// <summary>获取当前页面 URL</summary>
		/// <value>当前页面 URL</value>
		public static string PageUrl
		{
			get { return HttpContext.Current.Request.Url.ToString(); }
		}

		/// <summary>获取当前页面文件名</summary>
		/// <value>当前页面文件名</value>
		public static string PageName
		{
			get
			{
				string _page = HttpContext.Current.Request.Url.AbsolutePath;
				_page = Utils.GetFileName(_page);
				return _page;
			}
		}

		/// <summary>获取来访页面 URL</summary>
		/// <value>来访页面 URL</value>
		public static string UrlReferrer
		{
			get { return HttpContext.Current.Request.UrlReferrer.ToString(); }
		}

		/// <summary>获取当前请求的原始 URL</summary>
		/// <value>当前请求的原始 URL</value>
		public static string RawUrl
		{
			get { return HttpContext.Current.Request.RawUrl; }
		}

		/// <summary>获取客户端浏览器用户代理信息</summary>
		/// <value>客户端浏览器用户代理信息</value>
		public static string UserAgent
		{
			get { return HttpContext.Current.Request.UserAgent; }
		}

		private static string[] __ReadTextConfig(string path)
		{
			using(StreamReader sr = new StreamReader(path, Encoding.UTF8))
			{
				List<string> list = new List<string>();
				string line;
				while(!sr.EndOfStream)
				{
					line = sr.ReadLine().Trim();
					if(!line.IsNullOrEmpty_())
						list.Add(line);
				}
				return list.ToArray();
			}
		}

		private static void __WriteTextConfig(string path, string[] array)
		{
			Utils.WriteTextFile(array.Join_(Environment.NewLine), path);
		}

		private static string[] __Browsers;
		/// <summary>检测是否通过浏览器访问</summary>
		/// <value>如果是通过浏览器访问，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public static bool IsBrowser
		{
			get
			{
				string _type = UserAgent;
				if(string.IsNullOrEmpty(_type)) return false;

				if(null == __Browsers)
				{
					string _path = Utils.GetMapPath(@"App_Data\IsBroswerData");
					if(Utils.FileExists(_path))
						__Browsers = __ReadTextConfig(_path);
					else
					{
						__Browsers = new string[]{
							"ie",
							"opera",
							"netscape",
							"khtml",
							"gecko",
							"konqueror",
							"aol"
						};
						__WriteTextConfig(_path, __Browsers);
					}
				}
				return _type.In_(true, false, false, __Browsers);
			}
		}

		private static string[] __SearchCrawler;
		/// <summary>判断是否是搜索引擎来爬网</summary>
		/// <value>如果是搜索引擎来爬网，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public static bool IsSearchCrawler
		{
			get
			{
				string _agent = UserAgent;
				if(string.IsNullOrEmpty(_agent)) return false;

				if(null == __SearchCrawler)
				{
					string _path = Utils.GetMapPath(@"App_Data\IsSearchCrawlerData");
					if(Utils.FileExists(_path))
						__SearchCrawler = __ReadTextConfig(_path);
					else
					{
						__SearchCrawler = new string[]{
							"bot",
							"spider",
							"search",
							"slurp",
							"seek",
							"ia"
						};
						__WriteTextConfig(_path, __SearchCrawler);
					}
				}
				return _agent.In_(true, false, false, __SearchCrawler);
			}
		}

		private static string[] __SearchEngines;
		/// <summary>判断是否通过搜索引擎来访问</summary>
		/// <value>如果是通过搜索引擎来访问，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public static bool IsFromSearchEngine
		{
			get
			{
				string _url = UrlReferrer;
				if(string.IsNullOrEmpty(_url)) return false;

				if(null == __SearchEngines)
				{
					string _path = Utils.GetMapPath(@"App_Data\IsFromSearchEnginesData");
					if(Utils.FileExists(_path))
						__SearchEngines = __ReadTextConfig(_path);
					else
					{
						__SearchEngines = new string[]{
							"google",
							"baidu",
							"bing",
							"yahoo",
							"sohu", 
							"live",
							"sogou",
							"youdao",
							"lycos",
							"yisou",
							"iask",
							"soso",
							"gougou",
							"zhongsou",
							"tom"
						};
						__WriteTextConfig(_path, __SearchEngines);
					}
				}
				return _url.In_(true, false, false, __SearchEngines);
			}
		}

		private static string[] __Mobiles;
		/// <summary>判断是否通过移动电话来访问</summary>
		/// <value>如果是通过移动电话来访问，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public static bool IsMobile
		{
			get
			{
				string _agent = UserAgent;
				if(string.IsNullOrEmpty(_agent)) return false;

				if(null == __Mobiles)
				{
					string _path = Utils.GetMapPath(@"App_Data\IsMobileData");
					if(Utils.FileExists(_path))
						__Mobiles = __ReadTextConfig(_path);
					else
					{
						__Mobiles = new string[]{
							"alcatel",		//阿尔卡特
							"amoi",			//夏新
							"asus",			//华硕
							"benq",			//明基
							"bird",			//波导
							"capitel",		//首信
							"cect",			//中电通信
							"codacom",
							"cosmos",
							"daxian",		//大显
							"dbtel",		//迪比特
							"diamond",
							"dopod",		//多普达
							"eastcom",		//东信
							"emol",			//熊猫
							"ericsson",		//爱立信
							"ezze",
							"geo",
							"haier",		//海尔
							"herojet",
							"iac",			//英华达
							"inn",			//中侨
							"kejian",		//科健
							"konka",		//康佳
							"kyocera",		//京瓷
							"lenovo",		//联想
							"lg",			//LG
							"meijin",		//名人
							"mitsu",		//三菱
							"mot",			//摩托罗拉
							"nec",			//NEC
							"newgen",
							"nokia",		//诺基亚
							"ica",
							"pan",			//多彩,松下,熊猫
							"pt-",			//泛泰,托普
							"philips",		//飞利浦
							"sagem",		//萨基姆
							"sgh",			//三星
							"sch",			//三星
							"scp",			//三洋
							"sed",			//桑达
							"sharp",		//夏普
							"sie",			//西门子
							"sonyericsson",	//索爱
							"soutec",		//南方高科
							"tcl",			//TCL
							"top",			//天诺思
							"viking",
							"xplore",
							"zetta",
							"zte"			//中兴
						};
						__WriteTextConfig(_path, __Mobiles);
					}
				}
				return _agent.In_(true, false, false, __Mobiles);
			}
		}


		private static XmlDocument __BrowsersType;
		/// <summary>获取客户端浏览器类型</summary>
		/// <value>客户端浏览器类型</value>
		public static NameAndVersion ClientBrowser
		{
			get
			{
				string _agent = UserAgent;
				if(string.IsNullOrEmpty(_agent)) return new NameAndVersion();

				if(null == __BrowsersType)
				{
					__BrowsersType = new XmlDocument();
					string _path = Utils.GetMapPath(@"App_Data\BrowsersTypeData");
					if(!Utils.FileExists(_path))
					{
						XmlDeclaration _dec = __BrowsersType.CreateXmlDeclaration("1.0", "UTF-8", null);
						__BrowsersType.AppendChild(_dec);
						XmlElement _docElement = __BrowsersType.CreateElement("BrowsersType");
						__BrowsersType.AppendChild(_docElement);

						XmlElement _el = __BrowsersType.CreateElement("Browser");
						_el.SetAttribute("name", "Opera");
						_el.SetAttribute("regexp", @"opera[ /](?<ver>[\d\.]+)");
						_docElement.AppendChild(_el);

						_el = __BrowsersType.CreateElement("Browser");
						_el.SetAttribute("name", "Internet Explorer");
						_el.SetAttribute("regexp", @"msie (?<ver>[\d\.]+)");
						_docElement.AppendChild(_el);

						_el = __BrowsersType.CreateElement("Browser");
						_el.SetAttribute("name", "Firefox");
						_el.SetAttribute("regexp", @"firefox/(?<ver>[\d\.]+)");
						_docElement.AppendChild(_el);

						_el = __BrowsersType.CreateElement("Browser");
						_el.SetAttribute("name", "Chrome");
						_el.SetAttribute("regexp", @"chrome/(?<ver>[\d\.]+)");
						_docElement.AppendChild(_el);

						_el = __BrowsersType.CreateElement("Browser");
						_el.SetAttribute("name", "Safari");
						_el.SetAttribute("regexp", @"version/(?<ver>[\d\.]+)");
						_docElement.AppendChild(_el);

						__BrowsersType.Save(_path);
					}
					else
						__BrowsersType.Load(_path);
				}

				return __GetVersion(_agent, __BrowsersType);
			}
		}

		private static XmlDocument __OSType;
		/// <summary>获取客户端操作系统类型</summary>
		/// <value>客户端操作系统类型</value>
		public static NameAndVersion ClientOS
		{
			get
			{
				string _agent = UserAgent;
				if(string.IsNullOrEmpty(_agent)) return new NameAndVersion();
				//_agent = _agent.ToLowerInvariant();

				if(null == __OSType)
				{
					__OSType = new XmlDocument();
					string _path = Utils.GetMapPath(@"App_Data\OSTypeData");
					if(!Utils.FileExists(_path))
					{
						XmlDeclaration _dec = __OSType.CreateXmlDeclaration("1.0", "UTF-8", null);
						__OSType.AppendChild(_dec);
						XmlElement _docElement = __OSType.CreateElement("OSType");
						__OSType.AppendChild(_docElement);

						XmlElement _el = __OSType.CreateElement("OS");
						_el.SetAttribute("name", "Windows 7/Windows Server 2008 R2");
						_el.SetAttribute("regexp", @"nt 6\.1");
						_docElement.AppendChild(_el);

						_el = __OSType.CreateElement("OS");
						_el.SetAttribute("name", "Windows Vista/Windows Server 2008");
						_el.SetAttribute("regexp", "nt 6");
						_docElement.AppendChild(_el);

						_el = __OSType.CreateElement("OS");
						_el.SetAttribute("name", "Windows 2003");
						_el.SetAttribute("regexp", @"nt 5\.2");
						_docElement.AppendChild(_el);

						_el = __OSType.CreateElement("OS");
						_el.SetAttribute("name", "Windows XP");
						_el.SetAttribute("regexp", @"nt 5\.1");
						_docElement.AppendChild(_el);

						_el = __OSType.CreateElement("OS");
						_el.SetAttribute("name", "Mac OS");
						_el.SetAttribute("regexp", @"mac os(?<ver>[^;]+)");
						_docElement.AppendChild(_el);

						_el = __OSType.CreateElement("OS");
						_el.SetAttribute("name", "Linux");
						_el.SetAttribute("regexp", "linux");
						_docElement.AppendChild(_el);

						__OSType.Save(_path);
					}
					else
						__OSType.Load(_path);
				}

				return __GetVersion(_agent, __OSType);
			}
		}

		private static NameAndVersion __GetVersion(string agent, XmlDocument xml)
		{
			NameAndVersion nv = new NameAndVersion();
			Match _match;
			foreach(XmlNode _xn in xml.DocumentElement.ChildNodes)
			{
				_match = Regex.Match(agent, _xn.Attributes["regexp"].Value, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if(!_match.Success) continue;
				nv.Name = _xn.Attributes["name"].Value;
				if(_match.Groups["ver"].Success)
					nv.Version = _match.Groups["ver"].Value;
				break;
			}

			return nv;
		}
	}
}