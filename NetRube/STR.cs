using System;
using System.Collections.Generic;
using System.Text;

namespace NetRube
{
	/// <summary>字符串处理</summary>
	public class STR
	{
		/// <summary>内部 StringBuilder 对象</summary>
		protected StringBuilder __SB;

		#region 初始化
		/// <summary>初始化一个新 <see cref="STR" /> 实例。</summary>
		public STR()
		{
			this.__SB = new StringBuilder();
		}

		/// <summary>使用指定的容量初始化新 <see cref="STR" /> 实例</summary>
		/// <param name="capacity">此实例的建议起始大小</param>
		public STR(int capacity)
		{
			this.__SB = new StringBuilder(capacity);
		}

		/// <summary>使用指定的字符串初始化新 <see cref="STR" /> 实例</summary>
		/// <param name="value">用于初始化实例值的字符串。如果 value 为 nullNothingnullptrnull 引用（在 Visual Basic 中为 Nothing），则新的实例将包含空字符串（即包含 Empty）</param>
		public STR(string value)
		{
			this.__SB = new StringBuilder(value);
		}

		/// <summary>初始化新 <see cref="STR" /> 实例，起始于指定容量并且可增长到指定的最大容量</summary>
		/// <param name="capacity">此实例的建议起始大小</param>
		/// <param name="maxCapacity">当前字符串可包含的最大字符数</param>
		public STR(int capacity, int maxCapacity)
		{
			this.__SB = new StringBuilder(capacity, maxCapacity);
		}

		/// <summary>使用指定的字符串和容量初始化新 <see cref="STR" /> 实例</summary>
		/// <param name="value">用于初始化实例值的字符串。如果 value 为 nullNothingnullptrnull 引用（在 Visual Basic 中为 Nothing），则新的实例将包含空字符串（即包含 Empty）</param>
		/// <param name="capacity">此实例的建议起始大小</param>
		public STR(string value, int capacity)
		{
			this.__SB = new StringBuilder(value, capacity);
		}

		/// <summary>使用指定的子字符串和容量初始化新 <see cref="STR" /> 实例</summary>
		/// <param name="value">字符串，包含用于初始化此实例值的子字符串。如果 value 为 nullNothingnullptrnull 引用（在 Visual Basic 中为 Nothing），则新的实例将包含空字符串（即包含 Empty）</param>
		/// <param name="starIndex">value 中子字符串开始的位置</param>
		/// <param name="length">子字符串中的字符数</param>
		/// <param name="capacity">此实例的建议起始大小</param>
		public STR(string value, int starIndex, int length, int capacity)
		{
			this.__SB = new StringBuilder(value, starIndex, length, capacity);
		}

		/// <summary>使用指定的字符串集合初始化新 <see cref="STR" /> 实例</summary>
		/// <param name="collection">用于初始化的字符串集合</param>
		public STR(IEnumerable<string> collection)
		{
			if(collection.IsNullOrEmpty_())
				this.__SB = new StringBuilder();
			else
			{
				ICollection<string> ic = collection as ICollection<string>;
				if(ic != null)
					this.__SB = new StringBuilder(ic.Count);
				foreach(string s in collection)
					this.__SB.Append(s);
			}
		}

		/// <summary>使用指定的另一个实例初始化新 <see cref="STR" /> 实例</summary>
		/// <param name="value">用于初始化实例值的另一个实例</param>
		public STR(STR value)
		{
			this.__SB = value.__SB;
		}

		/// <summary>使用指定的 StringBuilder 对象初始化新 <see cref="STR" /> 实例</summary>
		/// <param name="value">StringBuilder 对象</param>
		public STR(StringBuilder value)
		{
			this.__SB = value;
		}
		#endregion

		#region 属性
		/// <summary>获取或设置可包含在当前实例所分配的内存中的最大字符数</summary>
		/// <value>可包含在当前实例所分配的内存中的最大字符数</value>
		public int Capacity
		{
			get { return this.__SB.Capacity; }
			set { this.__SB.Capacity = value; }
		}

		/// <summary>获取或设置此实例中指定字符位置处的字符</summary>
		/// <value><paramref name="index" /> 位置处的 Unicode 字符</value>
		/// <param name="index">字符的位置</param>
		/// <returns></returns>
		public char this[int index]
		{
			get { return this.__SB[index]; }
			set { this.__SB[index] = value; }
		}

