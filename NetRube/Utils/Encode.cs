using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace NetRube
{
	public partial class Utils
	{
		// 压缩解压、编码解码操作

		#region Guid
		/// <summary>获取全局唯一标识符</summary>
		/// <returns>一个新的 GUID</returns>
		public static Guid GetGuid()
		{
			return Guid.NewGuid();
		}

		/// <summary>获取 COMB（GUID 与时间混合型）类型 GUID 数据</summary>
		/// <returns>一个新的 COMB（GUID 与时间混合型）</returns>
		public static Guid GetComb()
		{
			byte[] _guidArr = GetGuid().ToByteArray();
			DateTime _baseDate = new DateTime(1900, 1, 1);
			DateTime _now = DateTime.Now;
			TimeSpan _days = new TimeSpan(_now.Ticks - _baseDate.Ticks);
			TimeSpan _msecs = _now.TimeOfDay;
			byte[] _dayArr = BitConverter.GetBytes(_days.Days);
			long _msec = (long)(_msecs.TotalMilliseconds / 3.333333d);
			byte[] _msecsArr = BitConverter.GetBytes(_msec);

			Array.Reverse(_dayArr);
			Array.Reverse(_msecsArr);
			Array.Copy(_dayArr, _dayArr.Length - 2, _guidArr, _guidArr.Length - 6, 2);
			Array.Copy(_msecsArr, _msecsArr.Length - 4, _guidArr, _guidArr.Length - 4, 4);

			return new Guid(_guidArr);
		}

		/// <summary>获取 COMB（GUID 与时间混合型）类型里的时间</summary>
		/// <param name="comb">COMB（GUID 与时间混合型）类型 GUID 数据</param>
		/// <returns>COMB（GUID 与时间混合型）类型里的时间</returns>
		public static DateTime GetDateFromComb_(this Guid comb)
		{
			DateTime _baseDate = new DateTime(1900, 1, 1);
			if(comb.Equals(Guid.Empty)) return _baseDate;

			byte[] _daysArr = new byte[4], _msecsArr = new byte[4], _guidArr = comb.ToByteArray();

			Array.Copy(_guidArr, _guidArr.Length - 6, _daysArr, 2, 2);
			Array.Copy(_guidArr, _guidArr.Length - 4, _msecsArr, 0, 4);
			Array.Reverse(_daysArr);
			Array.Reverse(_msecsArr);

			int _days = BitConverter.ToInt32(_daysArr, 0);
			int _msecs = BitConverter.ToInt32(_msecsArr, 0);

			DateTime _date = _baseDate.AddDays(_days);
			_date = _date.AddMilliseconds(_msecs * 3.333333);

			return _date;
		}
		#endregion

		#region 压缩解压
		/// <summary>压缩数据，用 GZip 格式</summary>
		/// <param name="data">要压缩的数据</param>
		/// <returns>压缩后的数据</returns>
		public static byte[] Compress_(this byte[] data)
		{
			if(data.IsNullOrEmpty_()) return EmptyArray<byte>();

			using(MemoryStream _ms = new MemoryStream())
			{
				using(GZipStream _gzip = new GZipStream(_ms, CompressionMode.Compress))
				{
					_gzip.Write(data, 0, data.Length);
					return _ms.ToArray();
				}
			}
		}

		/// <summary>解压数据，GZip 格式</summary>
		/// <param name="data">要解压的数据</param>
		/// <returns>解压后的数据</returns>
		public static byte[] Decompress_(this byte[] data)
		{
			if(data.IsNullOrEmpty_()) return EmptyArray<byte>();

			using(MemoryStream _ms = new MemoryStream())
			{
				_ms.Write(data, 0, data.Length);
				_ms.Position = 0;
				using(GZipStream _gzip = new GZipStream(_ms, CompressionMode.Decompress))
				{
					using(MemoryStream _stream = new MemoryStream())
					{
						byte[] _buffer = new byte[1024];
						int _position;
						while(true)
						{
							_position = _gzip.Read(_buffer, 0, 1024);
							if(_position <= 0)
								break;
							else
								_stream.Write(_buffer, 0, 1024);
						}
						return _stream.ToArray();
					}
				}
			}
		}

		/// <summary>字符串压缩</summary>
		/// <param name="str">要压缩的字符串</param>
		/// <returns>压缩后的字符串</returns>
		public static string Compress_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;

			byte[] _str = Encoding.UTF8.GetBytes(str);
			byte[] _data = Compress_(_str);
			string _retval = Convert.ToBase64String(_data);
			return _retval;
		}

		/// <summary>字符串解压</summary>
		/// <param name="str">要解压的字符串</param>
		/// <returns>解压后的字符串</returns>
		public static string Decompress_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;
			if(!IsBase64String_(str)) return string.Empty;

			byte[] _str = Convert.FromBase64String(str);
			byte[] _data = Decompress_(_str);
			string _retval = Encoding.UTF8.GetString(_data);
			return _retval;
		}
		#endregion

		#region 加密解密
		/// <summary>MD5 编码</summary>
		/// <param name="data">要编码的数据</param>
		/// <returns>编码后的数据</returns>
		public static string MD5_(this byte[] data)
		{
			using(MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider())
				data = _md5.ComputeHash(data);
			string[] _str = new string[16];
			for(int i = 0, l = data.Length; i < l; i++)
				_str[i] = data[i].ToString("x2");
			return _str.Join_();
		}

		/// <summary>MD5 编码</summary>
		/// <param name="str">要编码的字符串</param>
		/// <returns>编码后的字符串</returns>
		public static string MD5_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;
			byte[] _data = Encoding.UTF8.GetBytes(str);
			return MD5_(_data);
		}

		/// <summary>字符串加密</summary>
		/// <param name="str">要加密的字符串</param>
		/// <returns>加密后的字符串</returns>
		public static string Encrypt_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;

			byte[] encode = Encoding.UTF8.GetBytes(str);
			using(var md5 = new MD5CryptoServiceProvider())
				encode = md5.ComputeHash(encode);

			string[] _str = new string[16];
			for(int i = 0, l = encode.Length, n; i < l; i++)
			{
				n = encode[i] + 100;
				if(n > 0xff) n = n - 0xff;
				_str[i] = n.ToString("x2");
			}
			return _str.Join_();
		}

		/// <summary>将 MD5 字符串转换成用 Encrypt 加密的字符串</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>转换后的字符串</returns>
		public static string MD5StringToEncrypt(string str)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			int l = str.Length;
			if(l != 32) return string.Empty;
			if(!str.IsHex_()) return string.Empty;

			string[] _str = new string[16];
			for(int i = 0, n; i < 16; i++)
			{
				n = HexToInt(str.Substring(i * 2, 2)) + 100;
				if(n > 0xff) n = n - 0xff;
				_str[i] = n.ToString("x2");
			}
			return _str.Join_();
		}

		/// <summary>将用 Encrypt 加密字符串转换成 MD5 的字符串</summary>
		/// <param name="str">要转换的字符串</param>
		/// <returns>转换后的字符串</returns>
		public static string EncryptStringToMD5(string str)
		{
			if(str.IsNullOrEmpty_()) return string.Empty;
			int l = str.Length;
			if(l != 32) return string.Empty;
			if(!str.IsHex_()) return string.Empty;

			string[] _str = new string[16];
			for(int i = 0, n; i < 16; i++)
			{
				n = HexToInt(str.Substring(i * 2, 2)) - 100;
				if(n < 0) n = n + 0xff;
				_str[i] = n.ToString("x2");
			}
			return _str.Join_();
		}

		/// <summary>转换成 Base64 字符串</summary>
		/// <param name="data">要转换的数据</param>
		/// <returns>编码后的字符串</returns>
		public static string Base64Encode_(this byte[] data)
		{
			if(data.IsNullOrEmpty_()) return string.Empty;
			return Convert.ToBase64String(data);
		}

		/// <summary>Base64 编码</summary>
		/// <param name="str">要编码的字符串</param>
		/// <returns>编码后的字符串</returns>
		public static string Base64Encode_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;
			byte[] _data = Encoding.UTF8.GetBytes(str);
			return Convert.ToBase64String(_data);
		}

		/// <summary>Base64 解码</summary>
		/// <param name="str">要解码的字符串</param>
		/// <returns>解码后的字符串</returns>
		public static string Base64Decode_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;
			byte[] _data = Convert.FromBase64String(str);
			return Encoding.UTF8.GetString(_data);
		}

		/// <summary>SHA256 编码</summary>
		/// <param name="data">要编码的数据</param>
		/// <returns>编码后的字符串</returns>
		public static string SHA256_(this byte[] data)
		{
			using(SHA256Managed _sha = new SHA256Managed())
				data = _sha.ComputeHash(data);
			string _retval = Convert.ToBase64String(data);
			return _retval;
		}

		/// <summary>SHA256 编码</summary>
		/// <param name="str">要编码的字符串</param>
		/// <returns>编码后的字符串</returns>
		public static string SHA256_(this string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;

			byte[] _data = Encoding.UTF8.GetBytes(str);
			return SHA256_(_data);
		}

		private static byte[] AESIV = new byte[16] { 88, 232, 174, 184, 120, 231, 187, 154, 105, 229, 134, 172, 68, 229, 134, 172 };

		/// <summary>AES 加密</summary>
		/// <param name="encryptString">要加密的字符串</param>
		/// <param name="encryptKey">加密的密钥</param>
		/// <returns>加密后的字符串</returns>
		public static string AESEncode_(this string encryptString, string encryptKey)
		{
			if(string.IsNullOrEmpty(encryptString)) return string.Empty;

			string _enKey = encryptKey.GetSubString_(32).PadRight(32);

			using(RijndaelManaged _rijndael = new RijndaelManaged())
			{
				_rijndael.Key = Encoding.UTF8.GetBytes(_enKey);
				_rijndael.IV = AESIV;
				return Encryptor(_rijndael, encryptString);
			}
		}

		/// <summary>AES 解密</summary>
		/// <param name="decryptString">要解密的 Base64 字符串</param>
		/// <param name="decryptKey">解密密钥</param>
		/// <returns>解密后的字符串</returns>
		public static string AESDecode_(this string decryptString, string decryptKey)
		{
			if(string.IsNullOrEmpty(decryptString)) return string.Empty;
			if(!IsBase64String_(decryptString)) return string.Empty;

			string _deKey = decryptKey.GetSubString_(32).PadRight(32);

			using(RijndaelManaged _rijndael = new RijndaelManaged())
			{
				_rijndael.Key = Encoding.UTF8.GetBytes(_deKey);
				_rijndael.IV = AESIV;
				return Decryptor(_rijndael, decryptString);
			}
		}

		private static byte[] DESIV = new byte[8] { 232, 174, 184, 120, 231, 187, 154, 88 };

		/// <summary>DES 加密</summary>
		/// <param name="encryptString">要加密的字符串</param>
		/// <param name="encryptKey">加密的密钥</param>
		/// <returns>加密后的字符串</returns>
		public static string DESEncode_(this string encryptString, string encryptKey)
		{
			if(string.IsNullOrEmpty(encryptString)) return string.Empty;

			string _enKey = encryptKey.GetSubString_(8).PadRight(8);

			using(DESCryptoServiceProvider _des = new DESCryptoServiceProvider())
			{
				_des.Key = Encoding.UTF8.GetBytes(_enKey);
				_des.IV = DESIV;
				return Encryptor(_des, encryptString);
			}
		}

		/// <summary>DES 解密</summary>
		/// <param name="decryptString">要解密的 Base64 字符串</param>
		/// <param name="decryptKey">解密密钥</param>
		/// <returns>解密后的字符串</returns>
		public static string DESDecode_(this string decryptString, string decryptKey)
		{
			if(string.IsNullOrEmpty(decryptString)) return string.Empty;
			if(!IsBase64String_(decryptString)) return string.Empty;

			string _deKey = decryptKey.GetSubString_(8).PadRight(8);

			using(DESCryptoServiceProvider _des = new DESCryptoServiceProvider())
			{
				_des.Key = Encoding.UTF8.GetBytes(_deKey);
				_des.IV = DESIV;
				return Decryptor(_des, decryptString);
			}
		}

		private static string Encryptor(SymmetricAlgorithm cryptor, string encryptString)
		{
			using(ICryptoTransform _crypt = cryptor.CreateEncryptor())
			{
				byte[] _enStr = Encoding.UTF8.GetBytes(encryptString);
				byte[] _data = _crypt.TransformFinalBlock(_enStr, 0, _enStr.Length);
				return Convert.ToBase64String(_data);
			}
		}

		private static string Decryptor(SymmetricAlgorithm cryptor, string decryptString)
		{
			try
			{
				using(ICryptoTransform _crypt = cryptor.CreateDecryptor())
				{
					byte[] _deStr = Convert.FromBase64String(decryptString);
					byte[] _data = _crypt.TransformFinalBlock(_deStr, 0, _deStr.Length);
					return Encoding.UTF8.GetString(_data);
				}
			}
			catch { return string.Empty; }
		}
		#endregion

		#region 位运算
		/// <summary>获取指定位置是否为 1</summary>
		/// <param name="b">要运算的 byte</param>
		/// <param name="index">从 0 开始的位置</param>
		/// <returns>指示指定位置是否为 1</returns>
		public static bool GetBit_(this byte b, int index)
		{
			return (b & (1 << index)) > 0;
		}

		/// <summary>将指定位置设置为 1</summary>
		/// <param name="b">要运算的 byte</param>
		/// <param name="index">从 0 开始的位置</param>
		/// <returns>运算后的 byte</returns>
		public static byte SetBit_(this byte b, int index)
		{
			b |= (byte)(1 << index);
			return b;
		}

		/// <summary>将指定位置设置为 0</summary>
		/// <param name="b">要运算的 byte</param>
		/// <param name="index">从 0 开始的位置</param>
		/// <returns>运算后的 byte</returns>
		public static byte ClearBit_(this byte b, int index)
		{
			b &= (byte)((1 << 8) - 1 - (1 << index));
			return b;
		}

		/// <summary>将指定位置取反</summary>
		/// <param name="b">要运算的 byte</param>
		/// <param name="index">从 0 开始的位置</param>
		/// <returns>运算后的 byte</returns>
		public static byte ReverseBit_(this byte b, int index)
		{
			b ^= (byte)(1 << index);
			return b;
		}
		#endregion
	}
}