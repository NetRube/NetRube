using System.ComponentModel;

namespace NetRube.Net
{
	/// <summary>邮件优先级别</summary>
	public enum MailPriority
	{
		/// <summary>高</summary>
		[Description("高")]
		High,
		/// <summary>正常</summary>
		[Description("正常")]
		Normal,
		/// <summary>低</summary>
		[Description("低")]
		Low
	}
}