		/// <summary>获取或设置当前字符串的长度</summary>
		/// <value>此实例的长度</value>
		public int Length
		{
			get { return this.__SB.Length; }
			set { this.__SB.Length = value; }
		}

		/// <summary>获取此实例的最大容量</summary>
		/// <value>此实例可容纳的最大字符数</value>
		public int MaxCapacity
		{
			get { return this.__SB.MaxCapacity; }
		}

		/// <summary>获取此实例的内容是否为空</summary>
		/// <value>如果内容为空，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsEmpty
		{
			get { return this.__SB.Length == 0; }
		}
		#endregion

		#region 追加
		/// <summary>在此实例的结尾追加字符串</summary>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(string value)
		{
			this.__SB.Append(value);
			return this;
		}

		/// <summary>在此实例的结尾追加字符串集合</summary>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(params string[] value)
		{
			if(value.IsNullOrEmpty_()) return this;
			foreach(string str in value)
				this.__SB.Append(str);
			return this;
		}

		/// <summary>在此实例的结尾追加字符串集合</summary>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(IEnumerable<string> value)
		{
			if(value.IsNullOrEmpty_()) return this;
			foreach(string str in value)
				this.__SB.Append(str);
			return this;
		}

		/// <summary>在此实例的结尾追加对象的字符串表示形式</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append<T>(T value)
		{
			return this.Append(value.ToString());
		}

		/// <summary>在此实例的结尾追加指定的 Unicode 字符的字符串表示形式</summary>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(char value)
		{
			this.__SB.Append(value);
			return this;
		}

		/// <summary>在此实例的结尾追加指定的 Unicode 字符数组的字符串表示形式</summary>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(char[] value)
		{
			this.__SB.Append(value);
			return this;
		}

		/// <summary>在此实例的结尾追加 Unicode 字符的字符串表示形式指定数目的副本</summary>
		/// <param name="value">要追加的字符</param>
		/// <param name="repeatCount">追加 value 的次数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(char value, int repeatCount)
		{
			this.__SB.Append(value, repeatCount);
			return this;
		}

		/// <summary>在此实例的结尾追加指定的 Unicode 字符子数组的字符串表示形式</summary>
		/// <param name="value">字符数组</param>
		/// <param name="startIndex">value 内的起始索引</param>
		/// <param name="charCount">要追加的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(char[] value, int startIndex, int charCount)
		{
			this.__SB.Append(value, startIndex, charCount);
			return this;
		}

		/// <summary>在此实例的结尾追加指定子字符串的副本</summary>
		/// <param name="value">包含要追加的子字符串的 String</param>
		/// <param name="startIndex">value 中子字符串开始的位置</param>
		/// <param name="charCount">value 中要追加的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(string value, int startIndex, int charCount)
		{
			this.__SB.Append(value, startIndex, charCount);
			return this;
		}

		/// <summary>在此实例的结尾追加另一个实例的字符串表示形式</summary>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(STR value)
		{
			this.__SB.Append(value.__SB.ToString());
			return this;
		}

		/// <summary>在此实例的结尾追加另一个实例指定子字符串的副本</summary>
		/// <param name="value">包含要追加的另一个实例</param>
		/// <param name="startIndex">value 中子字符串开始的位置</param>
		/// <param name="charCount">value 中要追加的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(STR value, int startIndex, int charCount)
		{
			this.__SB.Append(value.__SB.ToString(), startIndex, charCount);
			return this;
		}

		/// <summary>在此实例的结尾追加 StringBuilder 实例的字符串表示形式</summary>
		/// <param name="value">要追加的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(StringBuilder value)
		{
			this.__SB.Append(value.ToString());
			return this;
		}

		/// <summary>在此实例的结尾追加另一个实例指定子字符串的副本</summary>
		/// <param name="value">包含要追加的另一个实例</param>
		/// <param name="startIndex">value 中子字符串开始的位置</param>
		/// <param name="charCount">value 中要追加的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Append(StringBuilder value, int startIndex, int charCount)
		{
			this.__SB.Append(value.ToString(), startIndex, charCount);
			return this;
		}
		#endregion

