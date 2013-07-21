using System;
using System.Globalization;

namespace NetRube
{
	public static partial class Utils
	{
		// 日期时间

		/// <summary>获取指定年份是否为闰年</summary>
		/// <param name="year">年</param>
		/// <returns>指示指定年份是否为闰年</returns>
		public static bool IsLeapYear_(int year)
		{
			return DateTime.IsLeapYear(year);
		}

		/// <summary>获取指定日期是否为闰年</summary>
		/// <param name="value">日期</param>
		/// <returns>指示指定年份是否为闰年</returns>
		public static bool IsLeapYear_(this DateTime value)
		{
			return IsLeapYear_(value.Year);
		}

		/// <summary>获取指定日期所属的季度</summary>
		/// <param name="value">日期</param>
		/// <returns>以 1、2、3、4 标识的四季</returns>
		public static int GetQuarter_(this DateTime value)
		{
			int month = value.Month;
			return month < 4 ? 1 : month < 7 ? 2 : month < 10 ? 3 : 4;
		}

		/// <summary>获取指定日期所属的半年。1 为上半年，2 为下半年</summary>
		/// <param name="value">日期</param>
		/// <returns>以 1、2 标识的上下半年</returns>
		public static int GetHalfYear_(this DateTime value)
		{
			return value.Month > 6 ? 2 : 1;
		}

		/// <summary>获取指定年份中总共有几天</summary>
		/// <param name="year">年</param>
		/// <returns>表示当年的总天数</returns>
		public static int GetDaysInYear_(int year)
		{
			return IsLeapYear_(year) ? 366 : 365;
		}

		/// <summary>获取指定日期当年中总共有几天</summary>
		/// <param name="value">日期</param>
		/// <returns>表示当年的总天数</returns>
		public static int GetDaysInYear_(this DateTime value)
		{
			return GetDaysInYear_(value.Year);
		}

		/// <summary>获取指定日期当月中总共有几天</summary>
		/// <param name="value">日期</param>
		/// <returns>表示当月的总天数</returns>
		public static int GetDaysInMonth_(this DateTime value)
		{
			return GetDaysInMonth_(value.Year, value.Month);
		}

		/// <summary>获取指定月份中总共有几天</summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <returns>表示当月的总天数</returns>
		public static int GetDaysInMonth_(int year, int month)
		{
			return DateTime.DaysInMonth(year, month);
		}

		/// <summary>获取指定日期是当年中的第几周</summary>
		/// <param name="value">日期</param>
		/// <returns>表示第几周的整数</returns>
		public static int GetWeekOfYear_(this DateTime value)
		{
			return new GregorianCalendar()
				.GetWeekOfYear(value, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
		}

		/// <summary>获取指定日期当年中总共有几周</summary>
		/// <param name="value">日期</param>
		/// <returns>表示当年的总周数</returns>
		public static int GetWeeksInYear_(this DateTime value)
		{
			return GetWeeksInYear_(value.Year);
		}

		/// <summary>获取指定年份中总共有几周</summary>
		/// <param name="year">年份</param>
		/// <returns>表示当年的总周数</returns>
		public static int GetWeeksInYear_(int year)
		{
			DateTime dt = new DateTime(year, 12, 31);
			return dt.GetWeekOfYear_();
		}

		/// <summary>获取指定日期当年开始的时间</summary>
		/// <param name="value">日期</param>
		/// <returns>指定日期当年开始的时间</returns>
		public static DateTime GetYearStart_(this DateTime value)
		{
			return new DateTime(value.Year, 1, 1);
		}

		/// <summary>获取指定日期当年结束的时间</summary>
		/// <param name="value">日期</param>
		/// <returns>指定日期当年结束的时间</returns>
		public static DateTime GetYearEnd_(this DateTime value)
		{
			return new DateTime(value.Year, 12, 31, 23, 59, 59, 999);
		}

		/// <summary>获取指定日期当月开始的时间</summary>
		/// <param name="value">日期</param>
		/// <returns>指定日期当月开始的时间</returns>
		public static DateTime GetMonthStart_(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, 1);
		}

		/// <summary>获取指定日期当月结束的时间</summary>
		/// <param name="value">日期</param>
		/// <returns>指定日期当月结束的时间</returns>
		public static DateTime GetMonthEnd_(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.GetDaysInMonth_(), 23, 59, 59, 999);
		}

