using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetRube
{
	public static partial class Utils
	{
		// 检测验证操作

		#region Contains_
		/// <summary>验证集合中是否包含特定值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="array">要验证的集合</param>
		/// <param name="value">用于验证的值</param>
		/// <returns>指示集合中是否包含特定值</returns>
		public static bool Contains_<T>(this IEnumerable<T> array, T value)
		{
			return value.In_(array);
		}

		/// <summary>验证数组中是否包含特定值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="array">要验证的数组</param>
		/// <param name="value">用于验证的值</param>
		/// <returns>指示数组中是否包含特定值</returns>
		public static bool Contains_<T>(this T[] array, T value)
		{
			return value.In_(array);
		}

		/// <summary>验证数组中是否包含特定字符串</summary>
		/// <param name="strArray">要比较的字符串数组</param>
		/// <param name="value">要比较的字符串</param>
		/// <param name="ignoreCase">是否区分大小写</param>
		/// <param name="full">是否完整匹配</param>
		/// <param name="atArray">非完整匹配时，true 为验证数组中的字符串是否包含要匹配的字符串，false 为验证要匹配的字符串是否包含数组里的字符串</param>
		/// <returns>指示数组中是否包含特定字符串</returns>
		public static bool Contains_(this string[] strArray, string value, bool ignoreCase = true, bool full = true, bool atArray = false)
		{
			return value.In_(ignoreCase, full, atArray, strArray);
		}
		#endregion

		#region In_
		/// <summary>验证对象是否存在于集合中</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要验证的对象</param>
		/// <param name="array">用于验证的集合</param>
		/// <returns>指示对象是否存在于集合中</returns>
		public static bool In_<T>(this T obj, IEnumerable<T> array)
		{
			if(null == obj || IsNullOrEmpty_(array)) return false;
			//ICollection<T> _array = array as ICollection<T>;
			//if(null != _array)
			//    return _array.Contains(obj);
			//else
			//{
			//    IEqualityComparer<T> _comparer = EqualityComparer<T>.Default;
			//    foreach(T _item in array)
			//        if(_comparer.Equals(_item, obj)) return true;
			//}
			//return false;
			return array.Contains(obj);
		}

		/// <summary>验证对象是否存在于集合中</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="obj">要验证的对象</param>
		/// <param name="array">用于验证的集合</param>
		/// <returns>指示对象是否存在于集合中</returns>
		public static bool In_<T>(this T obj, params T[] array)
		{
			return obj.In_(array as IEnumerable<T>);
		}

		/// <summary>验证字符串是否跟数组中的字符串匹配</summary>
		/// <param name="str">要比较的字符串</param>
		/// <param name="ignoreCase">是否区分大小写</param>
		/// <param name="full">是否完整匹配</param>
		/// <param name="atArray">非完整匹配时，true 为验证数组中的字符串是否包含要匹配的字符串，false 为验证要匹配的字符串是否包含数组里的字符串</param>
		/// <param name="strArray">要比较的字符串数组</param>
		/// <returns>指示字符串是否跟数组中的字符串匹配</returns>
		/// <remarks>
		/// 非完整匹配时，atArray 为 true 时验证数组中的字符串是否包含要匹配的字符串，为 false 时验证要匹配的字符串是否包含数组里的字符串
		/// </remarks>
		public static bool In_(this string str, bool ignoreCase = true, bool full = true, bool atArray = false, params string[] strArray)
		{
			if(string.IsNullOrEmpty(str) || IsNullOrEmpty_(strArray)) return false;

			StringComparison _sc = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			string _tmp;
			return strArray.Any(s =>
			{
				if(s.IsNullOrEmpty_()) return false;

				_tmp = s.Trim();

				if(full)
				{
					if(str.Equals(_tmp, _sc)) return true;
				}
				else
				{
					if(atArray)
					{
						if(_tmp.IndexOf(str, _sc) > -1) return true;
					}
					else
					{
						if(str.IndexOf(_tmp, _sc) > -1) return true;
					}
				}
				return false;
			});
		}

		/// <summary>验证字符串是否跟数组中的字符串匹配</summary>
		/// <param name="str">要比较的字符串</param>
		/// <param name="list">要比较的字符串列表</param>
		/// <param name="split">列表分隔符</param>
		/// <param name="ignoreCase">是否区分大小写</param>
		/// <returns>指示字符串是否跟数组中的字符串匹配</returns>
		/// <remarks>默认的 list 以半角逗号（,）拆分；默认忽略大小写。</remarks>
		public static bool In_(this string str, string list, string split = ",", bool ignoreCase = true)
		{
			return str.In_(ignoreCase, true, false, list.Split_(split));
		}
		#endregion

		#region IsNullOrEmpty_
		/// <summary>验证字符串是否为 null 或为空</summary>
		/// <param name="source">要验证的字符串</param>
		/// <returns>指示字符串是否为 null 或为空</returns>
		public static bool IsNullOrEmpty_(this string source)
		{
			return string.IsNullOrEmpty(source);
		}

		/// <summary>验证字符串是否为 null 或为空白字符串</summary>
		/// <param name="source">要验证的字符串</param>
		/// <returns>指示字符串是否为 null 或为空白字符串</returns>
		public static bool IsNullOrWhiteSpace_(this string source)
		{
			if(null == source) return true;
			for(int i = 0, len = source.Length; i < len; i++)
				if(!source[i].IsWhiteSpace_()) return false;
			return true;
		}

		/// <summary>验证对象是否为 null</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要验证的对象</param>
		/// <returns>指示对象是否为 null</returns>
		public static bool IsNull_<T>(this T source)
		{
			return null == source;
		}

		/// <summary>验证字符串是否为空</summary>
		/// <param name="source">要验证的字符串</param>
		/// <returns>指示字符串是否为空</returns>
		public static bool IsEmpty_(this string source)
		{
			return source.Length == 0;
		}

		/// <summary>验证 GUID 是否为空</summary>
		/// <param name="guid">要验证的 GUID</param>
		/// <returns>指示 GUID 是否为空</returns>
		public static bool IsEmpty_(this Guid guid)
		{
			return guid == Guid.Empty;
		}

		/// <summary>验证 GUID 是否为 null 或为空</summary>
		/// <param name="guid">要验证的 GUID</param>
		/// <returns>指示 GUID 是否为 null 或为空</returns>
		public static bool IsNullOrEmpty_(this Guid guid)
		{
			return guid == null || guid == Guid.Empty;
		}

		/// <summary>验证集合是否为 null 或为空</summary>
		/// <param name="source">要验证的集合</param>
		/// <returns>指示集合是否为 null 或为空</returns>
		public static bool IsNullOrEmpty_(this IEnumerable source)
		{
			if(null == source) return true;
			var _source = source as ICollection;
			if(null != _source) return _source.Count == 0;
			foreach(var s in source) return false;
			return false;
		}

		/// <summary>验证集合是否为 null 或为空</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要验证的集合</param>
		/// <returns>指示集合是否为 null 或为空</returns>
		public static bool IsNullOrEmpty_<T>(this IEnumerable<T> source)
		{
			if(null == source) return true;

			var _source = source as ICollection<T>;
			if(null != _source) return _source.Count == 0;
			var _source2 = source as ICollection;
			if(null != _source2) return _source2.Count == 0;
			foreach(T _item in source) return false;
			return true;
		}

		/// <summary>验证数组是否为 null 或为空</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要验证的数组</param>
		/// <returns>指示数组是否为 null 或为空</returns>
		public static bool IsNullOrEmpty_<T>(this T[] source)
		{
			return null == source || source.Length == 0;
		}

		/// <summary>验证对象是否为 null 或为空</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要验证的对象</param>
		/// <param name="defval">默认值</param>
		/// <returns>指示对象是否为 null 或为空</returns>
		public static bool IsNullOrEmpty_<T>(this StartAndEnd<T> source, T defval = default(T))
		{
			if(null == source) return true;
			return source.Start.Equals(defval) && source.End.Equals(defval);
		}

		/// <summary>验证对象是否为数据库字段 null 值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="value">要验证的对象</param>
		/// <returns>指示对象是否为数据库字段 null 值</returns>
		public static bool IsDBNull_<T>(this T value)
		{
			return Convert.IsDBNull(value);
		}

		/// <summary>验证对象是否为 null 或数据库字段 null 值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="value">要验证的对象</param>
		/// <returns>指示对象是否为 null 或数据库字段 null 值</returns>
		public static bool IsNullOrDBNull_<T>(this T value)
		{
			return null == value || Convert.IsDBNull(value);
		}
		#endregion

		#region 检测元素项是否是集合中的第一或最后项
		/// <summary>检测元素项是否是集合中的第一项</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="item">要检测的元素项</param>
		/// <param name="comparer">相等比较器</param>
		/// <returns>指示是否为集合中的第一项</returns>
		public static bool IsFirstItem_<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer = null)
		{
			if(null == source) return false;
			return comparer.Equals(source.FirstOrDefault(), item);
		}

		/// <summary>检测元素项是否是集合中的最后项</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="item">要检测的元素项</param>
		/// <param name="comparer">相等比较器</param>
		/// <returns>指示是否为集合中的最后项</returns>
		public static bool IsLastItem_<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer = null)
		{
			if(null == source) return false;
			comparer = comparer ?? EqualityComparer<T>.Default;
			return comparer.Equals(source.LastOrDefault(), item);
		}
		#endregion

		#region 验证是否为结构数据
		/// <summary>验证是否为结构数据</summary>
		/// <param name="type">要验证的类型</param>
		/// <returns>指示是否为结构数据</returns>
		public static bool IsStruct_(this Type type)
		{
			return ((type.IsValueType && !type.IsEnum) && (!type.IsPrimitive && !type.IsSerializable));
		}

		/// <summary>验证是否为结构数据</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="value">要验证的对象</param>
		/// <returns>指示是否为结构数据</returns>
		public static bool IsStruct_<T>(this T value)
		{
			return typeof(T).IsStruct_();
		}
		#endregion

		#region 验证是否为数字
		/// <summary>
		/// 验证是否为数字（sbyte、byte、short、ushort、int、uint、long、ulong、float、double、decimal）
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="n">要验证的对象</param>
		/// <returns>指示是否为数字</returns>
		public static bool IsNumber_<T>(this T n)
		{
			var tc = Type.GetTypeCode(typeof(T));
			return tc >= TypeCode.SByte && tc <= TypeCode.Decimal;
		}

		/// <summary>验证是否为整数（sbyte、byte、short、ushort、int、uint、long、ulong）</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="n">要验证的对象</param>
		/// <returns>指示是否为整数</returns>
		public static bool IsInt_<T>(this T n)
		{
			var tc = Type.GetTypeCode(typeof(T));
			return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
		}

		/// <summary>验证是否为浮点数（float、double、decimal）</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="n">要验证的对象</param>
		/// <returns>指示是否为浮点数</returns>
		public static bool IsFloat_<T>(this T n)
		{
			var tc = Type.GetTypeCode(typeof(T));
			return tc >= TypeCode.Single && tc <= TypeCode.Decimal;
		}
		#endregion

		#region 验证是否为虚成员
		/// <summary>验证是否为虚属性</summary>
		/// <param name="property">属性信息</param>
		/// <returns>指示是否为虚属性</returns>
		public static bool IsVirtual_(this PropertyInfo property)
		{
			var get = property.GetGetMethod();
			if(get != null && get.IsVirtual) return true;
			var set = property.GetSetMethod();
			if(set != null && set.IsVirtual) return true;
			return false;
		}
		#endregion

		#region 验证是否为可 null 类型
		/// <summary>验证是否为可 null 类型</summary>
		/// <param name="type">要验证的类型</param>
		/// <returns>指示是否为可 null 类型</returns>
		public static bool IsNullable_(this Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		/// <summary>验证是否为可 null 类型</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="value">要验证的对象</param>
		/// <returns>指示是否为可 null 类型</returns>
		public static bool IsNullable_<T>(this T value)
		{
			return value.GetType().IsNullable_();
		}
		#endregion
	}
}