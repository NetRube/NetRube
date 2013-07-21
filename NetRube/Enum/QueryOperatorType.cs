using System.ComponentModel;

namespace NetRube
{
	/// <summary>查询比较运算符类型</summary>
	public enum QueryOperatorType
	{
		/// <summary>等于（默认值）</summary>
		[Description("等于")]
		Equal,
		/// <summary>不等于</summary>
		[Description("不等于")]
		NotEqual,
		/// <summary>大于</summary>
		[Description("大于")]
		GreaterThan,
		/// <summary>小于</summary>
		[Description("小于")]
		LessThan,
		/// <summary>大于或等于</summary>
		[Description("大于或等于")]
		GreaterThanOrEqual,
		/// <summary>小于或等于</summary>
		[Description("小于或等于")]
		LessThanOrEqual,
		/// <summary>包含</summary>
		[Description("包含")]
		Contains,
		/// <summary>以……开头</summary>
		[Description("以……开头")]
		StartsWith,
		/// <summary>以……结尾</summary>
		[Description("以……结尾")]
		EndsWith
	}
}