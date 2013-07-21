using System.ComponentModel;

namespace NetRube
{
	/// <summary>提示信息类型</summary>
	public enum MessageType
	{
		/// <summary>提示</summary>
		[Description("提示")]
		Information,
		/// <summary>操作成功</summary>
		[Description("成功")]
		Success,
		/// <summary>警告</summary>
		[Description("警告")]
		Warning,
		/// <summary>错误</summary>
		[Description("错误")]
		Error
	}
}