		#region 格式规范追加
		/// <summary>向此实例追加包含零个或更多格式规范的格式化字符串。每个格式规范由相应对象参数的字符串表示形式替换</summary>
		/// <param name="format">复合格式字符串</param>
		/// <param name="arg0">要设置格式的对象</param>
		/// <returns>此字符串处理实例</returns>
		public STR AppendFormat(string format, object arg0)
		{
			this.__SB.AppendFormat(format, arg0);
			return this;
		}

		/// <summary>向此实例追加包含零个或更多格式规范的格式化字符串。每个格式规范由相应对象参数的字符串表示形式替换</summary>
		/// <param name="format">复合格式字符串</param>
		/// <param name="args">要设置格式的对象数组</param>
		/// <returns>此字符串处理实例</returns>
		public STR AppendFormat(string format, params object[] args)
		{
			this.__SB.AppendFormat(format, args);
			return this;
		}

		/// <summary>向此实例追加包含零个或更多格式规范的设置了格式的字符串。每个格式规范由相应对象参数的字符串表示形式替换</summary>
		/// <param name="provider">一个 IFormatProvider，它提供区域性特定的格式设置信息</param>
		/// <param name="format">复合格式字符串</param>
		/// <param name="args">要设置格式的对象数组</param>
		/// <returns>此字符串处理实例</returns>
		public STR AppendFormat(IFormatProvider provider, string format, params object[] args)
		{
			this.__SB.AppendFormat(provider, format, args);
			return this;
		}

		/// <summary>向此实例追加包含零个或更多格式规范的格式化字符串。每个格式规范由相应对象参数的字符串表示形式替换</summary>
		/// <param name="format">复合格式字符串</param>
		/// <param name="arg0">要设置格式的第一个对象</param>
		/// <param name="arg1">要设置格式的第二个对象</param>
		/// <returns>此字符串处理实例</returns>
		public STR AppendFormat(string format, object arg0, object arg1)
		{
			this.__SB.AppendFormat(format, arg0, arg1);
			return this;
		}

		/// <summary>向此实例追加包含零个或更多格式规范的格式化字符串。每个格式规范由相应对象参数的字符串表示形式替换</summary>
		/// <param name="format">复合格式字符串</param>
		/// <param name="arg0">要设置格式的第一个对象</param>
		/// <param name="arg1">要设置格式的第二个对象</param>
		/// <param name="arg2">要设置格式的第三个对象</param>
		/// <returns>此字符串处理实例</returns>
		public STR AppendFormat(string format, object arg0, object arg1, object arg2)
		{
			this.__SB.AppendFormat(format, arg0, arg1, arg2);
			return this;
		}
		#endregion

		#region 替换
		/// <summary>将此实例中所有的指定字符替换为其他指定字符</summary>
		/// <param name="oldChar">要替换的字符</param>
		/// <param name="newChar">替换 oldChar 的字符</param>
		/// <returns>此字符串处理实例</returns>
		public STR Replace(char oldChar, char newChar)
		{
			this.__SB.Replace(oldChar, newChar);
			return this;
		}

		/// <summary>将此实例中所有指定字符串的匹配项替换为其他指定字符串</summary>
		/// <param name="oldValue">要替换的字符串</param>
		/// <param name="newValue">替换 oldValue 的字符串</param>
		/// <returns>此字符串处理实例</returns>
		public STR Replace(string oldValue, string newValue)
		{
			this.__SB.Replace(oldValue, newValue);
			return this;
		}

		/// <summary>将此实例的子字符串中所有指定字符的匹配项替换为其他指定字符</summary>
		/// <param name="oldChar">要替换的字符</param>
		/// <param name="newChar">替换 oldChar 的字符</param>
		/// <param name="startIndex">此实例中子字符串开始的位置</param>
		/// <param name="count">子字符串的长度</param>
		/// <returns>此字符串处理实例</returns>
		public STR Replace(char oldChar, char newChar, int startIndex, int count)
		{
			this.__SB.Replace(oldChar, newChar, startIndex, count);
			return this;
		}

		/// <summary>将此实例的子字符串中所有指定字符串的匹配项替换为其他指定字符串</summary>
		/// <param name="oldValue">要替换的字符串</param>
		/// <param name="newValue">替换 oldValue 的字符串</param>
		/// <param name="startIndex">此实例中子字符串开始的位置</param>
		/// <param name="count">子字符串的长度</param>
		/// <returns>此字符串处理实例</returns>
		public STR Replace(string oldValue, string newValue, int startIndex, int count)
		{
			this.__SB.Replace(oldValue, newValue, startIndex, count);
			return this;
		}
		#endregion

