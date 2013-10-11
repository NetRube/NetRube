using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NetRube
{
	public static partial class Utils
	{
		// 其它操作

		/// <summary>基本日期，1900-01-01，主要针对 SQL 数据库的最小日期范围</summary>
		/// <value>针对 SQL 数据库的最小日期</value>
		public static readonly DateTime BaseDateTime = new DateTime(1900, 1, 1);

		/// <summary>是否运行于 Asp.Net 环境</summary>
		/// <returns>如果运行于 Asp.Net 环境中，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public static bool IsInAspDotNet()
		{
			return null == System.Reflection.Assembly.GetEntryAssembly();
		}

		/// <summary>是否运行于 Mono 环境</summary>
		/// <returns>如果运行于 Mono 环境中，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public static bool IsInMono()
		{
			return null != Type.GetType("Mono.Runtime");
		}

		#region GetPropertyName
		/// <summary>获取属性名称</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性名称</returns>
		public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
		{
			return GetPropertyInfo<T>((Expression)expression).Name;
		}

		/// <summary>获取属性名称</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <typeparam name="V">属性值类型</typeparam>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性名称</returns>
		public static string GetPropertyName<T, V>(Expression<Func<T, V>> expression)
		{
			return GetPropertyInfo<T>((Expression)expression).Name;
		}

		/// <summary>获取属性名称</summary>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性名称</returns>
		public static string GetPropertyName(Expression expression)
		{
			return GetPropertyInfo(expression).Name;
		}

		/// <summary>获取属性名称</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性名称</returns>
		public static string GetPropertyName<T>(Expression expression)
		{
			return GetPropertyInfo<T>(expression).Name;
		}
		#endregion

		#region GetPropertyInfo
		/// <summary>获取属性信息</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性信息</returns>
		public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> expression)
		{
			return GetPropertyInfo<T>((Expression)expression);
		}

		/// <summary>获取属性信息</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <typeparam name="V">属性值类型</typeparam>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性信息</returns>
		public static PropertyInfo GetPropertyInfo<T, V>(Expression<Func<T, V>> expression)
		{
			return GetPropertyInfo<T>((Expression)expression);
		}

		/// <summary>获取属性信息</summary>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性信息</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		public static PropertyInfo GetPropertyInfo(Expression expression)
		{
			Type type = null;
			Expression body = expression;
			var le = body as LambdaExpression;
			if(le != null)
			{
				body = le.Body;
				type = le.Parameters[0].Type;
			}
			var ue = body as UnaryExpression;
			if(ue != null)
			{
				body = ue.Operand;
			}

			var me = body as MemberExpression;
			if(me != null)
			{
				if(type != null)
					return type.GetProperty(me.Member.Name);
				return me.Member as PropertyInfo;
			}

			throw new InvalidOperationException();
		}

		/// <summary>获取属性信息</summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="expression">属性表达式</param>
		/// <returns>属性信息</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		public static PropertyInfo GetPropertyInfo<T>(Expression expression)
		{
			var type = typeof(T);
			var body = expression;
			var le = body as LambdaExpression;
			if(le != null)
				body = le.Body;
			var ue = body as UnaryExpression;
			if(ue != null)
				body = ue.Operand;
			var me = body as MemberExpression;
			if(me != null)
			{
				if(me.Member.ReflectedType == type)
					return me.Member as PropertyInfo;
				return type.GetProperty(me.Member.Name);
			}

			throw new InvalidOperationException();
		}
		#endregion

		#region GetDescription
		/// <summary>获取枚举值的说明（Description 属性），如果不存在则返回该枚举值的名称</summary>
		/// <param name="e">枚举值</param>
		/// <returns>说明或名称</returns>
		public static string GetDescription_(this Enum e)
		{
			var type = e.GetType();
			var val = Enum.GetName(type, e);
			var des = type.GetField(val).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
			return des == null ? val : ((DescriptionAttribute)des).Description;
		}
		#endregion

		#region GetLocalization
		/// <summary>获取本地化资源</summary>
		/// <param name="e">要获取的枚举值</param>
		/// <param name="res">所在的本地化资源</param>
		/// <param name="info">区域性信息</param>
		/// <returns>本地化资源</returns>
		public static string GetLocalization_(this Enum e, System.Resources.ResourceManager res, System.Globalization.CultureInfo info = null)
		{
			var type = e.GetType();
			var name = type.Name + "_" + Enum.GetName(type, e);
			return GetLocalization_(name, res, info);
		}

		/// <summary>获取本地化资源</summary>
		/// <param name="name">要获取的资源名称</param>
		/// <param name="res">所在的本地化资源</param>
		/// <param name="info">区域性信息</param>
		/// <returns>本地化资源</returns>
		public static string GetLocalization_(string name, System.Resources.ResourceManager res, System.Globalization.CultureInfo info = null)
		{
			if(name.IsNullOrEmpty_()) return string.Empty;
			return res.GetString(name, info);
		}
		#endregion

		#region GetValidValue
		/// <summary>获取对象有效值</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <typeparam name="V">值类型</typeparam>
		/// <param name="obj">要获取的对象</param>
		/// <param name="key">对象键</param>
		/// <param name="defval">无法获取时的默认值</param>
		/// <returns>有效值</returns>
		public static V GetValidValue_<T, V>(this T obj, Func<T, V> key, V defval = default(V))
		{
			if(obj == null || key == null)
				return defval;
			return key(obj);
		}

		/// <summary>获取对象有效值</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="obj">要获取的对象</param>
		/// <param name="key">对象键</param>
		/// <param name="defval">无法获取时的默认值</param>
		/// <returns>有效值</returns>
		public static object GetValidValue_<T>(this T obj, Func<T, object> key, object defval)
		{
			if(obj == null || key == null)
				return defval;
			return key(obj);
		}
		#endregion

		#region 克隆
		/// <summary>克隆对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="obj">要克隆的对象</param>
		/// <returns>新对象</returns>
		public static T Clone_<T>(this T obj)
		{
			if(obj == null) return default(T);

			var type = obj.GetType();
			if(type.IsValueType)
				return obj;

			var id = obj as IDictionary;
			if(id != null)
				return (T)CloneDict(id);

			var il = obj as IList;
			if(il != null)
				return (T)CloneList(il);

			var ic = obj as ICloneable;
			if(ic != null)
				return (T)ic.Clone();

			var newObj = FastReflection.FastInvoke<T>();
			var ls = FastReflection.FastGetAccessors(type);
			foreach(var a in ls.Values)
			{
				if(!a.CanRade || !a.CanWrite)
					continue;

				a.SetValue(newObj, Clone_(a.GetValue(obj)));
			}
			return newObj;
		}

		private static IDictionary CloneDict(IDictionary list)
		{
			if(list == null) return null;

			IDictionary ls = list.GetType().FastInvoke(list.Count) as IDictionary;
			if(ls == null) return null;

			foreach(DictionaryEntry item in list)
				ls.Add(Clone_(item.Key), Clone_(item.Value));
			return ls;
		}

		private static IList CloneList(IList list)
		{
			if(list == null) return null;

			IList ls = list.GetType().FastInvoke(list.Count) as IList;
			if(ls == null) return null;

			foreach(var item in list)
				ls.Add(Clone_(item));
			return ls;
		}
		#endregion
	}
}