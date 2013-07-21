using System;
using System.Collections.Generic;
using System.Linq;

namespace NetRube
{
	public static partial class Utils
	{
		// 数组操作

		/// <summary>返回空数组</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <returns>一个长度为 0 的空数组</returns>
		public static T[] EmptyArray<T>()
		{
			return new T[0];
		}

		/// <summary>以传入的参数包装成数组</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="value">要包装的参数</param>
		/// <returns>以传入的参数组成的数组</returns>
		public static T[] GetArray<T>(params T[] value)
		{
			return value;
		}

		/// <summary>将传入的参数合并成数组</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="item">要合并的对象</param>
		/// <param name="value">要合并的集合</param>
		/// <returns>合并成的数组</returns>
		public static IEnumerable<T> Concat_<T>(this T item, params T[] value)
		{
			if(item == null && value.IsNullOrEmpty_())
				return null;
			if(item == null) return value;
			if(value.IsNullOrEmpty_()) return GetArray(item);

			return GetArray(item).Concat(value);
		}

		/// <summary>将传入的参数合并成数组</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要合并的集合</param>
		/// <param name="item">要合并的对象</param>
		/// <returns>合并成的数组</returns>
		public static IEnumerable<T> Concat_<T>(this IEnumerable<T> source, T item)
		{
			if(item == null && source.IsNullOrEmpty_())
				return null;
			if(item == null) return source;
			if(source.IsNullOrEmpty_()) return GetArray(item);

			return source.Concat(GetArray(item));
		}

		/// <summary>以指定的大小创建新数组</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="size">数组大小</param>
		/// <returns>一个指定大小的空数组</returns>
		public static T[] NewArray<T>(int size)
		{
			return new T[size];
		}

		/// <summary>设置数组大小</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要设置的数组</param>
		/// <param name="size">新数组的大小</param>
		/// <returns>一个指定大小的新数组</returns>
		public static T[] SetSize_<T>(this T[] source, int size)
		{
			if(size <= 0) return EmptyArray<T>();
			T[] newArray = NewArray<T>(size);
			if(source.IsNullOrEmpty_()) return newArray;
			int len = source.Length;
			if(size == len) return source;

			Array.Copy(source, 0, newArray, 0, (len > size) ? size : len);
			return newArray;
		}

		/// <summary>设置数组大小</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要设置的数组</param>
		/// <param name="size">新数组的大小</param>
		/// <param name="defval">当新数组大小大于源数组时要添加的默认值</param>
		/// <returns>一个指定大小的新数组</returns>
		public static T[] SetSize_<T>(this T[] source, int size, T defval)
		{
			if(size <= 0) return EmptyArray<T>();
			T[] newArray = NewArray<T>(size);
			int len = source.IsNullOrEmpty_() ? 0 : source.Length;
			if(size == len) return source;

			int copyLen = size;
			if(size > len)
			{
				copyLen = len;
				for(int i = len - 1; i < size; i++)
					newArray[i] = defval;
			}
			if(len > 0)
				Array.Copy(source, 0, newArray, 0, copyLen);

			return newArray;
		}

		/// <summary>复制集合。复制后的集合元素会受原元素的修改影响</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要复制的集合</param>
		/// <returns>新集合</returns>
		public static List<T> Copy_<T>(this List<T> source)
		{
			return source.GetRange(0, source.Count);
		}

		/// <summary>复制集合。复制后的集合元素会受原元素的修改影响</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要复制的集合</param>
		/// <returns>新集合</returns>
		public static IEnumerable<T> Copy_<T>(this IEnumerable<T> source)
		{
			return source.ToList().Copy_();
		}

		/// <summary>将非 null 对象添加到集合</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">原集合</param>
		/// <param name="item">要添加的对象</param>
		/// <returns>新集合</returns>
		public static List<T> Add_<T>(this List<T> source, T item)
		{
			if(source == null) return null;
			if(item != null)
				source.Add(item);
			return source;
		}

		/// <summary>将非 null 对象添加到集合</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">原集合</param>
		/// <param name="item">要添加的对象</param>
		/// <returns>新集合</returns>
		public static ICollection<T> Add_<T>(this ICollection<T> source, T item)
		{
			if(source == null) return null;
			if(item != null)
				source.Add(item);
			return source;
		}

		/// <summary>串连数组为字符串</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">要串连的数组</param>
		/// <param name="separator">分隔符</param>
		/// <returns>串连后的新字符串</returns>
		public static string Join_<T>(this IEnumerable<T> source, string separator = null)
		{
			if(source.IsNullOrEmpty_()) return string.Empty;

			if(separator == null)
				return string.Concat(source);
			return string.Join(separator, source);
		}

		/// <summary>串连数组为字符串</summary>
		/// <param name="source">要串连的数组</param>
		/// <param name="separator">分隔符</param>
		/// <returns>串连后的新字符串</returns>
		public static string Join_(this string[] source, string separator = null)
		{
			if(source.IsNullOrEmpty_()) return string.Empty;

			if(separator == null)
				return string.Concat(source);
			return string.Join(separator, source);
		}

