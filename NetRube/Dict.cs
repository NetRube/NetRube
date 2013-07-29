using System;
using System.Collections.Generic;
using System.Threading;

namespace NetRube
{
	/// <summary>简单的可以多线程同时访问的字典</summary>
	/// <typeparam name="TKey">键的类型</typeparam>
	/// <typeparam name="TValue">值的类型</typeparam>
	public class Dict<TKey, TValue>
	{
		private ReaderWriterLockSlim LOCK;
		private Dictionary<TKey, TValue> DICT;

		/// <summary>初始化一个新 <see cref="Dict{TKey, TValue}"/> 实例。</summary>
		public Dict()
		{
			LOCK = new ReaderWriterLockSlim();
			DICT = new Dictionary<TKey, TValue>();
		}

		/// <summary>初始化一个新 <see cref="Dict{TKey, TValue}"/> 实例。</summary>
		/// <param name="capacity">可包含的初始元素数</param>
		public Dict(int capacity)
		{
			LOCK = new ReaderWriterLockSlim();
			DICT = new Dictionary<TKey, TValue>(capacity);
		}

		/// <summary>初始化一个新 <see cref="Dict{TKey, TValue}"/> 实例。</summary>
		/// <param name="comparer">比较键时要使用的比较器</param>
		public Dict(IEqualityComparer<TKey> comparer)
		{
			LOCK = new ReaderWriterLockSlim();
			DICT = new Dictionary<TKey, TValue>(comparer);
		}

		/// <summary>初始化一个新 <see cref="Dict{TKey, TValue}"/> 实例。</summary>
		/// <param name="capacity">可包含的初始元素数</param>
		/// <param name="comparer">比较键时要使用的比较器</param>
		public Dict(int capacity, IEqualityComparer<TKey> comparer)
		{
			LOCK = new ReaderWriterLockSlim();
			DICT = new Dictionary<TKey, TValue>(capacity, comparer);
		}

		/// <summary>获取当前字典中的数目</summary>
		/// <value>当前字典中的数目</value>
		public int Count
		{
			get { return DICT.Count; }
		}

		/// <summary>获取值</summary>
		/// <param name="key">键名</param>
		/// <param name="func">用于在值不存在时添加值的委托</param>
		/// <returns>值</returns>
		public TValue Get(TKey key, Func<TValue> func)
		{
			LOCK.EnterReadLock();
			TValue val;
			try
			{
				if(DICT.TryGetValue(key, out val))
					return val;
			}
			finally
			{
				LOCK.ExitReadLock();
			}

			LOCK.EnterWriteLock();
			try
			{
				if(DICT.TryGetValue(key, out val))
					return val;

				val = func();
				DICT.Add(key, val);
				return val;
			}
			finally
			{
				LOCK.ExitWriteLock();
			}
		}

		/// <summary>移除指定键的值</summary>
		/// <param name="key">要移除的键</param>
		/// <returns>返回是否移除成功</returns>
		public bool Del(TKey key)
		{
			LOCK.EnterWriteLock();
			try
			{
				return DICT.Remove(key);
			}
			finally
			{
				LOCK.ExitWriteLock();
			}
		}

		/// <summary>清除所有条目</summary>
		public void Clear()
		{
			LOCK.EnterWriteLock();
			try
			{
				DICT.Clear();
			}
			finally
			{
				LOCK.ExitWriteLock();
			}
		}
	}
}