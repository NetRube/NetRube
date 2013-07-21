using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NetRube
{
	/// <summary>数组集合缓冲器</summary>
	/// <typeparam name="T">数据类型</typeparam>
	[StructLayout(LayoutKind.Sequential)]
	public struct ArrayBuffer<T>
	{
		private T[] ITEMS;
		private int COUNT;

		/// <summary>初始化一个新 <see cref="ArrayBuffer&lt;T&gt;" /> 实例。</summary>
		/// <param name="source">用于初始化的集合</param>
		public ArrayBuffer(IEnumerable<T> source)
		{
			this.ITEMS = null;
			this.COUNT = 0;
			ICollection<T> ic = source as ICollection<T>;
			if(ic != null)
			{
				this.COUNT = ic.Count;
				if(this.COUNT > 0)
				{
					this.ITEMS = Utils.NewArray<T>(this.COUNT);
					ic.CopyTo(this.ITEMS, 0);
				}
			}
			else
			{
				foreach(T item in source)
				{
					if(this.ITEMS == null)
						this.ITEMS = Utils.NewArray<T>(8);
					else if(this.ITEMS.Length == this.COUNT)
					{
						T[] array = Utils.NewArray<T>(this.COUNT * 2);
						Array.Copy(this.ITEMS, 0, array, 0, this.COUNT);
						this.ITEMS = array;
					}
					this.ITEMS[this.COUNT] = item;
					this.COUNT++;
				}
			}
		}

		/// <summary>初始化一个新 <see cref="ArrayBuffer&lt;T&gt;" /> 实例。</summary>
		/// <param name="source">用于初始化的集合</param>
		public ArrayBuffer(T[] source)
		{
			this.ITEMS = source;
			this.COUNT = source.Length;
		}

		/// <summary>转换成数组</summary>
		/// <returns>数组</returns>
		public T[] ToArray()
		{
			if(this.COUNT == 0) return Utils.EmptyArray<T>();
			if(this.ITEMS.Length == this.COUNT) return this.ITEMS;
			T[] array = Utils.NewArray<T>(this.COUNT);
			Array.Copy(this.ITEMS, 0, array, 0, this.COUNT);
			return array;
		}

		/// <summary>转换成列表</summary>
		/// <returns>列表</returns>
		public List<T> ToList()
		{
			return new List<T>(this.ToArray());
		}
	}
}