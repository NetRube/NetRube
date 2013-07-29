
namespace NetRube
{
	public static partial class Utils
	{
		// 字符操作

		/// <summary>空白字符集</summary>
		public static readonly char[] WhitespaceChars = new char[] { '\t', '\n', '\v', '\f', '\r', ' ', '\x0085', '\x00a0', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '​', '\u2028', '\u2029', '　', '﻿' };

		#region 验证字符
		/// <summary>验证字符是否属于 ASCII 字符(0-127)</summary>
		/// <param name="c">要验证的字符</param>
		/// <returns>指示是否属于 ASCII 字符(0-127)</returns>
		public static bool IsAscii_(this char c)
		{
			return c <= '\x007f';
		}

		/// <summary>验证字符是否属于拉丁字符(0-255)</summary>
		/// <param name="c">要验证的字符</param>
		/// <returns>指示是否属于拉丁字符(0-255)</returns>
		public static bool IsLatin_(this char c)
		{
			return c <= '\x00ff';
		}

		/// <summary>验证字符是否属于空白字符</summary>
		/// <param name="c">要验证的字符</param>
		/// <returns>指示是否属于空白字符</returns>
		public static bool IsWhiteSpace_(this char c)
		{
			if(char.IsWhiteSpace(c)) return true;
			if(c == '﻿') return true;
			return false;
		}

		/// <summary>验证字符是否属于十六进制字符</summary>
		/// <param name="c">要验证的字符</param>
		/// <returns>指示是否属于十六进制字符</returns>
		public static bool IsHex_(this char c)
		{
			//if(char.IsDigit(c)) return true;
			//if(c.Between_('a', 'f', true)) return true;
			//if(c.Between_('A', 'F', true)) return true;
			//return false;
			return (c >= '0' && c <= '9')
				|| (c >= 'A' && c <= 'F')
				|| (c >= 'a' && c <= 'f');
		}
		#endregion
	}
}