		/// <summary>获取指定日期当日开始的时间</summary>
		/// <param name="value">日期</param>
		/// <returns>指定日期当日开始的时间</returns>
		public static DateTime GetDayStart_(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day);
		}

		/// <summary>获取指定日期当日结束的时间</summary>
		/// <param name="value">日期</param>
		/// <returns>指定日期当日结束的时间</returns>
		public static DateTime GetDayEnd_(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 999);
		}

		/// <summary>指示指定的日期时间部分是否为零</summary>
		/// <param name="value">日期</param>
		/// <returns>如果时间部分是否为零，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public static bool IsZeroTime_(this DateTime value)
		{
			return (value.Hour == 0
				&& value.Minute == 0
				&& value.Second == 0
				&& value.Millisecond == 0);

		}

		/// <summary>转换为 JSON 格式（yyyy-MM-ddTHH:mm:ss.fffZ）</summary>
		/// <param name="value">日期</param>
		/// <param name="useUTC">是否使用 UTC</param>
		/// <returns>转换后的 JSON 格式</returns>
		public static string ToJson_(this DateTime value, bool useUTC = true)
		{
			if(useUTC)
				return value.ToString("yyyy-MM-ddTHH:mm:ss");
			return value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
		}

		#region 比较
		/// <summary>以指定的比较类型比较指定日期早于、等于或晚于当前指定的日期</summary>
		/// <param name="t1">当前指定的日期</param>
		/// <param name="t2">要比较的日期</param>
		/// <param name="compareType">比较类型</param>
		/// <returns>以 -1、0、1 表示的早于、等于或晚于当前指定的日期</returns>
		public static int CompareTo_(this DateTime t1, DateTime t2, DateTimeCompareType compareType = DateTimeCompareType.None)
		{
			if(compareType == DateTimeCompareType.None)
				return t1.CompareTo(t2);

			DateTime d1, d2;
			int y1 = t1.Year, y2 = t2.Year;
			switch(compareType)
			{
				case DateTimeCompareType.ByYear:
					d1 = new DateTime(y1, 1, 1);
					d2 = new DateTime(y2, 1, 1);
					break;
				case DateTimeCompareType.ByHalfYear:
					d1 = new DateTime(y1, t1.GetHalfYear_(), 1);
					d2 = new DateTime(y2, t2.GetHalfYear_(), 1);
					break;
				case DateTimeCompareType.ByQuarter:
					d1 = new DateTime(y1, t1.GetQuarter_(), 1);
					d2 = new DateTime(y2, t2.GetQuarter_(), 1);
					break;
				case DateTimeCompareType.ByMonth:
					d1 = new DateTime(y1, t1.Month, 1);
					d2 = new DateTime(y2, t2.Month, 1);
					break;
				case DateTimeCompareType.ByWeek:
					if(y1 != y2)
						return y1.CompareTo(y2);
					return t1.GetWeekOfYear_().CompareTo(t2.GetWeekOfYear_());
				case DateTimeCompareType.ByDay:
					return t1.Date.CompareTo(t2.Date);
				case DateTimeCompareType.ByHour:
					d1 = new DateTime(y1, t1.Month, t1.Day, t1.Hour, 0, 0);
					d2 = new DateTime(y2, t2.Month, t2.Day, t2.Hour, 0, 0);
					break;
				case DateTimeCompareType.ByMinute:
					d1 = new DateTime(y1, t1.Month, t1.Day, t1.Hour, t1.Minute, 0);
					d2 = new DateTime(y2, t2.Month, t2.Day, t2.Hour, t2.Minute, 0);
					break;
				default:
					d1 = new DateTime(y1, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
					d2 = new DateTime(y2, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
					break;
			}

			return d1.CompareTo(d2);
		}

		/// <summary>以指定的比较类型比较指定日期是否早于当前指定的日期</summary>
		/// <param name="t1">当前指定的日期</param>
		/// <param name="t2">要比较的日期</param>
		/// <param name="compareType">比较类型</param>
		/// <returns>指示是否早于当前指定的日期</returns>
		public static bool LessThan_(this DateTime t1, DateTime t2, DateTimeCompareType compareType = DateTimeCompareType.None)
		{
			return t1.CompareTo_(t2, compareType) < 0;
		}

		/// <summary>以指定的比较类型比较指定日期是否等于当前指定的日期</summary>
		/// <param name="t1">当前指定的日期</param>
		/// <param name="t2">要比较的日期</param>
		/// <param name="compareType">比较类型</param>
		/// <returns>指示是否等于当前指定的日期</returns>
		public static bool Equals_(this DateTime t1, DateTime t2, DateTimeCompareType compareType = DateTimeCompareType.None)
		{
			return t1.CompareTo_(t2, compareType) == 0;
		}

		/// <summary>以指定的比较类型比较指定日期是否晚于当前指定的日期</summary>
		/// <param name="t1">当前指定的日期</param>
		/// <param name="t2">要比较的日期</param>
		/// <param name="compareType">比较类型</param>
		/// <returns>指示是否晚于当前指定的日期</returns>
		public static bool GreaterThan_(this DateTime t1, DateTime t2, DateTimeCompareType compareType = DateTimeCompareType.None)
		{
			return t1.CompareTo_(t2, compareType) > 0;
		}

		/// <summary>以指定的比较类型比较指定日期是否不早于当前指定的日期</summary>
		/// <param name="t1">当前指定的日期</param>
		/// <param name="t2">要比较的日期</param>
		/// <param name="compareType">比较类型</param>
		/// <returns>指示是否不早于当前指定的日期</returns>
		public static bool NotLessThan_(this DateTime t1, DateTime t2, DateTimeCompareType compareType = DateTimeCompareType.None)
		{
			return t1.CompareTo_(t2, compareType) >= 0;
		}

		/// <summary>以指定的比较类型比较指定日期是否不晚于当前指定的日期</summary>
		/// <param name="t1">当前指定的日期</param>
		/// <param name="t2">要比较的日期</param>
		/// <param name="compareType">比较类型</param>
		/// <returns>指示是否不晚于当前指定的日期</returns>
		public static bool NotGreaterThan_(this DateTime t1, DateTime t2, DateTimeCompareType compareType = DateTimeCompareType.None)
		{
			return t1.CompareTo_(t2, compareType) <= 0;
		}
		#endregion
	}
}