using System.ComponentModel;

namespace NetRube
{
	/// <summary>日期时间比较类型</summary>
	public enum DateTimeCompareType
	{
		/// <summary>默认</summary>
		[Description("默认")]
		None,
		/// <summary>年</summary>
		[Description("按年")]
		ByYear,
		/// <summary>半年</summary>
		[Description("按半年")]
		ByHalfYear,
		/// <summary>季</summary>
		[Description("按季")]
		ByQuarter,
		/// <summary>月</summary>
		[Description("按月")]
		ByMonth,
		/// <summary>周</summary>
		[Description("按周")]
		ByWeek,
		/// <summary>天</summary>
		[Description("按天")]
		ByDay,
		/// <summary>小时</summary>
		[Description("按小时")]
		ByHour,
		/// <summary>分钟</summary>
		[Description("按分钟")]
		ByMinute,
		/// <summary>秒钟</summary>
		[Description("按秒钟")]
		BySecond
	}
}