		/// <summary>对集合中的每个元素执行指定操作</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="action">要对集合中的每个元素执行的委托</param>
		public static void ForEach_<T>(this IEnumerable<T> source, Action<T> action)
		{
			if(source == null) return;
			foreach(T item in source)
				action(item);
		}

		/// <summary>对集合中的每个元素执行指定操作</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="action">要对集合中的每个元素执行的委托</param>
		public static void ForEach_<T>(this IEnumerable<T> source, Action<T, int> action)
		{
			if(source == null) return;
			var i = 0;
			foreach(T item in source)
			{
				action(item, i);
				i++;
			}
		}

		/// <summary>检索指定的元素在集合中第一次出现的索引位置，如果没找到则返回 -1</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="value">要检索的元素</param>
		/// <param name="comparer">相等比较器</param>
		/// <returns>元素在集合中第一次出现的索引位置，如果没找到则返回 -1</returns>
		public static int IndexOf_<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer = null)
		{
			var i = -1;
			if(source != null && value != null)
			{
				comparer = comparer ?? EqualityComparer<T>.Default;
				foreach(T obj in source)
				{
					i++;
					if(comparer.Equals(obj, value))
						break;
				}
			}
			return i;
		}

		/// <summary>获取元素位于集合中的下一个元素</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="item">要检索的元素</param>
		/// <param name="comparer">相等比较器</param>
		/// <returns>下一个元素</returns>
		public static T GetNext_<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer = null)
		{
			if(source.IsNullOrEmpty_() || item == null) return default(T);
			var i = source.IndexOf_(item, comparer);
			if(i == -1) return default(T);
			return source.ElementAtOrDefault(i);
		}

		/// <summary>获取元素位于集合中的上一个元素</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="item">要检索的元素</param>
		/// <param name="comparer">相等比较器</param>
		/// <returns>上一个元素</returns>
		public static T GetPrevious_<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer = null)
		{
			if(source.IsNullOrEmpty_()) return default(T);
			var i = source.IndexOf_(item, comparer);
			if(i < 1) return default(T);
			return source.ElementAtOrDefault(i - 1);
		}

		/// <summary>返回序列中的最大值或默认值</summary>
		/// <param name="source">集合</param>
		/// <returns>序列中的最大值或默认值</returns>
		public static int MaxOrDefault_(this IEnumerable<int> source)
		{
			if(source.IsNullOrEmpty_()) return 0;
			return source.Max();
		}

		/// <summary>返回序列中的最大值或默认值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <returns>序列中的最大值或默认值</returns>
		public static T MaxOrDefault_<T>(this IEnumerable<T> source)
		{
			if(source.IsNullOrEmpty_()) return default(T);
			return source.Max();
		}

		/// <summary>返回序列中的最大值或默认值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="selector">应用于每个元素的转换函数</param>
		/// <returns>序列中的最大值或默认值</returns>
		public static int MaxOrDefault_<T>(this IEnumerable<T> source, Func<T, int> selector)
		{
			if(source.IsNullOrEmpty_()) return 0;
			return source
				.Select(selector)
				.MaxOrDefault_();
		}

		/// <summary>返回序列中的最大值或默认值</summary>
		/// <typeparam name="TSource">集合中元素的数据类型</typeparam>
		/// <typeparam name="TResult">返回的数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="selector">应用于每个元素的转换函数</param>
		/// <returns>序列中的最大值或默认值</returns>
		public static TResult MaxOrDefault_<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if(source == null) return default(TResult);
			return source
				.Select(selector)
				.MaxOrDefault_();
		}

		/// <summary>返回序列中的最小值或默认值</summary>
		/// <param name="source">集合</param>
		/// <returns>序列中的最小值或默认值</returns>
		public static int MinOrDefault_(this IEnumerable<int> source)
		{
			if(source.IsNullOrEmpty_()) return 0;
			return source.Min();
		}

		/// <summary>返回序列中的最小值或默认值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <returns>序列中的最小值或默认值</returns>
		public static T MinOrDefault_<T>(this IEnumerable<T> source)
		{
			if(source.IsNullOrEmpty_()) return default(T);
			return source.Min();
		}

		/// <summary>返回序列中的最小值或默认值</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="selector">应用于每个元素的转换函数</param>
		/// <returns>序列中的最小值或默认值</returns>
		public static int MinOrDefault_<T>(this IEnumerable<T> source, Func<T, int> selector)
		{
			if(source.IsNullOrEmpty_()) return 0;
			return source
				.Select(selector)
				.MinOrDefault_();
		}

		/// <summary>返回序列中的最小值或默认值</summary>
		/// <typeparam name="TSource">集合中元素的数据类型</typeparam>
		/// <typeparam name="TResult">返回的数据类型</typeparam>
		/// <param name="source">集合</param>
		/// <param name="selector">应用于每个元素的转换函数</param>
		/// <returns>序列中的最小值或默认值</returns>
		public static TResult MinOrDefault_<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if(source == null) return default(TResult);
			return source
				.Select(selector)
				.MinOrDefault_();
		}
	}
}