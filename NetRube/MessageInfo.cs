using System.Collections.Generic;

namespace NetRube
{
	/// <summary>提示信息</summary>
	public class MessageInfo
	{
		/// <summary>初始化一个新 <see cref="MessageInfo" /> class 实例。</summary>
		public MessageInfo()
		{
			this.Title = string.Empty;
			this.Type = MessageType.Warning;
			this.HasAppError = false;
			this.Content = new List<string>();
		}

		#region 属性
		/// <summary>获取或设置信息标题</summary>
		/// <value>信息标题</value>
		public string Title { get; set; }

		/// <summary>获取或设置信息类型</summary>
		/// <value>信息类型</value>
		public MessageType Type { get; set; }

		/// <summary>获取或设置信息内容</summary>
		/// <value>信息内容</value>
		public List<string> Content { get; set; }

		/// <summary>获取或设置信息数量</summary>
		/// <value>信息数量</value>
		public int Count { get { return this.Content.Count; } }

		/// <summary>获取或设置是否包含有提示信息</summary>
		/// <value>如果包含有提示信息，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool HasMessage { get { return this.Content.Count > 0; } }

		/// <summary>获取或设置是否为应用程序错误</summary>
		/// <value>如果为应用程序错误，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool HasAppError { get; set; }

		/// <summary>获取自动跳转脚本</summary>
		/// <value>自动跳转脚本</value>
		public string RedirectScript { get; private set; }

		/// <summary>获取或设置附带返回的结果</summary>
		/// <value>附带返回的结果</value>
		public object Result { get; set; }
		#endregion

		#region 添加信息
		/// <summary>
		/// 添加信息
		/// </summary>
		/// <param name="message">信息内容</param>
		/// <returns>此提示信息</returns>
		public MessageInfo AddMessage(string message)
		{
			if(message.IsNullOrEmpty_()) return this;

			this.Content.Add(message);
			return this;
		}

		/// <summary>
		/// 添加链接
		/// </summary>
		/// <param name="title">链接标题</param>
		/// <param name="url">链接地址</param>
		/// <param name="inNewWindow">是否在新窗口中打开</param>
		/// <returns>此提示信息</returns>
		public MessageInfo AddLink(string title, string url, bool inNewWindow = false)
		{
			if(title.IsNullOrEmpty_()) return this;

			if(!inNewWindow)
				return this.AddMessage("<a href=\"" + url + "\">" + title + "</a>");
			else
				return this.AddMessage("<a href=\"" + url + "\" target=\"_blank\">" + title + "</a>");
		}

		/// <summary>
		/// 添加后退链接
		/// </summary>
		/// <returns>此提示信息</returns>
		public MessageInfo AddBackLink()
		{
			return this.AddMessage("<a href=\"javascript:history.go(-1)\">返回上一页</a>");
		}

		/// <summary>
		/// 添加后退链接
		/// </summary>
		/// <param name="url">链接地址</param>
		/// <returns>此提示信息</returns>
		public MessageInfo AddBackLink(string url)
		{
			return this.AddMessage("<a href=\"" + url + "\">返回上一页</a>");
		}
		#endregion

		/// <summary>
		/// 验证数据。条件 <paramref name="valid" /> 成立(为 true)时添加提示信息并返回 false; 否则直接返回 true
		/// </summary>
		/// <param name="valid">验证条件</param>
		/// <param name="msg">未通过提示信息</param>
		/// <returns>
		/// 条件 <paramref name="valid" /> 成立(为 true)时添加提示信息并返回 false; 否则直接返回 true
		/// </returns>
		public bool Validate(bool valid, params string[] msg)
		{
			if(!valid) return true;
			msg.ForEach_(m => this.AddMessage(m));
			this.Type = MessageType.Warning;
			return false;
		}

		/// <summary>相加</summary>
		/// <param name="self">本提示信息</param>
		/// <param name="value">另一个提示信息</param>
		/// <returns>此提示信息</returns>
		public static MessageInfo operator +(MessageInfo self, MessageInfo value)
		{
			if(value.HasMessage)
				self.Content.AddRange(value.Content);
			if(!value.Title.IsNullOrEmpty_())
				self.Title = value.Title;
			if(!value.RedirectScript.IsNullOrEmpty_())
				self.RedirectScript = value.RedirectScript;
			self.Type = value.Type;

			return self;
		}

		/// <summary>设置自动跳转链接</summary>
		/// <param name="title">链接标题</param>
		/// <param name="url">链接地址</param>
		/// <param name="seconds">多少秒后自动跳转</param>
		/// <returns>此提示信息</returns>
		public MessageInfo SetRedirect(string title, string url, int seconds)
		{
			if(url.IsNullOrEmpty_()) return this;
			if(!title.IsNullOrEmpty_())
				this.AddLink("{0} （{1} 秒后自动跳转）".F(title, seconds.ToString()), url);
			this.RedirectScript = "<script type=\"text/javascript\">setTimeout(\"document.location='{0}'\", {1}000);</script>".F(url, seconds.ToString());
			return this;
		}

		/// <summary>清除所有提示信息</summary>
		/// <returns>此提示信息</returns>
		public MessageInfo Clear()
		{
			this.Content.Clear();
			return this;
		}
	}
}