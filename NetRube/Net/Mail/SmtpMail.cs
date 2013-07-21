using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace NetRube.Net
{
	/// <summary>邮件发送类库</summary>
	public class SmtpMail
	{
		private List<string> TO;
		private List<string> CC;

		#region 属性
		/// <summary>获取发送错误信息</summary>
		/// <value>错误信息</value>
		public string ErrorMessage { private set; get; }

		#region 邮件相关
		/// <summary>设置邮件主题</summary>
		/// <value>邮件主题</value>
		public string Subject { set; private get; }

		/// <summary>设置邮件正文</summary>
		/// <value>邮件正文</value>
		public string Body { set; private get; }

		/// <summary>设置邮件正文是否为 HTML 格式</summary>
		/// <value>如果邮件正文为 HTML 格式，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsBodyHtml { set; private get; }

		/// <summary>设置发送人的电子邮件地址</summary>
		/// <value>发送人的电子邮件地址</value>
		public string From { set; private get; }

		/// <summary>设置发送人显示名称</summary>
		/// <value>发送人显示名称</value>
		public string DisplayName { set; private get; }

		/// <summary>设置邮件编码</summary>
		/// <value>邮件编码</value>
		public Encoding CharSet { set; private get; }

		/// <summary>设置设置邮件优先级别</summary>
		/// <value>邮件优先级别</value>
		public MailPriority Priority { set; private get; }
		#endregion

		#region 服务器相关
		/// <summary>设置发送邮件服务器的名称或 IP 地址</summary>
		/// <value>发送邮件服务器的名称或 IP 地址</value>
		public string Host { set; private get; }

		/// <summary>设置发送邮件服务器端口</summary>
		/// <value>发送邮件服务器端口</value>
		public int Port { set; private get; }

		/// <summary>设置发送邮件服务器帐号</summary>
		/// <value>发送邮件服务器帐号</value>
		public string UserName { set; private get; }

		/// <summary>设置发送邮件服务器密码</summary>
		/// <value>邮件服务器密码</value>
		public string Password { set; private get; }

		/// <summary>设置发送邮件服务器是否需要验证</summary>
		/// <value>如果发送邮件服务器需要验证，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool UseCredentials { set; private get; }
		#endregion
		#endregion

		#region 构造函数
		/// <summary>初始化一个新 <see cref="SmtpMail" /> class 实例。</summary>
		public SmtpMail()
		{
			this.TO = new List<string>();
			this.CC = new List<string>();
			this.IsBodyHtml = true;
			this.CharSet = Encoding.UTF8;
			this.Port = 25;
			this.UseCredentials = true;
		}
		#endregion

		#region 方法
		#region 添加收件人
		/// <summary>添加收件人的电子邮件地址</summary>
		/// <param name="email">电子邮件地址</param>
		public void AddTo(string email)
		{
			if(email.IsNullOrEmpty_()) return;
			this.TO.Add(email);
		}

		/// <summary>添加收件人的电子邮件地址列表</summary>
		/// <param name="emails">电子邮件地址列表</param>
		public void AddToList(IEnumerable<string> emails)
		{
			if(emails.IsNullOrEmpty_()) return;
			foreach(string email in emails)
				this.AddTo(email);
		}
		#endregion

		#region 添加抄送
		/// <summary>添加抄送收件人的电子邮件地</summary>
		/// <param name="email">电子邮件地址</param>
		public void AddCC(string email)
		{
			if(email.IsNullOrEmpty_()) return;
			this.CC.Add(email);
		}

		/// <summary>添加抄送收件人的电子邮件地址列表</summary>
		/// <param name="emails">电子邮件地址列表</param>
		public void AddCCList(IEnumerable<string> emails)
		{
			if(emails.IsNullOrEmpty_()) return;
			foreach(string email in emails)
				this.AddCC(email);
		}
		#endregion

		#region 发送邮件
		/// <summary>发送邮件</summary>
		/// <returns>指示是否发送成功</returns>
		public bool Send()
		{
			using(MailMessage mm = new MailMessage())
			{
				foreach(string mail in this.TO)
					mm.To.Add(mail);
				foreach(string mail in this.CC)
					mm.CC.Add(mail);
				switch(this.Priority)
				{
					case MailPriority.High:
						mm.Priority = System.Net.Mail.MailPriority.High;
						break;
					case MailPriority.Low:
						mm.Priority = System.Net.Mail.MailPriority.Low;
						break;
					default:
						mm.Priority = System.Net.Mail.MailPriority.Normal;
						break;
				}

				mm.Subject = this.Subject;
				mm.SubjectEncoding = this.CharSet;
				mm.Body = this.Body;
				mm.BodyEncoding = this.CharSet;
				mm.IsBodyHtml = this.IsBodyHtml;
				mm.From = new MailAddress(this.From, this.DisplayName, this.CharSet);

				using(SmtpClient sc = new SmtpClient(this.Host, this.Port))
				{
					// sc.UseDefaultCredentials = !this.UseCredentials;		// Mono 不支持 UseDefaultCredentials 属性的设置
					sc.Credentials = new NetworkCredential(this.UserName, this.Password);
					try { sc.Send(mm); }
					catch(Exception e)
					{
						this.ErrorMessage = e.Message;
						return false;
					}
					return true;
				}
			}
		}
		#endregion
		#endregion
	}
}