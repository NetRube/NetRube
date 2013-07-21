using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NetRube
{
	public static partial class Utils
	{
		// 扩展方式转换操作

		#region ToBool_
		/// <summary>将不为 '0' 的字符转换为 true</summary>
		/// <param name="c">要转换的字符，'0' 为 false，其余为 true</param>
		/// <returns>'0' 为 false，其余为 true</returns>
		public static bool ToBool_(this char c)
		{
			return !(c == '0' || c == '\0');
		}

		private static string[] FalseStrings = new string[] { "false", "no", "not", "null", "0" };
		/// <summary>将不为空值、null 值、"false"字符串的内容转换为 true</summary>
		/// <param name="str">要转换的字符串，空值、null 值、"false" 为 false，其余为 true</param>
		/// <returns>空值、null 值、"false" 为 false，其余为 true</returns>
		public static bool ToBool_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return false;
			if(str.In_(FalseStrings)) return false;
			if(str.IsNumber_()) return str.ToDouble_().ToBool_();
			return true;
		}

		/// <summary>将大于 0 的数字或不为空值、null 值、"false"字符串的内容转换为 true</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>大于 0 的数字或不为空值、null 值、"false"字符串的内容转换为 true</returns>
		public static bool ToBool_<T>(this T obj)
		{
			if(null == obj) return false;
			try { return Convert.ToBoolean(obj); }
			catch { return obj.ToString().ToBool_(); }
		}
		#endregion

		#region ToChar_
		/// <summary>转换为相应的字符</summary>
		/// <param name="b">要转换的布尔值，true 为 1，false 为 0</param>
		/// <returns>true 为 1，false 为 0</returns>
		public static char ToChar_(this bool b)
		{
			return b ? '1' : '0';
		}

		/// <summary>转换为相应的字符</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>字符串的第一个字符或默认值</returns>
		public static char ToChar_(this string str, char defval = '0')
		{
			return str.IsNullOrEmpty_() ? defval : str[0];
		}

		/// <summary>转换为相应的字符</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的字符或默认值</returns>
		public static char ToChar_<T>(this T obj, char defval = '0')
		{
			if(null == obj) return defval;
			try { return Convert.ToChar(obj); }
			catch { return defval; }
		}

		/// <summary>转换为相应的字符，转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的字符或 null</returns>
		public static char? ToCharOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_())
				return null;
			return str[0];
		}

		/// <summary>转换为相应的字符，转换不成功时返回 null</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的字符或 null</returns>
		public static char? ToCharOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToChar(obj); }
			catch { return null; }
		}
		#endregion

		#region ToByte_
		/// <summary>转换为相应的整数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static byte ToByte_(this string str, byte defval = 0)
		{
			if(str.IsNullOrEmpty_()) return defval;
			byte _retval;
			if(!byte.TryParse(str, NumberStyles.Any, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换为相应的整数</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static byte ToByte_<T>(this T obj, byte defval = 0)
		{
			if(null == obj) return defval;
			try { return Convert.ToByte(obj); }
			catch { return obj.ToString().ToByte_(defval); }
		}

		/// <summary>转换为相应的整数，转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的整数或 null</returns>
		public static byte? ToByteOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_()) return null;
			byte _retval;
			if(!byte.TryParse(str, NumberStyles.Any, null, out _retval))
				return null;
			return _retval;
		}

		/// <summary>转换为相应的整数，转换不成功时返回 null</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的整数或 null</returns>
		public static byte? ToByteOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToByte(obj); }
			catch { return obj.ToString().ToByteOrNull_(); }
		}
		#endregion

		#region ToInt_
		/// <summary>转换为相应的整数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static int ToInt_(this string str, int defval = 0)
		{
			if(str.IsNullOrEmpty_()) return defval;
			int _retval;
			if(!int.TryParse(str, NumberStyles.Any, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换为相应的整数</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static int ToInt_<T>(this T obj, int defval = 0)
		{
			if(null == obj) return defval;
			try { return Convert.ToInt32(obj); }
			catch { return obj.ToString().ToInt_(defval); }
		}

		/// <summary>转换为相应的整数，转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的整数，转换不成功时返回 null</returns>
		public static int? ToIntOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_()) return null;
			int _retval;
			if(!int.TryParse(str, NumberStyles.Any, null, out _retval))
				return null;
			return _retval;
		}

		/// <summary>转换为相应的整数或 null</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">要转换的字符串</param>
		/// <returns>相应的整数或 null</returns>
		public static int? ToIntOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToInt32(obj); }
			catch { return obj.ToString().ToIntOrNull_(); }
		}
		#endregion

		#region ToShort_
		/// <summary>转换为相应的整数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static short ToShort_(this string str, short defval = 0)
		{
			if(str.IsNullOrEmpty_()) return defval;
			short _retval;
			if(!short.TryParse(str, NumberStyles.Any, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换为相应的整数</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static short ToShort_<T>(this T obj, short defval = 0)
		{
			if(null == obj) return defval;
			try { return Convert.ToInt16(obj); }
			catch { return obj.ToString().ToShort_(defval); }
		}

		/// <summary>转换为相应的整数，转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的整数或 null</returns>
		public static short? ToShortOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_()) return null;
			short _retval;
			if(!short.TryParse(str, NumberStyles.Any, null, out _retval))
				return null;
			return _retval;
		}

		/// <summary>转换为相应的整数，转换不成功时返回 null</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的整数或 null</returns>
		public static short? ToShortOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToInt16(obj); }
			catch { return obj.ToString().ToShortOrNull_(); }
		}
		#endregion

		#region ToLong_
		/// <summary>转换为相应的整数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static long ToLong_(this string str, long defval = 0L)
		{
			if(str.IsNullOrEmpty_()) return defval;
			long _retval;
			if(!long.TryParse(str, NumberStyles.Any, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换为相应的整数</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static long ToLong_<T>(this T obj, long defval = 0L)
		{
			if(null == obj) return defval;
			try { return Convert.ToInt64(obj); }
			catch { return obj.ToString().ToLong_(defval); }
		}

		/// <summary>转换为相应的整数，转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的整数或 null</returns>
		public static long? ToLongOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_()) return null;
			long _retval;
			if(!long.TryParse(str, NumberStyles.Any, null, out _retval))
				return null;
			return _retval;
		}

		/// <summary>转换为相应的整数，转换不成功时返回 null</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的整数或 null</returns>
		public static long? ToLongOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToInt64(obj); }
			catch { return obj.ToString().ToLongOrNull_(); }
		}
		#endregion

		#region ToFloat_
		/// <summary>转换为相应的单精度浮点数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的单精度浮点数或默认值</returns>
		public static float ToFloat_(this string str, float defval = 0F)
		{
			if(str.IsNullOrEmpty_()) return defval;
			float _retval;
			if(!float.TryParse(str, NumberStyles.Any, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换为相应的单精度浮点数</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的单精度浮点数或默认值</returns>
		public static float ToFloat_<T>(this T obj, float defval = 0F)
		{
			if(null == obj) return defval;
			try { return Convert.ToSingle(obj); }
			catch { return obj.ToString().ToFloat_(defval); }
		}

		/// <summary>转换为相应的单精度浮点数，转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的单精度浮点数或 null</returns>
		public static float? ToFloatOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_()) return null;
			float _retval;
			if(!float.TryParse(str, NumberStyles.Any, null, out _retval))
				return null;
			return _retval;
		}

		/// <summary>转换为相应的单精度浮点数，转换不成功时返回 null</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的单精度浮点数或 null</returns>
		public static float? ToFloatOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToSingle(obj); }
			catch { return obj.ToString().ToFloatOrNull_(); }
		}
		#endregion

		#region ToDouble_
		/// <summary>转换为相应的双精度浮点数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的双精度浮点数或默认值</returns>
		public static double ToDouble_(this string str, double defval = 0D)
		{
			if(str.IsNullOrEmpty_()) return defval;
			double _retval;
			if(!double.TryParse(str, NumberStyles.Any, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换为相应的双精度浮点数</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的双精度浮点数或默认值</returns>
		public static double ToDouble_<T>(this T obj, double defval = 0D)
		{
			if(null == obj) return defval;
			try { return Convert.ToDouble(obj); }
			catch { return obj.ToString().ToDouble_(defval); }
		}

		/// <summary>转换为相应的双精度浮点数，转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的双精度浮点数或 null</returns>
		public static double? ToDoubleOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_()) return null;
			double _retval;
			if(!double.TryParse(str, NumberStyles.Any, null, out _retval))
				return null;
			return _retval;
		}

		/// <summary>转换为相应的双精度浮点数，转换不成功时返回 null</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的双精度浮点数或 null</returns>
		public static double? ToDoubleOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToDouble(obj); }
			catch { return obj.ToString().ToDoubleOrNull_(); }
		}
		#endregion

		#region ToDecimal_
		/// <summary>转换高精度的十进制数（一般用于货币）</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的十进制数或默认值</returns>
		public static decimal ToDecimal_(this string str, decimal defval = 0M)
		{
			if(str.IsNullOrEmpty_()) return defval;
			decimal _retval;
			if(!decimal.TryParse(str, NumberStyles.Any, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换高精度的十进制数（一般用于货币）</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的十进制数或默认值</returns>
		public static decimal ToDecimal_<T>(this T obj, decimal defval = 0M)
		{
			if(null == obj) return defval;
			try { return Convert.ToDecimal(obj); }
			catch { return obj.ToString().ToDecimal_(defval); }
		}

		/// <summary>转换高精度的十进制数（一般用于货币），转换不成功时返回 null</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的十进制数或 null</returns>
		public static decimal? ToDecimalOrNull_(this string str)
		{
			if(str.IsNullOrEmpty_()) return null;
			decimal _retval;
			if(!decimal.TryParse(str, NumberStyles.Any, null, out _retval))
				return null;
			return _retval;
		}

		/// <summary>转换高精度的十进制数（一般用于货币），转换不成功时返回 null</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的十进制数或 null</returns>
		public static decimal? ToDecimalOrNull_<T>(this T obj)
		{
			if(null == obj) return null;
			try { return Convert.ToDecimal(obj); }
			catch { return obj.ToString().ToDecimalOrNull_(); }
		}
		#endregion

		#region ToEnum_
		/// <summary>转换为相应的枚举值</summary>
		/// <typeparam name="T">枚举类型</typeparam>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的枚举值或默认值</returns>
		public static T ToEnum_<T>(this string str, T defval = default(T)) where T : struct
		{
			if(str.IsNullOrEmpty_()) return defval;
			Type _type = typeof(T);
			if(!_type.IsEnum) return defval;
			T _retval;
			if(Enum.TryParse(str, true, out _retval))
				if(Enum.IsDefined(_type, _retval))
					return _retval;
			return defval;
		}

		/// <summary>转换为相应的枚举值</summary>
		/// <typeparam name="T">枚举类型</typeparam>
		/// <param name="num">要转换的数字</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的枚举值或默认值</returns>
		public static T ToEnum_<T>(this object num, T defval = default(T)) where T : struct
		{
			Type _type = typeof(T);
			if(!_type.IsEnum) return defval;
			try
			{
				T _retval = (T)Enum.ToObject(_type, num);
				if(Enum.IsDefined(_type, _retval))
					return _retval;
				return defval;
			}
			catch { return num.ToString().ToEnum_(defval); }
		}

		/// <summary>转换为相应的枚举值</summary>
		/// <typeparam name="T">枚举类型</typeparam>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的枚举值或 null</returns>
		public static T? ToEnumOrNull_<T>(this string str) where T : struct
		{
			if(str.IsNullOrEmpty_()) return null;
			Type _type = typeof(T);
			if(!_type.IsEnum) return null;
			T _retval;
			if(Enum.TryParse(str, true, out _retval))
				if(Enum.IsDefined(_type, _retval))
					return _retval;
			return null;
		}

		/// <summary>转换为相应的枚举值</summary>
		/// <typeparam name="T">枚举类型</typeparam>
		/// <param name="num">要转换的数字</param>
		/// <returns>相应的枚举值或 null</returns>
		public static T? ToEnumOrNull_<T>(this object num) where T : struct
		{
			Type _type = typeof(T);
			if(!_type.IsEnum) return null;
			try
			{
				T _retval = (T)Enum.ToObject(_type, num);
				if(Enum.IsDefined(_type, _retval))
					return _retval;
				return null;
			}
			catch { return num.ToString().ToEnumOrNull_<T>(); }
		}
		#endregion

		#region ToString_
		/// <summary>转换为相应的枚举值名称</summary>
		/// <param name="e">要转换的枚举值</param>
		/// <returns>枚举值名称</returns>
		public static string ToString_(this Enum e)
		{
			return null == e ? string.Empty : Enum.GetName(e.GetType(), e);
		}

		/// <summary>转换为不含分隔符的 GUID 字符串</summary>
		/// <param name="g">要转换的 GUID</param>
		/// <returns>不含分隔符的 GUID 字符串</returns>
		public static string ToString_(this Guid g)
		{
			if(g == null) g = Guid.Empty;
			return g.ToString("N");
		}

		/// <summary>将 true 转换为 "1"，false 转换为 "0"</summary>
		/// <param name="b">要转换的布尔值，true 为 "1"，false 为 "0"</param>
		/// <returns>true 为 "1"，false 为 "0"</returns>
		public static string ToString_(this bool b)
		{
			return b ? "1" : "0";
		}

		/// <summary>将 true 转换为 "1"，false 转换为 "0"</summary>
		/// <param name="b">要转换的布尔值，true 为 "1"，false 为 "0"，为 null 时返回空字符串</param>
		/// <returns>true 为 "1"，false 为 "0"，为 null 时返回空字符串</returns>
		public static string ToString_(this bool? b)
		{
			return null == b ? string.Empty : b.Value.ToString_();
		}

		/// <summary>转换为 yyyy-MM-dd HH:mm:ss 格式的字符串</summary>
		/// <param name="time">要转换的日期</param>
		/// <returns>yyyy-MM-dd HH:mm:ss 格式的字符串</returns>
		public static string ToString_(this DateTime time)
		{
			return time.ToString("yyyy-MM-dd HH:mm:ss");
		}

		/// <summary>转换为 yyyy-MM-dd HH:mm:ss 格式的字符串</summary>
		/// <param name="time">要转换的日期，为 null 时返回空字符串</param>
		/// <returns>yyyy-MM-dd HH:mm:ss 格式或空字符串</returns>
		public static string ToString_(this DateTime? time)
		{
			return null == time ? string.Empty : time.Value.ToString_();
		}

		/// <summary>转换成对应的字符串格式（x 天 xx 小时 xx 分 xx 秒 xxx 毫秒）</summary>
		/// <param name="ts">时间间隔</param>
		/// <returns>“x 天 xx 小时 xx 分 xx 秒 xxx 毫秒”格式的字符串</returns>
		public static string ToString_(this TimeSpan ts)
		{
			STR str = new STR(30);
			bool is0 = true;
			int temp;
			temp = ts.Days;
			if(temp != 0)
			{
				str.Append(temp).Append(Localization.Resources.TimeSpanDays);
				is0 = false;
			}
			temp = ts.Hours;
			if(temp != 0 || !is0)
			{
				str.Append(temp).Append(Localization.Resources.TimeSpanHours);
			}
			temp = ts.Minutes;
			if(temp != 0 || !is0)
			{
				str.Append(temp).Append(Localization.Resources.TimeSpanMinutes);
			}
			temp = ts.Seconds;
			if(temp != 0 || !is0)
			{
				str.Append(temp).Append(Localization.Resources.TimeSpanSeconds);
			}
			temp = ts.Milliseconds;
			if(temp != 0 || is0)
			{
				str.Append(temp).Append(Localization.Resources.TimeSpanMilliseconds);
			}

			return str.TrimEnd().ToString();
		}

		/// <summary>转换成对应的字符串格式（x 天 xx 小时 xx 分 xx 秒 xxx 毫秒）</summary>
		/// <param name="ts">时间间隔，为 null 时返回空字符串</param>
		/// <returns>“x 天 xx 小时 xx 分 xx 秒 xxx 毫秒”格式或空字符串</returns>
		public static string ToString_(this TimeSpan? ts)
		{
			return null == ts ? string.Empty : ts.Value.ToString_();
		}
		#endregion

		#region ToDate_
		/// <summary>转换为相应的日期</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认日期</param>
		/// <returns>相应的日期或默认值</returns>
		public static DateTime ToDate_(this string str, DateTime defval)
		{
			if(str.IsNullOrEmpty_()) return defval;
			DateTime _retval;
			if(!DateTime.TryParse(str, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>转换为相应的日期</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的日期</returns>
		public static DateTime ToDate_(this string str)
		{
			return ToDate_(str, BaseDateTime);
		}

		/// <summary>以指定格式转换为相应的日期</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认日期</param>
		/// <param name="format">日期格式（如：yyyy年MM月dd日 HH时mm分ss秒），如果格式中任一部分跟要转换的字符串不一致，都将导致转换失败而返回默认日期</param>
		/// <returns>相应的日期或默认值</returns>
		public static DateTime ToDate_(this string str, DateTime defval, string format)
		{
			if(str.IsNullOrEmpty_()) return defval;
			if(format.IsNullOrEmpty_()) return str.ToDate_(defval);
			DateTime _retval;
			if(!DateTime.TryParseExact(str, format, null, DateTimeStyles.None, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>以指定格式转换为相应的日期</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="format">日期格式（如：yyyy年MM月dd日 HH时mm分ss秒），如果格式中任一部分跟要转换的字符串不一致，都将导致转换失败而返回默认日期</param>
		/// <returns>相应的日期</returns>
		public static DateTime ToDate_(this string str, string format)
		{
			return ToDate_(str, BaseDateTime, format);
		}

		/// <summary>将以100纳（毫微）秒表示的日期转换为相应的日期</summary>
		/// <param name="num">要转换的数字</param>
		/// <param name="defval">转换不成功时的默认日期</param>
		/// <param name="kind">指示此数字是指定了本地时间、协调世界时 (UTC)，还是两者皆未指定。</param>
		/// <returns>相应的日期或默认值</returns>
		public static DateTime ToDate_(this long num, DateTime defval, DateTimeKind kind = DateTimeKind.Local)
		{
			try { return new DateTime(num, kind); }
			catch { return defval; }
		}

		/// <summary>将以100纳（毫微）秒表示的日期转换为相应的日期</summary>
		/// <param name="num">要转换的数字</param>
		/// <param name="kind">指示此数字是指定了本地时间、协调世界时 (UTC)，还是两者皆未指定。</param>
		/// <returns>相应的日期或默认值</returns>
		public static DateTime ToDate_(this long num, DateTimeKind kind = DateTimeKind.Local)
		{
			return ToDate_(num, BaseDateTime, kind);
		}

		/// <summary>转换为相应的日期</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="defval">转换不成功时的默认日期</param>
		/// <param name="format">如果 obj 为字符串时的日期格式（如：yyyy年MM月dd日 HH时mm分ss秒），如果格式中任一部分跟要转换的字符串不一致，都将导致转换失败而返回默认日期</param>
		/// <returns>相应的日期或默认值</returns>
		public static DateTime ToDate_<T>(this T obj, DateTime defval, string format = null)
		{
			if(null == obj) return defval;
			if(obj is DateTime) return Convert.ToDateTime(obj);
			return obj.ToString().ToDate_(defval, format);
		}

		/// <summary>转换为相应的日期</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <param name="format">如果 obj 为字符串时的日期格式（如：yyyy年MM月dd日 HH时mm分ss秒），如果格式中任一部分跟要转换的字符串不一致，都将导致转换失败而返回默认日期</param>
		/// <returns>相应的日期</returns>
		public static DateTime ToDate_<T>(this T obj, string format = null)
		{
			return obj.ToDate_(BaseDateTime, format);
		}
		#endregion

		#region ToGuid_
		/// <summary>转换为相应的 GUID</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>相应的 GUID</returns>
		public static Guid ToGuid_(this string str)
		{
			Guid g = Guid.Empty;
			if(!str.IsNullOrEmpty_())
				Guid.TryParse(str, out g);
			return g;
		}

		/// <summary>转换为相应的 GUID</summary>
		/// <param name="guid">要转换的 GUID</param>
		/// <returns>相应的 GUID</returns>
		public static Guid ToGuid_(this Guid guid)
		{
			return guid;
		}

		/// <summary>转换为相应的 GUID</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要转换的对象</param>
		/// <returns>相应的 GUID</returns>
		public static Guid ToGuid_<T>(this T obj)
		{
			if(null == obj) return Guid.Empty;
			return obj.ToString().ToGuid_();
		}
		#endregion

		#region ToIEnumerable
		/// <summary>将 Enum 转换为 <see cref="Dictionary&lt;int, string&gt;" /> 集合</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <returns><see cref="Dictionary&lt;int, string&gt;" /> 集合</returns>
		public static Dictionary<int, string> ToDict_<T>() where T : struct
		{
			Type type = typeof(T);
			if(!type.IsEnum) return null;

			var vals = Enum.GetValues(type);
			if(vals.IsNullOrEmpty_()) return null;
			var dict = new Dictionary<int, string>(vals.Length);
			FieldInfo fi;
			int key;
			string val;
			foreach(var item in vals)
			{
				key = Convert.ChangeType(item, Enum.GetUnderlyingType(type)).ToInt_();
				val = item.ToString();
				fi = type.GetField(val);
				var att = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
				if(att != null)
					val = ((DescriptionAttribute)att).Description;
				dict.Add(key, val);
			}
			return dict;
		}

		/// <summary>将 Enum 转换为 <see cref="NameValueCollection" /> 集合</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <returns><see cref="NameValueCollection" /> 集合</returns>
		public static NameValueCollection ToNVC_<T>() where T : struct
		{
			Type type = typeof(T);
			if(!type.IsEnum) return null;

			var vals = Enum.GetValues(type);
			if(vals.IsNullOrEmpty_()) return null;
			var nvc = new NameValueCollection(vals.Length);
			FieldInfo fi;
			string key;
			string val;
			foreach(var item in vals)
			{
				key = Convert.ChangeType(item, Enum.GetUnderlyingType(type)).ToString();
				val = item.ToString();
				fi = type.GetField(val);
				var att = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
				if(att != null)
					val = ((DescriptionAttribute)att).Description;
				nvc.Add(key, val);
			}
			return nvc;
		}

		/// <summary>
		/// 将 Enum 转换为 <see cref="List&lt;KeyValuePair&lt;int, string&gt;&gt;" /> 集合
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <returns><see cref="List&lt;KeyValuePair&lt;int, string&gt;&gt;" /> 集合</returns>
		public static List<KeyValuePair<int, string>> ToKVL_<T>() where T : struct
		{
			Type type = typeof(T);
			if(!type.IsEnum) return null;

			var vals = Enum.GetValues(type);
			if(vals.IsNullOrEmpty_()) return null;
			var kvl = new List<KeyValuePair<int, string>>(vals.Length);
			FieldInfo fi;
			int key;
			string val;
			foreach(var item in vals)
			{
				key = Convert.ChangeType(item, Enum.GetUnderlyingType(type)).ToInt_();
				val = item.ToString();
				fi = type.GetField(val);
				var att = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
				if(att != null)
					val = ((DescriptionAttribute)att).Description;
				kvl.Add(new KeyValuePair<int, string>(key, val));
			}
			return kvl;
		}

		/// <summary>将 Enum 转换为 <see cref="Dictionary&lt;int, string&gt;" /> 集合</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="res">所在的本地化资源</param>
		/// <param name="info">区域性信息</param>
		/// <returns><see cref="Dictionary&lt;int, string&gt;" /> 集合</returns>
		public static Dictionary<int, string> ToDict_<T>(System.Resources.ResourceManager res, System.Globalization.CultureInfo info = null) where T : struct
		{
			Type type = typeof(T);
			if(!type.IsEnum) return null;

			var vals = Enum.GetValues(type);
			if(vals.IsNullOrEmpty_()) return null;
			var dict = new Dictionary<int, string>(vals.Length);
			int key;
			string val, name;
			foreach(var item in vals)
			{
				key = Convert.ChangeType(item, Enum.GetUnderlyingType(type)).ToInt_();
				name = item.ToString();
				val = GetLocalization_(type.Name + "_" + name, res, info);
				if(val.IsNullOrEmpty_())
					val = name;
				dict.Add(key, val);
			}
			return dict;
		}

		/// <summary>将 Enum 转换为 <see cref="NameValueCollection" /> 集合</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="res">所在的本地化资源</param>
		/// <param name="info">区域性信息</param>
		/// <returns><see cref="NameValueCollection" /> 集合</returns>
		public static NameValueCollection ToNVC_<T>(System.Resources.ResourceManager res, System.Globalization.CultureInfo info = null) where T : struct
		{
			Type type = typeof(T);
			if(!type.IsEnum) return null;

			var vals = Enum.GetValues(type);
			if(vals.IsNullOrEmpty_()) return null;
			var nvc = new NameValueCollection(vals.Length);
			string key, val, name;
			foreach(var item in vals)
			{
				key = Convert.ChangeType(item, Enum.GetUnderlyingType(type)).ToString();
				name = item.ToString();
				val = GetLocalization_(type.Name + "_" + name, res, info);
				if(val.IsNullOrEmpty_())
					val = name;
				nvc.Add(key, val);
			}
			return nvc;
		}

		/// <summary>
		/// 将 Enum 转换为 <see cref="List&lt;KeyValuePair&lt;int, string&gt;&gt;" /> 集合
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="res">所在的本地化资源</param>
		/// <param name="info">区域性信息</param>
		/// <returns><see cref="List&lt;KeyValuePair&lt;int, string&gt;&gt;" /> 集合</returns>
		public static List<KeyValuePair<int, string>> ToKVL_<T>(System.Resources.ResourceManager res, System.Globalization.CultureInfo info = null) where T : struct
		{
			Type type = typeof(T);
			if(!type.IsEnum) return null;

			var vals = Enum.GetValues(type);
			if(vals.IsNullOrEmpty_()) return null;
			var kvl = new List<KeyValuePair<int, string>>(vals.Length);
			int key;
			string val, name;
			foreach(var item in vals)
			{
				key = Convert.ChangeType(item, Enum.GetUnderlyingType(type)).ToInt_();
				name = item.ToString();
				val = GetLocalization_(type.Name + "_" + name, res, info);
				if(val.IsNullOrEmpty_())
					val = name;
				kvl.Add(new KeyValuePair<int, string>(key, val));
			}
			return kvl;
		}
		#endregion

		#region ToStrArray_
		/// <summary>转换成字符串列表</summary>
		/// <param name="list">要转换的列表</param>
		/// <param name="ignoreEmpty">是否忽略掉空格</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <returns>字符串列表</returns>
		public static string[] ToStrArray_(this string[] list, bool ignoreEmpty = false, bool distinct = false)
		{
			if(list.IsNull_()) return EmptyArray<string>();
			if(!ignoreEmpty && !distinct) return list;
			return list.ToStrArray_<IEnumerable<string>>(ignoreEmpty, distinct);
		}

		/// <summary>转换成字符串列表</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <param name="ignoreEmpty">是否忽略掉空格</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <returns>字符串列表</returns>
		public static string[] ToStrArray_<T>(this IEnumerable<string> list, bool ignoreEmpty = false, bool distinct = false)
		{
			if(list.IsNull_()) return EmptyArray<string>();

			IEnumerable<string> array;
			if(distinct)
				array = __ToStrArrayDistinct(list, ignoreEmpty);
			else if(ignoreEmpty)
				array = __ToStrArray(list, ignoreEmpty);
			else
				array = list;
			return array.ToArray();
		}

		/// <summary>转换成字符串列表</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <param name="ignoreEmpty">是否忽略掉空格</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <returns>字符串列表</returns>
		public static string[] ToStrArray_<T>(this IEnumerable<T> list, bool ignoreEmpty = false, bool distinct = false)
		{
			if(list.IsNull_()) return EmptyArray<string>();

			IEnumerable<string> array;
			if(distinct)
				array = __ToStrArrayDistinct(list, ignoreEmpty);
			else
				array = __ToStrArray(list, ignoreEmpty);
			return array.ToArray();
		}

		private static IEnumerable<string> __ToStrArray(IEnumerable<string> list, bool ignoreEmpty)
		{
			foreach(string str in list)
			{
				if(ignoreEmpty && str.Trim().IsNullOrEmpty_()) continue;
				yield return str;
			}
		}
		private static IEnumerable<string> __ToStrArrayDistinct(IEnumerable<string> list, bool ignoreEmpty)
		{
			HashSet<string> hs = new HashSet<string>();
			foreach(string str in list)
			{
				if((ignoreEmpty && str.Trim().IsNullOrEmpty_()) || hs.Contains(str)) continue;
				hs.Add(str);
				yield return str;
			}
		}
		private static IEnumerable<string> __ToStrArray<T>(IEnumerable<T> list, bool ignoreEmpty)
		{
			string str;
			foreach(T item in list)
			{
				if(item == null) continue;
				str = item.ToString();
				if(ignoreEmpty && str.Trim().IsNullOrEmpty_()) continue;
				yield return str;
			}
		}
		private static IEnumerable<string> __ToStrArrayDistinct<T>(IEnumerable<T> list, bool ignoreEmpty)
		{
			string str;
			HashSet<string> hs = new HashSet<string>();
			foreach(T item in list)
			{
				if(item == null) continue;
				str = item.ToString();
				if((ignoreEmpty && str.Trim().IsNullOrEmpty_()) || hs.Contains(str)) continue;
				hs.Add(str);
				yield return str;
			}
		}
		#endregion

		#region StrToGuidArray
		/// <summary>将包含 Guid 字符串列表转换成 Guid 列表</summary>
		/// <param name="strList">要转换的字符串列表</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <returns>Guid 列表</returns>
		public static Guid[] ToGuidArray_(this IEnumerable<string> strList, bool distinct = false)
		{
			if(strList.IsNullOrEmpty_()) return EmptyArray<Guid>();
			IEnumerable<Guid> array = distinct ? __ToGuidArrayDistinct(strList) : __ToGuidArray(strList);
			return array.ToArray();
		}

		private static IEnumerable<Guid> __ToGuidArray(IEnumerable<string> strList)
		{
			Guid guid;
			foreach(string str in strList)
			{
				if(str.IsNullOrEmpty_()) continue;
				guid = str.ToGuid_();
				if(guid == Guid.Empty) continue;
				yield return guid;
			}
		}
		private static IEnumerable<Guid> __ToGuidArrayDistinct(IEnumerable<string> strList)
		{
			Guid guid;
			HashSet<Guid> list = new HashSet<Guid>();
			foreach(string str in strList)
			{
				if(str.IsNullOrEmpty_()) continue;
				guid = str.ToGuid_();
				if(guid == Guid.Empty || list.Contains(guid)) continue;
				list.Add(guid);
				yield return guid;
			}
		}
		#endregion

		#region StrToIntArray
		/// <summary>将包含数字字符串列表转换成数字列表</summary>
		/// <param name="strList">要转换的字符串列表</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <returns>数字列表</returns>
		public static int[] ToIntArray_(this IEnumerable<string> strList, bool distinct = false)
		{
			if(strList.IsNullOrEmpty_()) return EmptyArray<int>();
			IEnumerable<int> array = distinct ? __ToIntArrayDistinct(strList) : __ToIntArray(strList);
			return array.ToArray();
		}

		private static IEnumerable<int> __ToIntArray(IEnumerable<string> strList)
		{
			int num;
			foreach(string str in strList)
			{
				if(str.IsNullOrEmpty_() || !int.TryParse(str.Trim(), out num)) continue;
				yield return num;
			}
		}
		private static IEnumerable<int> __ToIntArrayDistinct(IEnumerable<string> strList)
		{
			int num;
			HashSet<int> list = new HashSet<int>();
			foreach(string str in strList)
			{
				if(str.IsNullOrEmpty_() || !int.TryParse(str, out num) || list.Contains(num)) continue;
				list.Add(num);
				yield return num;
			}
		}
		#endregion

		#region ToArray
		/// <summary>将列表转换成另外一种数据类型列表</summary>
		/// <typeparam name="TFrom">源数据类型</typeparam>
		/// <typeparam name="TTo">目标数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <param name="func">自定义转换函数</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <param name="ec">过滤重复项要用的相等比较器</param>
		/// <returns>转换后的列表</returns>
		public static TTo[] ToArray_<TFrom, TTo>(this IEnumerable<TFrom> list, Func<TFrom, TTo> func, bool distinct = false, IEqualityComparer<TTo> ec = null)
		{
			return list.To_(func, distinct, ec).ToArray();
		}
		/// <summary>将列表转换成另外一种数据类型列表</summary>
		/// <typeparam name="TFrom">源数据类型</typeparam>
		/// <typeparam name="TTo">目标数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <param name="func">自定义转换函数</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <param name="ec">过滤重复项要用的相等比较器</param>
		/// <returns>转换后的列表</returns>
		public static IEnumerable<TTo> To_<TFrom, TTo>(this IEnumerable<TFrom> list, Func<TFrom, TTo> func, bool distinct = false, IEqualityComparer<TTo> ec = null)
		{
			if(list.IsNullOrEmpty_()) return EmptyArray<TTo>();
			return distinct ? __ToArrayDistinct(list, func, ec) : __ToArray(list, func);
		}
		private static IEnumerable<TTo> __ToArray<TFrom, TTo>(IEnumerable<TFrom> list, Func<TFrom, TTo> func)
		{
			foreach(var item in list)
			{
				yield return func(item);
			}
		}
		private static IEnumerable<TTo> __ToArrayDistinct<TFrom, TTo>(IEnumerable<TFrom> list, Func<TFrom, TTo> func, IEqualityComparer<TTo> ec)
		{
			HashSet<TTo> hs = new HashSet<TTo>(ec);
			foreach(var item in list)
			{
				TTo obj = func(item);
				if(hs.Contains(obj)) continue;
				hs.Add(obj);
				yield return obj;
			}
		}

		/// <summary>将列表转换成另外一种数据类型列表</summary>
		/// <typeparam name="TTo">目标数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <param name="func">自定义转换函数</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <param name="ec">过滤重复项要用的相等比较器</param>
		/// <returns>转换后的列表</returns>
		public static TTo[] ToArray_<TTo>(this IEnumerable list, Func<object, TTo> func, bool distinct = false, IEqualityComparer<TTo> ec = null)
		{
			return list.To_(func, distinct, ec).ToArray();
		}
		/// <summary>将列表转换成另外一种数据类型列表</summary>
		/// <typeparam name="TTo">目标数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <param name="func">自定义转换函数</param>
		/// <param name="distinct">是否过滤重复项</param>
		/// <param name="ec">过滤重复项要用的相等比较器</param>
		/// <returns>转换后的列表</returns>
		public static IEnumerable<TTo> To_<TTo>(this IEnumerable list, Func<object, TTo> func, bool distinct = false, IEqualityComparer<TTo> ec = null)
		{
			if(list.IsNullOrEmpty_()) return EmptyArray<TTo>();
			return distinct ? __ToArrayDistinct(list, func, ec) : __ToArray(list, func);
		}
		private static IEnumerable<TTo> __ToArray<TTo>(IEnumerable list, Func<object, TTo> func)
		{
			foreach(var item in list)
			{
				yield return func(item);
			}
		}
		private static IEnumerable<TTo> __ToArrayDistinct<TTo>(IEnumerable list, Func<object, TTo> func, IEqualityComparer<TTo> ec)
		{
			HashSet<TTo> hs = new HashSet<TTo>(ec);
			foreach(var item in list)
			{
				TTo obj = func(item);
				if(hs.Contains(obj)) continue;
				hs.Add(obj);
				yield return obj;
			}
		}
		#endregion

		#region ToJSON
		/// <summary>将对象转换成 JSON 字符串</summary>
		/// <param name="obj">要转换的对象</param>
		/// <returns>转换后的 JSON 字符串</returns>
		public static string ToJson_(this object obj)
		{
			return new FastJson.Json().ToJson(obj);
		}

		/// <summary>将 JSON 字符串转换成对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="json">要转换的 JSON 字符串</param>
		/// <returns>转换后的对象</returns>
		public static T ToObject_<T>(this string json)
		{
			return new FastJson.Json().ToObject<T>(json);
		}

		/// <summary>将 JSON 字符串转换成对象</summary>
		/// <param name="json">要转换的 JSON 字符串</param>
		/// <param name="type">对象类型</param>
		/// <returns>转换后的对象</returns>
		public static object ToObject_(this string json, Type type)
		{
			return new FastJson.Json().ToObject(json, type);
		}
		#endregion

		#region HexTo
		/// <summary>将十六进制字符串转换为相应的整数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static byte HexToByte(string str, byte defval = 0)
		{
			if(str.IsNullOrEmpty_()) return defval;
			byte _retval;
			if(!byte.TryParse(str, NumberStyles.HexNumber, null, out _retval))
				return defval;
			return _retval;
		}

		/// <summary>将十六进制字符串转换为相应的整数</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="defval">转换不成功时的默认值</param>
		/// <returns>相应的整数或默认值</returns>
		public static int HexToInt(string str, int defval = 0)
		{
			if(str.IsNullOrEmpty_()) return defval;
			int _retval;
			if(!int.TryParse(str, NumberStyles.HexNumber, null, out _retval))
				return defval;
			return _retval;
		}
		#endregion

		#region ToHex_
		/// <summary>转换成十六进制代码</summary>
		/// <param name="data">要转换的数据</param>
		/// <returns>十六进制代码</returns>
		public static string ToHex_(this byte[] data)
		{
			if(data.IsNullOrEmpty_()) return string.Empty;
			STR str = new STR(data.Length * 2);
			foreach(byte b in data)
				str += b.ToString("X2");
			return str.ToString();
		}

		/// <summary>转换成十六进制代码</summary>
		/// <param name="data">要转换的数据</param>
		/// <param name="separator">各字节间的分隔符</param>
		/// <returns>十六进制代码</returns>
		public static string ToHex_(this byte[] data, string separator)
		{
			if(data.IsNullOrEmpty_()) return string.Empty;
			if(separator.IsNullOrEmpty_()) return data.ToHex_();

			int i = 0, l = data.Length;
			STR str = new STR(l * 3);
			foreach(byte b in data)
			{
				str += b.ToString("X2");
				i++;
				if(i < l) str += separator;
			}
			return str.ToString();
		}
		#endregion

		#region ToStartAndEnd
		/// <summary>将列表转换成开始和结束类型</summary>
		/// <typeparam name="T">开始和结束的数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <returns>开始和结束类型</returns>
		public static StartAndEnd<T> ToStartAndEnd_<T>(this T[] list)
		{
			if(list.IsNullOrEmpty_()) return null;
			var se = new StartAndEnd<T>();
			se.Start = list[0];
			if(list.Length > 1)
				se.End = list[1];
			else
				se.End = se.Start;
			return se;
		}

		/// <summary>将列表转换成开始和结束类型</summary>
		/// <typeparam name="T">开始和结束的数据类型</typeparam>
		/// <param name="list">要转换的列表</param>
		/// <returns>开始和结束类型</returns>
		public static StartAndEnd<T> ToStartAndEnd_<T>(this IEnumerable<T> list)
		{
			if(null == list) return null;
			var se = new StartAndEnd<T>();
			var len = 0;
			foreach(var item in list)
			{
				if(len == 0)
				{
					se.Start = item;
					se.End = item;
				}
				else if(len == 1)
				{
					se.End = item;
					break;
				}
				len++;
			}

			return len == 0 ? null : se;
		}

		/// <summary>将字符串分离后转换成开始和结束类型</summary>
		/// <typeparam name="T">开始和结束的数据类型</typeparam>
		/// <param name="str">要转换的字符串</param>
		/// <param name="func">转换函数</param>
		/// <param name="separator">分离分隔符</param>
		/// <returns>开始和结束类型</returns>
		public static StartAndEnd<T> ToStartAndEnd_<T>(this string str, Func<string, T> func, char separator = ',')
		{
			if(str.IsNullOrEmpty_()) return null;
			var se = new StartAndEnd<T>();
			var list = str.Split(separator);
			se.Start = func(list[0]);
			if(list.Length > 1)
				se.End = func(list[1]);
			else
				se.End = se.Start;
			return se;
		}
		#endregion

		#region 转换
		/// <summary>转换数据类型</summary>
		/// <param name="value">要转换的值</param>
		/// <param name="type">要转换的类型</param>
		/// <returns>转换后的数据</returns>
		public static object To_(this object value, Type type)
		{
			if(value.IsNullOrDBNull_()) return null;

			if(type.IsStruct_())
				return XmlSerialization.Deserialize(value.ToString(), type);

			Type t = value.GetType();
			if(t == type) return value;

			if(type.IsEnum)
			{
				try { return Enum.Parse(type, value.ToString(), true); }
				catch { return Enum.ToObject(type, value); }
			}

			if(type.IsGenericType) type = type.GetGenericArguments()[0];
			if(type == typeof(Guid))
				return value.ToGuid_();

			TypeCode tc = Type.GetTypeCode(type);
			switch(tc)
			{
				case TypeCode.Boolean:
					return value.ToBool_();
				case TypeCode.Byte:
				case TypeCode.SByte:
					return value.ToByte_();
				case TypeCode.Char:
					return value.ToChar_();
				case TypeCode.DateTime:
					return value.ToDate_();
				case TypeCode.Decimal:
					return value.ToDecimal_();
				case TypeCode.Double:
					return value.ToDouble_();
				case TypeCode.Int16:
				case TypeCode.UInt16:
					return value.ToShort_();
				case TypeCode.Int32:
				case TypeCode.UInt32:
					return value.ToInt_();
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return value.ToLong_();
				case TypeCode.Single:
					return value.ToFloat_();
				case TypeCode.String:
					return value.ToString();
			}
			return Convert.ChangeType(value, type);
		}

		/// <summary>转换数据类型</summary>
		/// <typeparam name="TFrom">原数据类型</typeparam>
		/// <typeparam name="TTo">目标数据类型</typeparam>
		/// <param name="from">要转换的值</param>
		/// <returns>转换后的数据</returns>
		public static TTo To_<TFrom, TTo>(this TFrom from)
		{
			return (TTo)from.To_(typeof(TTo));
		}

		/// <summary>转换数据类型</summary>
		/// <typeparam name="TFrom">原数据类型</typeparam>
		/// <typeparam name="TTo">目标数据类型</typeparam>
		/// <param name="from">要转换的值</param>
		/// <param name="func">自定义转换函数</param>
		/// <returns>转换后的数据</returns>
		public static TTo To_<TFrom, TTo>(this TFrom from, Func<TFrom, TTo> func)
		{
			return func(from);
		}
		#endregion
	}
}