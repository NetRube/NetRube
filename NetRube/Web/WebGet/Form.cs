using System;
using System.Web;

namespace NetRube.Web
{
	/// <summary>NetRube Web 客户端和远程数据获取类库</summary>
	public partial class WebGet
	{
		// 表单

		/// <summary>判断当前页面是否接收到了 POST 请求</summary>
		/// <value>如果为 POST 请求，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public static bool IsPost
		{
			get { return HttpContext.Current.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase); }
		}

		/// <summary>获取表单提交的变量数量</summary>
		/// <value>表单提交的变量数量</value>
		public static int FormCount
		{
			get { return HttpContext.Current.Request.Form.Count; }
		}

		/// <summary>获取表单提交的变量</summary>
		/// <param name="name">变量名称</param>
		/// <returns>表单提交的变量值</returns>
		public static string GetFormString(string name)
		{
			if(name.IsNullOrEmpty_()) return string.Empty;
			string _retval = HttpContext.Current.Request.Form.Get(name);
			return _retval ?? string.Empty;
		}

		/// <summary>获取表单提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>表单提交的变量并转换成整数</returns>
		public static int GetFormInt(string name, int defval = 0)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToInt_(defval);
		}

		/// <summary>获取表单提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>表单提交的变量并转换成整数</returns>
		public static long GetFormLong(string name, long defval = 0L)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToLong_(defval);
		}

		/// <summary>获取表单提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>表单提交的变量并转换成整数</returns>
		public static short GetFormShort(string name, short defval = 0)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToShort_(defval);
		}

		/// <summary>获取表单提交的变量并转换成单精度浮点数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成浮点数时的默认值</param>
		/// <returns>表单提交的变量并转换成单精度浮点数</returns>
		public static float GetFormFloat(string name, float defval = 0F)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToFloat_(defval);
		}

		/// <summary>获取表单提交的变量并转换成双精度浮点数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成浮点数时的默认值</param>
		/// <returns>表单提交的变量并转换成双精度浮点数</returns>
		public static double GetFormDouble(string name, double defval = 0D)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToDouble_(defval);
		}

		/// <summary>获取表单提交的变量并转换成高精度的十进制数（一般用于货币）</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成高精度的十进制数时的默认值</param>
		/// <returns>表单提交的变量并转换成高精度的十进制数（一般用于货币）</returns>
		public static decimal GetFormDecimal(string name, decimal defval = 0M)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToDecimal_(defval);
		}

		/// <summary>获取表单提交的变量并转换成整数</summary>
		/// <param name="name">变量名称</param>
		/// <param name="defval">无法转换成整数时的默认值</param>
		/// <returns>表单提交的变量并转换成整数</returns>
		public static byte GetFormByte(string name, byte defval = 0)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return defval;
			return _value.ToByte_(defval);
		}

		/// <summary>获取表单提交的变量并转换成 GUID</summary>
		/// <param name="name">变量名称</param>
		/// <returns>表单提交的变量并转换成 GUID</returns>
		public static Guid GetFormGuid(string name)
		{
			string _value = GetFormString(name);
			if(_value.IsNullOrEmpty_()) return Guid.Empty;
			return _value.ToGuid_();
		}

		/// <summary>获取表单提交的变量并转换成数组</summary>
		/// <param name="name">变量名称</param>
		/// <param name="splitOption">拆分选项</param>
		/// <returns>表单提交的变量并转换成数组</returns>
		public static string[] GetFormStringArray(string name, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
		{
			string _value = HttpContext.Current.Request.Form.Get(name);
			if(_value.IsNull_()) return Utils.EmptyArray<string>();
			return _value.Split_(",", splitOption);
		}

		/// <summary>获取表单提交的变量并转换成数字数组</summary>
		/// <param name="name">变量名称</param>
		/// <param name="splitOption">拆分选项</param>
		/// <returns>表单提交的变量并转换成数字数组</returns>
		public static int[] GetFormIntArray(string name, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
		{
			return GetFormStringArray(name, splitOption).ToIntArray_();
		}

		/// <summary>获取表单提交的变量并转换成 GUID 数组</summary>
		/// <param name="name">变量名称</param>
		/// <param name="splitOption">拆分选项</param>
		/// <returns>表单提交的变量并转换成 GUID 数组</returns>
		public static Guid[] GetFormGuidArray(string name, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
		{
			return GetFormStringArray(name, splitOption).ToGuidArray_();
		}
	}
}