		#region 插入
		/// <summary>将指定的 Unicode 字符的字符串表示形式插入到此实例中的指定位置</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, char value)
		{
			this.__SB.Insert(index, value);
			return this;
		}

		/// <summary>将指定的 Unicode 字符数组的字符串表示形式插入到此实例中的指定字符位置</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, char[] value)
		{
			this.__SB.Insert(index, value);
			return this;
		}

		/// <summary>将字符串插入到此实例中的指定字符位置</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, string value)
		{
			this.__SB.Insert(index, value);
			return this;
		}

		/// <summary>将字符串集合插入到此实例中的指定字符位置</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, params string[] value)
		{
			if(value.IsNullOrEmpty_()) return this;
			this.__SB.Insert(index, value.Join_());
			return this;
		}

		/// <summary>将字符串集合插入到此实例中的指定字符位置</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, IEnumerable<string> value)
		{
			if(value.IsNullOrEmpty_()) return this;
			this.__SB.Insert(index, value.Join_());
			return this;
		}

		/// <summary>将对象的字符串表示形式插入到此实例中的指定字符位置</summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert<T>(int index, T value)
		{
			return this.Insert(index, value.ToString());
		}

		/// <summary>将指定字符串的一个或更多副本插入到此实例中的指定字符位置</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的字符串</param>
		/// <param name="count">插入 value 的次数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, string value, int count)
		{
			this.__SB.Insert(index, value, count);
			return this;
		}

		/// <summary>将指定的 Unicode 字符子数组的字符串表示形式插入到此实例中的指定字符位置</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">字符数组</param>
		/// <param name="startIndex">value 内的起始索引</param>
		/// <param name="charCount">要插入的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, char[] value, int startIndex, int charCount)
		{
			this.__SB.Insert(index, value, startIndex, charCount);
			return this;
		}

		/// <summary>在此实例的结尾追加另一个实例的字符串表示形式</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, STR value)
		{
			this.__SB.Insert(index, value.__SB.ToString());
			return this;
		}

		/// <summary>在此实例的结尾追加 StringBuilder 实例的字符串表示形式</summary>
		/// <param name="index">此实例中开始插入的位置</param>
		/// <param name="value">要插入的值</param>
		/// <returns>此字符串处理实例</returns>
		public STR Insert(int index, StringBuilder value)
		{
			this.__SB.Insert(index, value.ToString());
			return this;
		}
		#endregion

		#region Trim
		private STR TrimHelper(char[] trimChars, int trimType)
		{
			int len = this.__SB.Length;
			if(len == 0) return this;
			bool f = trimChars.IsNullOrEmpty_();
			int index;
			if(trimType != 1)
			{
				for(index = 0; index < this.__SB.Length; index++)
				{
					if(f)
					{
						if(!this.__SB[index].IsWhiteSpace_())
							break;
					}
					else
					{
						if(!this.__SB[index].In_(trimChars))
							break;
					}
				}
				if(index > 0)
					this.__SB.Remove(0, index);
			}
			if(trimType != 0)
			{
				len--;
				for(index = len; index >= 0; index--)
				{
					if(f)
					{
						if(!this.__SB[index].IsWhiteSpace_())
							break;
					}
					else
					{
						if(!this.__SB[index].In_(trimChars))
							break;
					}
				}
				if(index < len)
					this.__SB.Remove(index + 1, len - index);
			}
			return this;
		}

		/// <summary>移除前导字符</summary>
		/// <param name="trimChars">要移除的字符，如果不指定将移除空白字符</param>
		/// <returns>此字符串处理实例</returns>
		public STR TrimStart(params char[] trimChars)
		{
			return this.TrimHelper(trimChars, 0);
		}

		/// <summary>移除结尾字符</summary>
		/// <param name="trimChars">要移除的字符，如果不指定将移除空白字符</param>
		/// <returns>此字符串处理实例</returns>
		public STR TrimEnd(params char[] trimChars)
		{
			return this.TrimHelper(trimChars, 1);
		}

		/// <summary>移除前导和结尾字符</summary>
		/// <param name="trimChars">要移除的字符，如果不指定将移除空白字符</param>
		/// <returns>此字符串处理实例</returns>
		public STR Trim(params char[] trimChars)
		{
			return this.TrimHelper(trimChars, 2);
		}
		#endregion

