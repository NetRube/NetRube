using System;

namespace NetRube.Web
{
	/// <summary>NetRube Web 客户端和远程数据获取类库</summary>
	public partial class WebGet
	{
		// 混合

		/// <summary>获取提交的变量</summary>
		/// <param name="name">变量名称</param>
		/// <returns>提交的变量</returns>
		public static string GetString(string name)
		{
			if(name.IsNullOrEmpty_()) return string.Empty;
			string _retval = GetFormString(name);
			if(!_retval.IsNullOrEmpty_()) return _retval;
			return GetQueryString(name);
		}

		/// <summary>获取提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>提交的变量并转换成整数</returns>
		public static int GetInt(string name, int defval = 0)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToInt_(defval);
		}

		/// <summary>获取提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>提交的变量并转换成整数</returns>
		public static long GetLong(string name, long defval = 0L)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToLong_(defval);
		}

		/// <summary>获取提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>提交的变量并转换成整数</returns>
		public static short GetShort(string name, short defval = 0)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToShort_(defval);
		}

		/// <summary>获取提交的变量并转换成单精度浮点数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成浮点数时的默认值</param>
		/// <returns>提交的变量并转换成单精度浮点数</returns>
		public static float GetFloat(string name, float defval = 0F)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToFloat_(defval);
		}

		/// <summary>获取提交的变量并转换成双精度浮点数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成浮点数时的默认值</param>
		/// <returns>提交的变量并转换成双精度浮点数</returns>
		public static double GetDouble(string name, double defval = 0D)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToDouble_(defval);
		}

		/// <summary>获取提交的变量并转换成高精度的十进制数（一般用于货币）</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成高精度的十进制数时的默认值</param>
		/// <returns>提交的变量并转换成高精度的十进制数（一般用于货币）</returns>
		public static decimal GetDecimal(string name, decimal defval = 0M)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToDecimal_(defval);
		}

		/// <summary>获取提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>提交的变量并转换成整数</returns>
		public static byte GetByte(string name, byte defval = 0)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToByte_(defval);
		}

		/// <summary>获取提交的变量并转换成 GUID</summary>
		/// <param name="name">变量名称</param>
		/// <returns>提交的变量并转换成 GUID</returns>
		public static Guid GetGuid(string name)
		{
			string _value = GetString(name);
			if(_value.IsNullOrEmpty_()) return Guid.Empty;
			return _value.ToGuid_();
		}

		/// <summary>获取提交的变量并转换成数组</summary>
		/// <param name="name">变量名称</param>
		/// <param name="splitOption">拆分选项</param>
		/// <returns>提交的变量并转换成数组</returns>
		public static string[] GetStringArray(string name, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
		{
			var _value = GetFormStringArray(name, splitOption);
			if(_value.IsNullOrEmpty_())
			{
				_value = GetQueryStringArray(name, splitOption);
				if(_value.IsNullOrEmpty_()) return Utils.EmptyArray<string>();
			}
			return _value;
		}

		/// <summary>获取提交的变量并转换成数字数组</summary>
		/// <param name="name">变量名称</param>
		/// <param name="splitOption">拆分选项</param>
		/// <returns>提交的变量并转换成数字数组</returns>
		public static int[] GetIntArray(string name, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
		{
			return GetStringArray(name, splitOption).ToIntArray_();
		}

		/// <summary>获取提交的变量并转换成 GUID 数组</summary>
		/// <param name="name">变量名称</param>
		/// <param name="splitOption">拆分选项</param>
		/// <returns>提交的变量并转换成 GUID 数组</returns>
		public static Guid[] GetGuidArray(string name, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
		{
			return GetStringArray(name, splitOption).ToGuidArray_();
		}
	}
}