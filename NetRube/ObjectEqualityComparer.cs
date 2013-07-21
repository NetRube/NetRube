using System;
using System.Collections.Generic;

namespace NetRube
{
	/// <summary>对象相等比较器</summary>
	/// <typeparam name="T">对象类型</typeparam>
	public class ObjectEqualityComparer<T> : IEqualityComparer<T>
	{
		/// <summary>初始化一个新 <see cref="ObjectEqualityComparer&lt;T&gt;" /> 实例。</summary>
		/// <param name="equals">比较函数</param>
		/// <param name="getHashCode">获取哈希代码函数，如果为 null 默认返回 1</param>
		public ObjectEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode = null)
		{
			this.__equals = equals;
			this.__getHashCode = getHashCode;
		}

		private Func<T, T, bool> __equals;
		private Func<T, int> __getHashCode;

		/// <summary>确定指定的对象是否相等</summary>
		/// <param name="x">要比较的第一个对象</param>
		/// <param name="y">要比较的第二个对象</param>
		/// <returns>指示指定的对象是否相等</returns>
		public bool Equals(T x, T y)
		{
			if(!typeof(T).IsValueType)
			{
				if(object.ReferenceEquals(x, y)) return true;
				if(object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;
			}
			return this.__equals(x, y);
		}

		/// <summary>返回指定对象的哈希代码</summary>
		/// <param name="obj">将为其返回哈希代码</param>
		/// <returns>指定对象的哈希代码</returns>
		public int GetHashCode(T obj)
		{
			if(this.__getHashCode == null) return 1;
			return this.__getHashCode(obj);
		}
	}
}