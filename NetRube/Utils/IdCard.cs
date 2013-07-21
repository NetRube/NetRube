using System;

namespace NetRube
{
	public static partial class Utils
	{
		// 身份证号码操作

		#region 提取
		/// <summary>提取身份证号码里的生日</summary>
		/// <param name="cardNumber">要提取的身份证号码</param>
		/// <param name="defval">提取不成功时的默认值</param>
		/// <returns>身份证号码里的生日</returns>
		public static DateTime GetIdCardNumberBirthday(string cardNumber, DateTime defval)
		{
			if(!IsIdCardNumber(cardNumber)) return defval;

			int y, m, d;
			switch(cardNumber.Length)
			{
				case 15:
					y = cardNumber.Substring(6, 2).ToInt_() + 1900;
					m = cardNumber.Substring(8, 2).ToInt_();
					d = cardNumber.Substring(10, 2).ToInt_();
					break;
				case 18:
					y = cardNumber.Substring(6, 4).ToInt_();
					m = cardNumber.Substring(10, 2).ToInt_();
					d = cardNumber.Substring(12, 2).ToInt_();
					break;
				default:
					return defval;
			}
			return new DateTime(y, m, d);
		}

		/// <summary>提取身份证号码里的生日（格式为：yyyy-MM-dd）</summary>
		/// <param name="cardNumber">要提取的身份证号码</param>
		/// <param name="defval">提取不成功时的默认值（格式为：yyyy-MM-dd）</param>
		/// <returns>身份证号码里格式为：yyyy-MM-dd 的生日</returns>
		public static string GetIdCardNumberBirthday(string cardNumber, string defval)
		{
			if(!IsIdCardNumber(cardNumber)) return defval;

			string[] bday = new string[3];
			switch(cardNumber.Length)
			{
				case 15:
					bday[0] = "19" + cardNumber.Substring(6, 2);
					bday[1] = cardNumber.Substring(8, 2);
					bday[2] = cardNumber.Substring(10, 2);
					break;
				case 18:
					bday[0] = cardNumber.Substring(6, 4);
					bday[1] = cardNumber.Substring(10, 2);
					bday[2] = cardNumber.Substring(12, 2);
					break;
				default:
					return defval;
			}
			return string.Join("-", bday);
		}
		#endregion

		#region 验证
		/// <summary>验证身份证号码格式是否正确</summary>
		/// <param name="cardNumber">要验证的身份证号码</param>
		/// <param name="bday">生日</param>
		/// <param name="sex">性别。0 为女，1 为男</param>
		/// <returns>指示身份证号码格式是否正确</returns>
		public static bool IsIdCardNumber(string cardNumber, DateTime bday, int sex)
		{
			if(string.IsNullOrEmpty(cardNumber)) return false;

			string bdate = bday.ToString("yyyyMMdd");

			switch(cardNumber.Length)
			{
				case 8:
					if(!cardNumber.IsDigit_()) return false;
					if(bday.AddYears(19).CompareTo(DateTime.Now) > 0) return false;
					if(!cardNumber.Equals(bdate)) return false;
					break;
				case 15:
					if(!cardNumber.IsDigit_()) return false;
					if(!bdate.EndsWith(cardNumber.Substring(6, 6))) return false;
					if(cardNumber[14].ToString().ToInt_() % 2 != sex) return false;
					break;
				case 18:
					char last = cardNumber[17];
					if(!char.IsDigit(last) && last != 'X' && last != 'x') return false;
					if(!bdate.Equals(cardNumber.Substring(6, 8))) return false;
					if(cardNumber[16].ToString().ToInt_() % 2 != sex) return false;

					int[] w = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
					char[] c = new char[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
					int n = 0;

					for(int i = 0; i < 17; i++)
					{
						if(!char.IsDigit(cardNumber[i])) return false;
						n += cardNumber[i].ToString().ToInt_() * w[i];
					}

					if(!char.ToUpperInvariant(cardNumber[17]).Equals(c[n % 11])) return false;
					break;
				default:
					return false;
			}
			return true;
		}

		/// <summary>简单验证身份证号码是否正确</summary>
		/// <param name="cardNumber">要验证的身份证号码</param>
		/// <returns>指示身份证号码格式是否正确</returns>
		public static bool IsIdCardNumber(string cardNumber)
		{
			if(string.IsNullOrEmpty(cardNumber)) return false;

			switch(cardNumber.Length)
			{
				case 15:
					if(!cardNumber.IsDigit_()) return false;
					break;
				case 18:
					int[] w = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
					char[] c = new char[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
					int n = 0;

					for(int i = 0; i < 17; i++)
					{
						if(!char.IsDigit(cardNumber[i])) return false;
						n += cardNumber[i].ToString().ToInt_() * w[i];
					}

					if(!char.ToUpperInvariant(cardNumber[17]).Equals(c[n % 11])) return false;
					break;
				default:
					return false;
			}
			return true;
		}
		#endregion

		#region 转换
		/// <summary>15 位身份证号码转换成 18 位</summary>
		/// <param name="cardNumber">要转换的身份证号码</param>
		/// <returns>转换后的 18 位身份证号码</returns>
		public static string ToIdCardNumber18(string cardNumber)
		{
			if(!Utils.IsIdCardNumber(cardNumber)) return string.Empty;
			if(cardNumber.Length == 18) return cardNumber;

			char[] val = new char[18];
			int[] w = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
			char[] c = new char[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
			int s = 0;
			for(int i = 0, n = 0; n < 15; i++, n++)
			{
				if(i == 6)
				{
					val[6] = '1';
					s += w[6];
					val[7] = '9';
					s += 9 * w[7];
					i++;
					n--;
				}
				else
				{
					val[i] = cardNumber[n];
					s += val[i].ToString().ToInt_() * w[i];
				}
			}
			val[17] = c[s % 11];

			return new string(val);
		}

		/// <summary>18 位身份证号码转换成 15 位</summary>
		/// <param name="cardNumber">要转换的身份证号码</param>
		/// <returns>转换后的 15 位身份证号码</returns>
		public static string ToIdCardNumber15(string cardNumber)
		{
			if(!Utils.IsIdCardNumber(cardNumber)) return string.Empty;
			if(cardNumber.Length == 15) return cardNumber;

			char[] val = new char[15];
			for(int i = 0, n = 0; i < 17; i++, n++)
			{
				if(i == 6) i += 2;
				val[n] = cardNumber[i];
			}
			return new string(val);
		}
		#endregion
	}
}