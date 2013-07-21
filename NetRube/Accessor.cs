using System;
using System.Reflection;

namespace NetRube
{
	/// <summary>字段/属性访问器</summary>
	public class Accessor
	{
		/// <summary>访问器名称</summary>
		public string Name;
		/// <summary>成员信息</summary>
		public MemberInfo Member;
		/// <summary>访问器数据类型</summary>
		public Type DataType;
		/// <summary>访问器类型</summary>
		public AccessorType AccessorType;
		/// <summary>是否为可读属性</summary>
		public bool CanRade;
		/// <summary>是否为可写属性</summary>
		public bool CanWrite;
		/// <summary>是否为虚属性</summary>
		public bool IsVirtual;

		/// <summary>设置访问器的值</summary>
		/// <param name="instance">要设置的对象实例</param>
		/// <param name="value">要设置的值</param>
		public void SetValue(object instance, object value)
		{
			switch(this.AccessorType)
			{
				case AccessorType.Property:
					((PropertyInfo)this.Member).FastSetValue(instance, value);
					break;
				case AccessorType.Field:
					((FieldInfo)this.Member).FastSetValue(instance, value);
					break;
			}
		}

		/// <summary>获取访问器的值</summary>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>访问器的值</returns>
		public object GetValue(object instance)
		{
			object val = null;
			switch(this.AccessorType)
			{
				case AccessorType.Property:
					val = ((PropertyInfo)this.Member).FastGetValue(instance);
					break;
				case AccessorType.Field:
					val = ((FieldInfo)this.Member).FastGetValue(instance);
					break;
			}
			return val;
		}

		/// <summary>获取访问器的值</summary>
		/// <typeparam name="T">值的数据类型</typeparam>
		/// <param name="instance">要获取的对象实例</param>
		/// <returns>访问器的值</returns>
		public T GetValue<T>(object instance)
		{
			return (T)this.GetValue(instance);
		}
	}
}