		#region 获取子字符串
		/// <summary>截取指定位置到结束的内容</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <returns>新字符串处理实例</returns>
		public STR Get(int startIndex)
		{
			return this.Get(startIndex, this.__SB.Length - startIndex);
		}

		/// <summary>截取指定范围内的内容</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <param name="length">要截取的字符数</param>
		/// <returns>新字符串处理实例</returns>
		public STR Get(int startIndex, int length)
		{
			int strLen = this.__SB.Length;
			if(strLen == 0 || length <= 0) return new STR();
			if(startIndex < 0)
			{
				length += startIndex;
				startIndex = 0;
			}
			if(startIndex + length > strLen) length = strLen - startIndex;
			if(length >= strLen) return new STR(this);
			return new STR(this.__SB.ToString(startIndex, length));
		}

		/// <summary>截取指定范围内的内容</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <param name="endIndex">从 0 开始的结束位置</param>
		/// <returns>新字符串处理实例</returns>
		public STR GetPart(int startIndex, int endIndex)
		{
			return this.Get(startIndex, endIndex - startIndex + 1);
		}

		/// <summary>截取指定位置到结束的内容</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <returns>新字符串处理实例</returns>
		public STR GetPart(int startIndex)
		{
			return this.Get(startIndex);
		}

		/// <summary>截取左边指定字符数的内容</summary>
		/// <param name="len">要获取的字符数</param>
		/// <returns>新字符串处理实例</returns>
		public STR GetLeft(int len)
		{
			return this.Get(0, len);
		}

		/// <summary>截取右边指定字符数的内容</summary>
		/// <param name="len">要获取的字符数</param>
		/// <returns>新字符串处理实例</returns>
		public STR GetRight(int len)
		{
			return this.Get(this.__SB.Length - len, len);
		}
		#endregion

		#region 删除
		/// <summary>删除指定范围的字符</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <param name="length">要移除的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR Remove(int startIndex, int length)
		{
			int strLen = this.__SB.Length;
			if(strLen == 0 || length <= 0) return this;
			if(startIndex < 0)
			{
				length += startIndex;
				startIndex = 0;
			}
			if(startIndex + length > strLen) length = strLen - startIndex;
			if(length >= strLen)
				this.__SB.Clear();
			else
				this.__SB.Remove(startIndex, length);
			return this;
		}

		/// <summary>删除指定位置到结束的内容</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <returns>此字符串处理实例</returns>
		public STR Remove(int startIndex)
		{
			return this.Remove(startIndex, this.__SB.Length - startIndex);
		}

		/// <summary>删除指定范围的字符</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <param name="endIndex">结束位置</param>
		/// <returns>此字符串处理实例</returns>
		public STR RemovePart(int startIndex, int endIndex)
		{
			return this.Remove(startIndex, endIndex - startIndex + 1);
		}

		/// <summary>删除指定位置到结束的内容</summary>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <returns>此字符串处理实例</returns>
		public STR RemovePart(int startIndex)
		{
			return this.Remove(startIndex);
		}

		/// <summary>删除左边指定的字符数</summary>
		/// <param name="len">要删除的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR RemoveLeft(int len)
		{
			return this.Remove(0, len);
		}

		/// <summary>删除右边指定的字符数</summary>
		/// <param name="len">要删除的字符数</param>
		/// <returns>此字符串处理实例</returns>
		public STR RemoveRight(int len)
		{
			return this.Remove(this.__SB.Length - len, len);
		}

		/// <summary>删除左边指定的字符串</summary>
		/// <param name="str">要删除的字符串</param>
		/// <returns>此字符串处理实例</returns>
		public STR RemoveLeft(string str)
		{
			int len = this.__SB.Length;
			if(this.__SB.Length == 0 || string.IsNullOrEmpty(str)) return this;
			int strLen = str.Length;
			int index;
			for(index = 0; index < strLen && index < len; index++)
			{
				if(this.__SB[index] != str[index])
					break;
			}
			if(index < strLen) return this;
			if(index >= len) this.__SB.Clear();
			else this.__SB.Remove(0, strLen);
			return this;
		}

