using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NetRube
{
	public static partial class Utils
	{
		// 字符串操作

		/// <summary>获取指定字符串的实际长度，1 个汉字长度为 2。</summary>
		/// <param name="str">要获取的字符串</param>
		/// <returns>返回指定字符串的实际长度，1 个汉字长度为 2</returns>
		public static int GetLength_(this string str)
		{
			if(str.IsNullOrEmpty_()) return 0;
			return Encoding.Default.GetByteCount(str);
		}

		#region 重复字符串
		/// <summary>重复字符串</summary>
		/// <param name="str">要重复的字符串</param>
		/// <param name="count">重复次数</param>
		/// <returns>返回按重复次数串连起来的要重复内容的字符串</returns>
		public static string Repeat_(this string str, int count)
		{
			if(str == null || str.Length == 0 || count < 1) return string.Empty;
			if(count == 1) return str;

			return string.Join(str, NewArray<string>(count + 1));
		}
		#endregion

		#region 格式化
		/// <summary>格式化字符串</summary>
		/// <param name="str">要格式化的字符串</param>
		/// <param name="args">格式化参数</param>
		/// <returns>格式化后的字符串</returns>
		public static string F(this string str, params object[] args)
		{
			if(args.IsNullOrEmpty_()) return str;
			return string.Format(str, args);
		}
		#endregion

		#region 连接
		/// <summary>连接字符串数组</summary>
		/// <param name="str">要连接的字符串</param>
		/// <param name="args">要连接的字符串集合</param>
		/// <returns>连接的字符串数组</returns>
		public static string[] C(this string str, params string[] args)
		{
			return str.Concat_(args).ToArray();
		}
		#endregion

		#region 拆分字符串
		/// <summary>拆分字符串</summary>
		/// <param name="str">要拆分的字符串</param>
		/// <param name="split">用于分隔的字符串</param>
		/// <param name="splitOption">拆分选项</param>
		/// <returns>拆分后的字符串数组</returns>
		public static string[] Split_(this string str, string split, StringSplitOptions splitOption = StringSplitOptions.None)
		{
			if(str.IsNull_()) return EmptyArray<string>();
			if(splitOption == StringSplitOptions.RemoveEmptyEntries && str.IsEmpty_()) return EmptyArray<string>();
			if(split.IsNullOrEmpty_() || !str.Contains(split)) return GetArray(str);
			if(split.Length == 1)
				return str.Split(GetArray(split[0]), splitOption);
			return str.Split(GetArray(split), splitOption);
		}

		/// <summary>以给出的正则表达式拆分字符串</summary>
		/// <param name="str">用于拆分的字符串</param>
		/// <param name="pattern">正则表达式。可以使用内联 (?imnsx-imnsx:) 分组构造或 (?imnsx-imnsx) 其他构造设置选项，一个选项或一组选项前面的减号 (-) 用于关闭这些选项。例如，内联构造 (?ix-ms)
		/// <para>* i 指定不区分大小写的匹配</para>
		/// <para>* m 指定多行模式。更改 ^ 和 $ 的含义，以使它们分别与任何行的开头和结尾匹配，而不只是与整个字符串的开头和结尾匹配</para>
		/// <para>* n 指定唯一有效的捕获是显式命名或编号的 (?&lt;name&gt;…) 形式的组。这允许圆括号充当非捕获组，从而避免了由 (?:…) 导致的语法上的笨拙</para>
		/// <para>* s 指定单行模式。更改句点字符 (.) 的含义，以使它与每个字符（而不是除 \n 之外的所有字符）匹配</para>
		/// <para>* x 指定从模式中排除非转义空白并启用数字符号 (#) 后面的注释</para></param>
		/// <param name="ignoreCase">是否忽略大小写</param>
		/// <returns>拆分后的字符串数组</returns>
		public static string[] Split_(this string str, string pattern, bool ignoreCase)
		{
			if(str.IsNullOrEmpty_()) return EmptyArray<string>();
			if(pattern.IsNullOrEmpty_()) return GetArray(str);
			return Regex.Split(str, pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
		}
		#endregion

		#region 截取字符串
		/// <summary>截取指定字节数的字符串</summary>
		/// <param name="srcString">要截取的字符串</param>
		/// <param name="length">要截取的长度，1 个汉字长度为 2，所以汉字的话长度要乘 2</param>
		/// <param name="tailString">结尾附加的内容，如“...”，长度会自动减去该附加内容的长度</param>
		/// <returns>返回截取后的字符串</returns>
		public static string GetSubString_(this string srcString, int length, string tailString = null)
		{
			if(length < 1 || srcString.IsNullOrEmpty_()) return srcString;

			int tlen = tailString.GetLength_();
			length -= tlen;
			int i = 0;
			for(int len = srcString.Length, end = 0; i < len; i++)
			{
				end += (srcString[i] > '\x007f') ? 2 : 1;
				if(end >= length) break;
			}
			return srcString.Substring(0, i) + tailString ?? string.Empty;
		}

		/// <summary>截取指定中文文字长度的字符串</summary>
		/// <param name="srcString">要截取的字符串</param>
		/// <param name="length">要截取的长度，2 个英文及半角符号长度为 1</param>
		/// <param name="tailString">结尾附加的内容，如“...”，长度会自动减去该附加内容的长度</param>
		/// <returns>返回截取后的字符串</returns>
		public static string GetSubChineseString_(this string srcString, int length, string tailString = null)
		{
			return srcString.GetSubString_(length * 2, tailString);
		}

		/// <summary>获取指定字符串范围内的字符串</summary>
		/// <param name="str">用于获取的字符串</param>
		/// <param name="startString">标记为开始的字符串</param>
		/// <param name="endString">标记为结束的字符串</param>
		/// <param name="include">是否包含标记的字符串</param>
		/// <returns>返回指定字符串范围内的字符串</returns>
		public static string GetSubstr_(this string str, string startString, string endString, bool include = false)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			if(startString.IsNullOrEmpty_() || endString.IsNullOrEmpty_()) return string.Empty;

			int start = str.IndexOf(startString);
			if(start < 0) return string.Empty;
			if(!include) start += startString.Length;
			int end = str.IndexOf(endString, start);
			if(end < 0) return string.Empty;
			if(include) end += endString.Length;
			return str.Substring(start, end - start);
		}

		/// <summary>获取指定范围内的字符串</summary>
		/// <param name="str">用于获取的字符串</param>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <param name="endIndex">结束位置</param>
		/// <returns>返回指定字符串范围内的字符串</returns>
		public static string GetSubstr_(this string str, int startIndex, int endIndex = 0)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			startIndex = startIndex.NotLessThan_(0);
			if(endIndex == 0) return str.Substring(startIndex);

			int len = str.Length;
			if(endIndex < 0) endIndex = len + endIndex;
			endIndex++;
			endIndex = endIndex.NotGreaterThan_(len);
			if(startIndex > endIndex) return string.Empty;
			if(startIndex == 0 && endIndex == len) return str;
			return str.Substring(startIndex, len - endIndex);
		}

		/// <summary>获取左边指定字符数的字符串</summary>
		/// <param name="str">用于获取的字符串</param>
		/// <param name="len">要获取的字符数</param>
		/// <returns>返回左边指定字符数的字符串</returns>
		public static string GetLeft_(this string str, int len)
		{
			if(str.IsNullOrEmpty_() || len <= 0) return string.Empty;
			if(len >= str.Length) return str;
			return str.Substring(0, len);
		}

		/// <summary>获取左边指定字符串前的字符串</summary>
		/// <param name="str">用于获取的字符串</param>
		/// <param name="endString">标记为结束的字符串</param>
		/// <param name="include">是否包含标记的字符串</param>
		/// <returns>返回左边指定字符数的字符串</returns>
		public static string GetLeft_(this string str, string endString, bool include = false)
		{
			if(str.IsNullOrEmpty_() || endString.IsNullOrEmpty_()) return string.Empty;
			int n = str.IndexOf(endString);
			if(n < 0) return string.Empty;
			if(include) n += endString.Length;
			return str.Substring(0, n);
		}

		/// <summary>获取右边指定字符数的字符串</summary>
		/// <param name="str">用于获取的字符串</param>
		/// <param name="len">要获取的字符数</param>
		/// <returns>返回右边指定字符数的字符串</returns>
		public static string GetRight_(this string str, int len)
		{
			if(str.IsNullOrEmpty_() || len <= 0) return string.Empty;
			int _strLen = str.Length;
			if(len >= _strLen) return str;
			return str.Substring(_strLen - len);
		}

		/// <summary>获取右边指定字符串后的字符串</summary>
		/// <param name="str">用于获取的字符串</param>
		/// <param name="startString">标记为开始的字符串</param>
		/// <param name="include">是否包含标记的字符串</param>
		/// <returns>返回右边指定字符数的字符串</returns>
		public static string GetRight_(this string str, string startString, bool include = false)
		{
			if(str.IsNullOrEmpty_() || startString.IsNullOrEmpty_()) return string.Empty;
			int n = str.IndexOf(startString);
			if(n < 0) return string.Empty;
			if(!include) n += startString.Length;
			return str.Substring(n);
		}

		/// <summary>以给出的正则表达式获取匹配的字符串</summary>
		/// <param name="str">用于获取的字符串</param>
		/// <param name="pattern">正则表达式。可以使用内联 (?imnsx-imnsx:) 分组构造或 (?imnsx-imnsx) 其他构造设置选项，一个选项或一组选项前面的减号 (-) 用于关闭这些选项。例如，内联构造 (?ix-ms)
		/// <para>* i 指定不区分大小写的匹配</para>
		/// <para>* m 指定多行模式。更改 ^ 和 $ 的含义，以使它们分别与任何行的开头和结尾匹配，而不只是与整个字符串的开头和结尾匹配</para>
		/// <para>* n 指定唯一有效的捕获是显式命名或编号的 (?&lt;name&gt;…) 形式的组。这允许圆括号充当非捕获组，从而避免了由 (?:…) 导致的语法上的笨拙</para>
		/// <para>* s 指定单行模式。更改句点字符 (.) 的含义，以使它与每个字符（而不是除 \n 之外的所有字符）匹配</para>
		/// <para>* x 指定从模式中排除非转义空白并启用数字符号 (#) 后面的注释</para></param>
		/// <param name="ignoreCase">是否忽略大小写</param>
		/// <returns>返回匹配的字符串</returns>
		public static string GetMatch_(this string str, string pattern, bool ignoreCase = false)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			if(pattern.IsNullOrEmpty_()) return str;
			return Regex.Match(str, pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None).Value;
		}
		#endregion

		#region 删除字符串
		/// <summary>删除指定范围的字符</summary>
		/// <param name="str">用于删除的字符串</param>
		/// <param name="startIndex">从 0 开始的位置</param>
		/// <param name="endIndex">结束位置</param>
		/// <returns>返回删除后的字符串</returns>
		public static string RemovePart_(this string str, int startIndex, int endIndex = 0)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			startIndex = startIndex.NotLessThan_(0);
			if(endIndex == 0) return str.Remove(startIndex);

			int len = str.Length;
			if(endIndex < 0) endIndex = len + endIndex;
			endIndex++;
			endIndex = endIndex.NotGreaterThan_(len);
			if(startIndex > endIndex) return string.Empty;
			if(startIndex == 0 && endIndex == len) return string.Empty;
			return str.Remove(startIndex, endIndex - startIndex);
		}

		/// <summary>删除左边指定的字符数</summary>
		/// <param name="str">用于删除的字符串</param>
		/// <param name="len">要删除的字符数</param>
		/// <returns>返回删除后的字符串</returns>
		public static string RemoveLeft_(this string str, int len)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			if(len <= 0) return str;
			if(len >= str.Length) return string.Empty;
			return str.Remove(0, len);
		}

		/// <summary>删除右边指定的字符数</summary>
		/// <param name="str">用于删除的字符串</param>
		/// <param name="len">要删除的字符数</param>
		/// <returns>返回删除后的字符串</returns>
		public static string RemoveRight_(this string str, int len)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			if(len <= 0) return str;
			int _strLen = str.Length;
			if(len >= _strLen) return string.Empty;
			return str.Remove(_strLen - len);
		}

		/// <summary>删除左边指定的字符串</summary>
		/// <param name="srcString">用于删除的字符串</param>
		/// <param name="str">要删除的字符串</param>
		/// <returns>返回删除后的字符串</returns>
		public static string RemoveLeft_(this string srcString, string str)
		{
			if(srcString.IsNullOrEmpty_()) return string.Empty;
			if(str.IsNullOrEmpty_()) return srcString;
			if(!srcString.StartsWith(str)) return srcString;
			return srcString.Remove(0, str.Length);
		}

		/// <summary>删除右边指定的字符串</summary>
		/// <param name="srcString">用于删除的字符串</param>
		/// <param name="str">要删除的字符串</param>
		/// <returns>返回删除后的字符串</returns>
		public static string RemoveRight_(this string srcString, string str)
		{
			if(srcString.IsNullOrEmpty_()) return string.Empty;
			if(str.IsNullOrEmpty_()) return srcString;
			if(!srcString.EndsWith(str)) return srcString;
			return srcString.Remove(srcString.Length - str.Length);
		}
		#endregion

		#region 全角半角转换
		/// <summary>将全角字符转换为半角字符</summary>
		/// <param name="c">要转换的字符</param>
		/// <param name="ignoreSpace">是否忽略对空格的转换</param>
		/// <returns>返回转换后的字符串</returns>
		public static char ToDBC_(this char c, bool ignoreSpace = false)
		{
			int _n = c;
			if(_n == 12288 && !ignoreSpace)
				return '\x0050';
			if(_n.Between_(65281, 65374))
				return (char)(_n - 65248);
			return c;
		}

		/// <summary>将字符串中的全角字符转换为半角字符</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="ignoreSpace">是否忽略对空格的转换</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToDBC_(this string str, bool ignoreSpace = false)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;
			char[] _str = str.ToCharArray();
			int _len = _str.Length;
			for(int i = 0; i < _len; i++)
				_str[i] = _str[i].ToDBC_(ignoreSpace);
			return new string(_str);
		}

		/// <summary>将半角字符转换为全角字符</summary>
		/// <param name="c">要转换的字符</param>
		/// <param name="ignoreSpace">是否忽略对空格的转换</param>
		/// <returns>返回转换后的字符串</returns>
		public static char ToSBC_(this char c, bool ignoreSpace = false)
		{
			int _n = c;
			if(_n == 32 && !ignoreSpace)
				return '\x3000';
			if(_n.Between_(33, 126))
				return (char)(_n + 65248);
			return c;
		}

		/// <summary>将字符串中的半角字符转换为全角字符</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="ignoreSpace">是否忽略对空格的转换</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToSBC_(this string str, bool ignoreSpace = false)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;
			char[] _str = str.ToCharArray();
			int _len = _str.Length;
			for(int i = 0; i < _len; i++)
				_str[i] = _str[i].ToSBC_(ignoreSpace);
			return new string(_str);
		}
		#endregion

		#region 中文大写金额转换
		/// <summary>转换成中文大写金额</summary>
		/// <param name="num">要转换的数字</param>
		/// <returns>中文大写金额</returns>
		public static string ToRMB_(this decimal num)
		{
			return ToRMB_(num, Localization.Resources.CurrencyDigital, Localization.Resources.CurrencyUnit, Localization.Resources.CurrencyOutOfRange);
		}

		private static string ToRMB_(decimal num, string digits, string units, string err)
		{
			string _str = num.ToString("F2");
			STR _s = new STR();
			if(_str[0] == '-')
			{
				_s += units[15];
				_str = _str.TrimStart('-');
			}

			string[] _parts = _str.Split('.');

			int _intLen = _parts[0].Length;
			if(_intLen > 12)
				return err;
			bool _int0 = _parts[0] == "0";
			bool _dec0 = _parts[1] == "00";

			int i, j, x, n = 0;
			bool _0 = false, _j0 = false;

			if(_int0 && _dec0)
			{
				_s += digits[0];
				_s += units[1];
			}
			else if(!_int0)
			{
				// 处理整数部分
				for(i = 0; i < _intLen; i++)
				{
					n = Convert.ToInt32(_parts[0][i]) - 48;
					x = _intLen - i;
					// j = 0 时为段尾
					j = (x - 1) & 0x3;

					// 处理 0
					if(n == 0)
					{
						_0 = true;
						// 处理为段尾的 0
						if(j == 0)
						{
							if(_intLen == 1 && _dec0)
								_s += digits[0];
							if(_j0 || x == 1)
								_s += units[x];
							_j0 = false;
						}
					}
					// 处理非 0
					else
					{
						if(_0)
						{
							_s += digits[0];
							_0 = false;
						}
						_s += digits[n];
						_s += units[x];

						// 如果是段尾,设置非 0 标志
						_j0 = j == 0 ? false : true;
					}
				}
			}

			// 处理小数部分
			// 如果没有小数,添加"整"字符
			if(_dec0)
			{
				_s += units[0];
			}
			else
			{
				int g = n;

				// 角
				n = Convert.ToInt32(_parts[1][0]) - 48;
				if(n != 0)
				{
					_s += digits[n];
					_s += units[13];
				}
				else if(_intLen != 1 || g != 0)
					_s += digits[0];

				// 分
				n = Convert.ToInt32(_parts[1][1]) - 48;
				if(n != 0)
				{
					_s += digits[n];
					_s += units[14];
				}
				else
					_s += units[0];
			}

			return _s.ToString();
		}
		#endregion

		#region 文本转换
		/// <summary>转换成首字母大写（每个单词的首字母大写）</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToPascalCase_(this string str)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			char[] _cs = str.ToCharArray();
			int _len = _cs.Length;
			if(char.IsLower(_cs[0]))
				_cs[0] = char.ToUpperInvariant(_cs[0]);
			for(int i = 1; i < _len; i++)
			{
				if(!char.IsLetter(_cs[i]))
				{
					i++;
					if(char.IsLower(_cs[i]))
						_cs[i] = char.ToUpperInvariant(_cs[i]);
				}
				else
				{
					if(char.IsUpper(_cs[i]))
						_cs[i] = char.ToLowerInvariant(_cs[i]);
				}
			}
			return new string(_cs, 0, _len);
		}

		/// <summary>转换成首字母大写（每个单词的首字母大写）</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="separators">字符串中单词间的分隔符</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToPascalCase_(this string str, params char[] separators)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			if(separators.IsNullOrEmpty_()) return str;
			char[] _cs = str.ToCharArray();
			int _len = _cs.Length;
			if(char.IsLower(_cs[0]))
				_cs[0] = char.ToUpperInvariant(_cs[0]);
			for(int i = 1; i < _len; i++)
			{
				if(_cs[i].In_(separators))
				{
					i++;
					if(char.IsLower(_cs[i]))
						_cs[i] = char.ToUpperInvariant(_cs[i]);
				}
				else
				{
					if(char.IsUpper(_cs[i]))
						_cs[i] = char.ToLowerInvariant(_cs[i]);
				}
			}
			return new string(_cs, 0, _len);
		}

		/// <summary>转换成驼峰格式（首个单词的首字母小写，其余单词的首字母大写）</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="separators">要清除的分隔符</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToCamelCase_(this string str, params char[] separators)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			if(separators.IsNullOrEmpty_()) return str;
			char[] _cs = NewArray<char>(str.Length);
			int _index = 0;
			bool _ws = false;
			foreach(char c in str)
			{
				if(c.In_(separators))
				{
					_ws = true;
					continue;
				}
				else
				{
					if(_ws)
					{
						_cs[_index] = char.IsLower(c) ? char.ToUpperInvariant(c) : c;
						_ws = false;
					}
					else
					{
						_cs[_index] = char.IsUpper(c) ? char.ToLowerInvariant(c) : c;
					}
					_index++;
				}
			}
			return new string(_cs, 0, _index);
		}

		/// <summary>将连字符分隔转换成驼峰格式（首个单词的首字母小写，其余单词的首字母大写）</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToCamelCase_(this string str)
		{
			return str.ToCamelCase_('-');
		}

		/// <summary>将驼峰格式转换成以指定字符分隔</summary>
		/// <param name="str">要转换的字符串</param>
		/// <param name="separator">分隔符</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToSeparate_(this string str, string separator = "-")
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			char[] _cs = str.ToCharArray();
			int _len = _cs.Length;
			char _c;
			STR _str = new STR((int)Math.Ceiling(_len * 1.5));
			_str += _cs[0];
			for(int i = 1; i < _len; i++)
			{
				_c = _cs[i];
				if(char.IsUpper(_c))
					_str += separator + char.ToLowerInvariant(_c);
				else
					_str += _c;
			}
			return _str.ToString();
		}

		/// <summary>将驼峰格式转换成以连字符分隔</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ToHyphenate_(this string str)
		{
			return str.ToSeparate_("-");
		}

		/// <summary>转换文本格式</summary>
		/// <param name="text">要转换的文本</param>
		/// <param name="type">要转换的类型</param>
		/// <returns>返回转换后的字符串</returns>
		public static string ConvertTextCase_(this string text, TextCaseType type)
		{
			if(text.IsNullOrEmpty_()) return string.Empty;
			switch(type)
			{
				case TextCaseType.LowerCase:
					return text.ToLowerInvariant();
				case TextCaseType.UpperCase:
					return text.ToUpperInvariant();
				case TextCaseType.PascalCase:
					return text.ToPascalCase_();
				case TextCaseType.CamelCase:
					return text.ToCamelCase_();
				case TextCaseType.Hyphenate:
					return text.ToHyphenate_();
				default:
					return text;
			}
		}

		/// <summary>清除多余空白</summary>
		/// <param name="str">要清除的字符串</param>
		/// <returns>清除后的字符串</returns>
		public static string Clean_(this string str)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;

			string _str = str.Trim();
			char[] _cs = NewArray<char>(_str.Length);
			int _index = 0;
			bool _ws = false;
			foreach(char c in _str)
			{
				if(c.IsWhiteSpace_())
				{
					_ws = true;
					continue;
				}
				else
				{
					if(_ws)
					{
						_cs[_index] = ' ';
						_ws = false;
						_index++;
					}
					_cs[_index] = c;
					_index++;
				}
			}
			return new string(_cs, 0, _index);
		}

		/// <summary>格式化时间量</summary>
		/// <param name="num">毫秒数</param>
		/// <returns>返回格式化后的时间量</returns>
		public static string FormatTime(double num)
		{
			TimeSpan ts = TimeSpan.FromMilliseconds(num);
			return ts.ToString_();
		}

		/// <summary>转换 XML 转义符</summary>
		/// <param name="xml">要转换的 XML 文本</param>
		/// <returns>返回转换后的字符串</returns>
		public static string EscapeXml_(this string xml)
		{
			if(xml.IsNullOrEmpty_()) return string.Empty;
			StringBuilder _sb = new StringBuilder(xml, Math.Ceiling(xml.Length * 1.5).ToInt_());
			if(xml.IndexOf('&') > -1)
				_sb.Replace("&", "&amp;");
			if(xml.IndexOf('\'') > -1)
				_sb.Replace("'", "&apos;");
			if(xml.IndexOf('"') > -1)
				_sb.Replace("\"", "&quot;");
			if(xml.IndexOf('<') > -1)
				_sb.Replace("<", "&lt;");
			if(xml.IndexOf('>') > -1)
				_sb.Replace(">", "&rt;");
			return _sb.ToString();
		}

		/// <summary>以给出的正则表达式替换字符串</summary>
		/// <param name="str">要替换的字符串</param>
		/// <param name="pattern">正则表达式。可以使用内联 (?imnsx-imnsx:) 分组构造或 (?imnsx-imnsx) 其他构造设置选项，一个选项或一组选项前面的减号 (-) 用于关闭这些选项。例如，内联构造 (?ix-ms)
		/// <para>* i 指定不区分大小写的匹配</para>
		/// <para>* m 指定多行模式。更改 ^ 和 $ 的含义，以使它们分别与任何行的开头和结尾匹配，而不只是与整个字符串的开头和结尾匹配</para>
		/// <para>* n 指定唯一有效的捕获是显式命名或编号的 (?&lt;name&gt;…) 形式的组。这允许圆括号充当非捕获组，从而避免了由 (?:…) 导致的语法上的笨拙</para>
		/// <para>* s 指定单行模式。更改句点字符 (.) 的含义，以使它与每个字符（而不是除 \n 之外的所有字符）匹配</para>
		/// <para>* x 指定从模式中排除非转义空白并启用数字符号 (#) 后面的注释</para></param>
		/// <param name="replace">替换字符串</param>
		/// <param name="ignoreCase">是否忽略大小写</param>
		/// <returns>返回替换后的字符串</returns>
		public static string Replace_(this string str, string pattern, string replace, bool ignoreCase = false)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			if(pattern.IsNullOrEmpty_()) return str;
			return Regex.Replace(str, pattern, replace, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
		}
		#endregion

		#region 验证测试
		/// <summary>验证是否是纯数字字符串</summary>
		/// <param name="str">要验证的字符串</param>
		/// <returns>指示是否为纯数字字符串</returns>
		public static bool IsDigit_(this string str)
		{
			if(str.IsNullOrEmpty_()) return false;
			foreach(char c in str)
				if(!char.IsDigit(c)) return false;
			return true;
		}

		/// <summary>验证是否是数字字符串</summary>
		/// <param name="str">要验证的字符串</param>
		/// <returns>指示是否为数字字符串</returns>
		public static bool IsNumber_(this string str)
		{
			if(str.IsNullOrEmpty_()) return false;
			double num;
			return double.TryParse(str, NumberStyles.Any, null, out num);
		}

		/// <summary>验证是否是整数字符串</summary>
		/// <param name="str">要验证的字符串</param>
		/// <returns>指示是否为整数字符串</returns>
		public static bool IsInt_(this string str)
		{
			if(str.IsNullOrEmpty_()) return false;
			long num;
			return long.TryParse(str, NumberStyles.Integer, null, out num);
		}

		/// <summary>验证是否是 ASCII(0-127)字符串</summary>
		/// <param name="str">要验证的字符</param>
		/// <returns>指示是否为 ASCII(0-127)字符串</returns>
		public static bool IsAscii_(this string str)
		{
			if(str.IsNullOrEmpty_()) return false;
			return str.All(c => c.IsAscii_());
		}

		/// <summary>验证是否是拉丁(0-255)字符串</summary>
		/// <param name="str">要验证的字符</param>
		/// <returns>指示是否为拉丁(0-255)字符串</returns>
		public static bool IsLatin_(this string str)
		{
			if(str.IsNullOrEmpty_()) return false;
			return str.All(c => c.IsLatin_());
		}

		/// <summary>验证是否是空白字符串</summary>
		/// <param name="str">要验证的字符</param>
		/// <returns>指示是否为空白字符串</returns>
		public static bool IsWhiteSpace_(this string str)
		{
			return str.All(c => c.IsWhiteSpace_());
		}

		/// <summary>验证是否是十六进制字符串</summary>
		/// <param name="str">要验证的字符</param>
		/// <returns>指示是否为十六进制字符串</returns>
		public static bool IsHex_(this string str)
		{
			return str.All(c => c.IsHex_());
		}

		/// <summary>验证是否是 Guid 字符串</summary>
		/// <param name="str">要验证的字符串</param>
		/// <returns>指示是否为 Guid 字符串</returns>
		public static bool IsGuid_(this string str)
		{
			if(str.IsNullOrEmpty_()) return false;
			Guid g;
			return Guid.TryParse(str, out g);
		}

		/// <summary>验证是否是 Base64 字符串</summary>
		/// <param name="str">要验证的字符串</param>
		/// <returns>指示是否为 Base64 字符串</returns>
		public static bool IsBase64String_(this string str)
		{
			if(str.IsNullOrEmpty_()) return false;
			//return Regex.IsMatch(str, @"^[A-Za-z0-9\+\/\=]$");
			foreach(char c in str)
			{
				if(char.IsLetterOrDigit(c)) continue;
				if(c == '+' || c == '/' || c == '=') continue;
				return false;
			}
			return true;
		}

		/// <summary>以给出的正则表达式验证匹配字符串</summary>
		/// <param name="str">要验证的字符串</param>
		/// <param name="pattern">正则表达式。可以使用内联 (?imnsx-imnsx:) 分组构造或 (?imnsx-imnsx) 其他构造设置选项，一个选项或一组选项前面的减号 (-) 用于关闭这些选项。例如，内联构造 (?ix-ms)
		/// <para>* i 指定不区分大小写的匹配</para>
		/// <para>* m 指定多行模式。更改 ^ 和 $ 的含义，以使它们分别与任何行的开头和结尾匹配，而不只是与整个字符串的开头和结尾匹配</para>
		/// <para>* n 指定唯一有效的捕获是显式命名或编号的 (?&lt;name&gt;…) 形式的组。这允许圆括号充当非捕获组，从而避免了由 (?:…) 导致的语法上的笨拙</para>
		/// <para>* s 指定单行模式。更改句点字符 (.) 的含义，以使它与每个字符（而不是除 \n 之外的所有字符）匹配</para>
		/// <para>* x 指定从模式中排除非转义空白并启用数字符号 (#) 后面的注释</para></param>
		/// <param name="ignoreCase">是否忽略大小写</param>
		/// <returns>指示是否为匹配的字符串</returns>
		public static bool IsMatch_(this string str, string pattern, bool ignoreCase = false)
		{
			if(str.IsNullOrEmpty_() || pattern.IsNullOrEmpty_()) return false;
			return Regex.IsMatch(str, pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
		}

		private static Regex REG_EMAIL = new Regex(@"^((\w+[-\.\+]*)*\w+)+@((\w+-*)*\w+\.){1,2}\w{2,4}$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		/// <summary>验证 Email 地址格式是否正确</summary>
		/// <param name="email">要验证的 Email 地址</param>
		/// <returns>指示是否为 Email 地址格式</returns>
		public static bool IsEmail(string email)
		{
			if(email.IsNullOrEmpty_()) return false;
			return REG_EMAIL.IsMatch(email);
		}

		private static Regex REG_URL = new Regex(@"^(http|https|ftp)\:\/\/[a-z0-9\-\.]+\.[a-z]{2,3}(?::[a-z0-9]*)?\/?\S*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		/// <summary>验证 URL 格式是否正确</summary>
		/// <param name="url">要验证的 URL</param>
		/// <returns>指示是否为 URL 格式</returns>
		public static bool IsUrl(string url)
		{
			if(url.IsNullOrEmpty_()) return false;
			return REG_URL.IsMatch(url);
		}

		/// <summary>验证是否是图像格式文件名</summary>
		/// <param name="fileName">要验证的文件名</param>
		/// <returns>指示是否为图像格式文件名</returns>
		public static bool IsImageFileName(string fileName)
		{
			if(fileName.IsNullOrEmpty_()) return false;
			string[] _exts = new string[5] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
			return CheckFileNameByExtension(fileName, _exts);
		}

		/// <summary>验证是否是以某些指定扩展名为结尾的文件名</summary>
		/// <param name="fileName">要验证的文件名</param>
		/// <param name="exts">用于验证的扩展名列表</param>
		/// <returns>指示是否为以某些指定扩展名结尾的文件名</returns>
		public static bool CheckFileNameByExtension(string fileName, string[] exts)
		{
			if(fileName.IsNullOrEmpty_()) return false;
			if(!fileName.Contains(".")) return false;

			string _filename = fileName.ToLowerInvariant();
			foreach(string _ext in exts)
				if(_filename.EndsWith(_ext)) return true;

			return false;
		}
		#endregion

		#region IP
		/// <summary>检测 IP 是否位于 IP 列表中</summary>
		/// <param name="ip">要检测的 IP</param>
		/// <param name="ipList">IP 列表</param>
		/// <returns>指示是否位于 IP 列表中</returns>
		public static bool InIpList(string ip, string[] ipList)
		{
			if(ip.IsNullOrEmpty_() || ipList.IsNullOrEmpty_()) return false;

			string[] _srcIp = ip.Split('.');
			int _srcIpLen = _srcIp.Length;
			string[] _ip;
			int _n = 0;
			int _ipListLen = ipList.Length, _ipLen;
			for(int _i = 0; _i < _ipListLen; _i++)
			{
				_ip = ipList[_i].Split('.');
				_n = 0;
				_ipLen = _ip.Length;
				for(int _j = 0; _j < _ipLen; _j++)
				{
					if(_ip[_j] == "*") return true;
					if(_srcIpLen <= _j) break;
					if(_ip[_j] != _srcIp[_j]) break;

					_n++;
				}

				if(_n == 4) return true;
			}

			return false;
		}

		/// <summary>验证 IP 地址格式是否正确</summary>
		/// <param name="ip">要验证的 IP 地址</param>
		/// <param name="allowWildcard">是否允许以星号（*）作为某一段的通配符</param>
		/// <returns>指示是否为 IP 地址格式</returns>
		public static bool IsIp(string ip, bool allowWildcard = false)
		{
			if(string.IsNullOrEmpty(ip)) return false;
			string[] _ips = ip.Split('.');
			if(_ips.Length != 4) return false;

			int _n;
			for(int _i = 3; _i >= 0; _i--)
			{
				if(_ips[_i] == "*")
				{
					if(allowWildcard)
						continue;
					else
						return false;
				}
				else
				{
					_n = ToInt_(_ips[_i], 256);
					if(_n == 256) return false;
					if(_n >= 0 && _n < 256)
						continue;
					else
						return false;
				}
			}
			return true;
		}
		#endregion
	}
}