		/// <summary>删除右边指定的字符串</summary>
		/// <param name="str">要删除的字符串</param>
		/// <returns>此字符串处理实例</returns>
		public STR RemoveRight(string str)
		{
			int len = this.__SB.Length;
			if(this.__SB.Length == 0 || string.IsNullOrEmpty(str)) return this;
			int strLen = str.Length;
			int index;
			int startIndex = len - strLen;
			if(startIndex < 0) return this;
			for(index = 0; index < strLen && index < len; index++)
			{
				if(this.__SB[index + startIndex] != str[index])
					break;
			}
			if(index < strLen) return this;
			if(index >= len) this.__SB.Clear();
			else this.__SB.Remove(startIndex, strLen);
			return this;
		}
		#endregion

		#region 其他继承 StringBuilder 的方法
		/// <summary>将默认的行终止符追加到当前对象的末尾</summary>
		/// <returns>此字符串处理实例</returns>
		public STR AppendLine()
		{
			this.__SB.AppendLine();
			return this;
		}

		/// <summary>将指定字符串的副本和默认的行终止符追加到当前对象的末尾</summary>
		/// <param name="value">要追加的字符串</param>
		/// <returns>此字符串处理实例</returns>
		public STR AppendLine(string value)
		{
			this.__SB.AppendLine(value);
			return this;
		}

		/// <summary>将此实例的指定段中的字符复制到目标 Char 数组的指定段中</summary>
		/// <param name="sourceIndex">此实例中开始复制字符的位置。索引是从零开始的</param>
		/// <param name="destination">要将字符复制到的 Char 数组</param>
		/// <param name="destinationIndex">要将字符复制到的 destination 中的起始位置。索引是从零开始的</param>
		/// <param name="count">要复制的字符数</param>
		public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			this.__SB.CopyTo(sourceIndex, destination, destinationIndex, count);
		}

		/// <summary>确保此实例的容量至少是指定值</summary>
		/// <param name="capacity">要确保的最小容量</param>
		/// <returns>此实例的新容量</returns>
		public int EnsureCapacity(int capacity)
		{
			return this.__SB.EnsureCapacity(capacity);
		}
		#endregion

		#region 显式转换成字符串
		/// <summary>返回当前实例的内容</summary>
		/// <returns>当前实例的内容</returns>
		public override string ToString()
		{
			return this.__SB.ToString();
		}

		/// <summary>返回当前实例的内容</summary>
		/// <param name="startIndex">此实例内子字符串的起始位置</param>
		/// <param name="length">子字符串的长度</param>
		/// <returns>当前实例的内容</returns>
		public string ToString(int startIndex, int length)
		{
			return this.__SB.ToString(startIndex, length);
		}
		#endregion

		#region 隐式转换
		/// <summary>将 <see cref="string" /> 隐式转换成 <see cref="STR" /></summary>
		/// <param name="value">要转换的内容</param>
		/// <returns>新 <see cref="STR" /> 实例</returns>
		public static implicit operator STR(string value)
		{
			return new STR(value);
		}
		#endregion

		#region 重载操作符
		#region +
		#region 添加
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, bool value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, byte value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, char value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, char[] value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, decimal value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, double value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, short value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, int value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, long value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, sbyte value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, float value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, string value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, string[] value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, IEnumerable<string> value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, ushort value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, uint value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, ulong value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, STR value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, StringBuilder value)
		{
			return self.Append(value);
		}
		/// <summary>添加</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要添加的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(STR self, object value)
		{
			return self.Append(value);
		}
		#endregion

		#region 插入
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(string value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(string[] value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(IEnumerable<string> value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(char value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(bool value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(byte value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(char[] value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(decimal value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(double value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(short value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(int value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(long value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(sbyte value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(float value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(ushort value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(uint value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(ulong value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(StringBuilder value, STR self)
		{
			return self.Insert(0, value);
		}
		/// <summary>插入</summary>
		/// <param name="value">要插入的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator +(object value, STR self)
		{
			return self.Insert(0, value);
		}
		#endregion
		#endregion

		#region -
		/// <summary>删除</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要删除的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator -(STR self, string value)
		{
			return self.RemoveRight(value);
		}
		/// <summary>删除</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要删除的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator -(STR self, StringBuilder value)
		{
			return self.RemoveRight(value.ToString());
		}
		/// <summary>删除</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要删除的值</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator -(STR self, STR value)
		{
			return self.RemoveRight(value.__SB.ToString());
		}
		/// <summary>删除</summary>
		/// <param name="value">要删除的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator -(string value, STR self)
		{
			return self.RemoveLeft(value);
		}
		/// <summary>删除</summary>
		/// <param name="value">要删除的值</param>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <returns>此 <see cref="STR" /> 实例</returns>
		public static STR operator -(StringBuilder value, STR self)
		{
			return self.RemoveLeft(value.ToString());
		}
		#endregion

		#region ==
		/// <summary>比较此 <see cref="STR" /> 实例和指定的对象是否表示相同的值</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要比较的值</param>
		/// <returns>指示是否表示相同的值</returns>
		public static bool operator ==(STR self, string value)
		{
			if(Object.ReferenceEquals(self, value)) return true;
			return self.__SB.ToString().Equals(value);
		}
		/// <summary>比较此 <see cref="STR" /> 实例和指定的对象是否表示相同的值</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要比较的值</param>
		/// <returns>指示是否表示相同的值</returns>
		public static bool operator ==(STR self, StringBuilder value)
		{
			if(Object.ReferenceEquals(self, value)) return true;
			return self.__SB.ToString().Equals(value.ToString());
		}
		/// <summary>比较此 <see cref="STR" /> 实例和指定的对象是否表示相同的值</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要比较的值</param>
		/// <returns>指示是否表示相同的值</returns>
		public static bool operator ==(STR self, STR value)
		{
			if(Object.ReferenceEquals(self, value)) return true;
			return self.__SB.ToString().Equals(value.__SB.ToString());
		}
		/// <summary>比较此 <see cref="STR" /> 实例和指定的对象是否表示相同的值</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要比较的值</param>
		/// <returns>指示是否表示相同的值</returns>
		public static bool operator !=(STR self, STR value)
		{
			if(Object.ReferenceEquals(self, value)) return false;
			return !self.__SB.ToString().Equals(value.__SB.ToString());
		}
		/// <summary>比较此 <see cref="STR" /> 实例和指定的对象是否表示相同的值</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要比较的值</param>
		/// <returns>指示是否表示相同的值</returns>
		public static bool operator !=(STR self, string value)
		{
			return !self.__SB.ToString().Equals(value);
		}
		/// <summary>比较此 <see cref="STR" /> 实例和指定的对象是否表示相同的值</summary>
		/// <param name="self">此 <see cref="STR" /> 实例</param>
		/// <param name="value">要比较的值</param>
		/// <returns>指示是否表示相同的值</returns>
		public static bool operator !=(STR self, StringBuilder value)
		{
			return !self.__SB.ToString().Equals(value.ToString());
		}
		#endregion
		#endregion

		#region Object 重载
		/// <summary>返回此实例的哈希代码。</summary>
		/// <returns>此实例的 32 位有符号整数哈希代码，适用于如哈希表的哈希算法和数据结构的使用。</returns>
		public override int GetHashCode()
		{
			return this.__SB.ToString().GetHashCode();
		}

		/// <summary>确定指定的对象是否等于当前实例。</summary>
		/// <param name="obj">要与当前实例进行比较的对象。</param>
		/// <returns>如果指定的对象等于当前实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Equals(STR obj)
		{
			if(Object.ReferenceEquals(this, obj)) return true;
			return this.__SB.ToString().Equals(obj.__SB.ToString());
		}
		/// <summary>确定指定的对象是否等于当前实例。</summary>
		/// <param name="obj">要与当前实例进行比较的对象。</param>
		/// <returns>如果指定的对象等于当前实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Equals(StringBuilder obj)
		{
			if(Object.ReferenceEquals(this, obj)) return true;
			return this.__SB.ToString().Equals(obj.ToString());
		}
		/// <summary>确定指定的对象是否等于当前实例。</summary>
		/// <param name="obj">要与当前实例进行比较的对象。</param>
		/// <returns>如果指定的对象等于当前实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			var o = obj as STR;
			if(o.IsNull_()) return false;
			return this.Equals(o);
		}
		#endregion

		#region 静态方法
		/// <summary>连接字符串数组元素</summary>
		/// <param name="str">要连接的字符串</param>
		/// <returns>连接后的字符串</returns>
		public static string Concat(params string[] str)
		{
			if(Utils.IsNullOrEmpty_(str)) return string.Empty;
			return string.Concat(str);
		}
		#endregion
	}
}