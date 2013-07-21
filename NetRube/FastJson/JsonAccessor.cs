using System;

namespace NetRube.FastJson
{
	internal class JsonAccessor : Accessor
	{
		/// <summary>数据类型是否为值类型</summary>
		public bool IsValueType;
		/// <summary>数据类型是否为泛型</summary>
		public bool IsGenericType;
		/// <summary>数据类型为泛型时的类型集合</summary>
		public Type[] GenericTypes;
		/// <summary>数据类型是否为字典</summary>
		public bool IsDictionary;
		/// <summary>数据类型是否为数组</summary>
		public bool IsArray;
		/// <summary>数据类型是否为字节数组</summary>
		public bool IsByteArray;
		/// <summary>数据类型是否为 GUID</summary>
		public bool IsGuid;
		/// <summary>数据类型是否为 DataSet</summary>
		public bool IsDataSet;
		/// <summary>数据类型是否为 DataTable</summary>
		public bool IsDataTable;
		/// <summary>数据类型是否为 Hashtable</summary>
		public bool IsHashtable;
		/// <summary>数据类型是否为枚举</summary>
		public bool IsEnum;
		/// <summary>数据类型是否为日期时间</summary>
		public bool IsDateTime;
		/// <summary>数据类型是否为整数</summary>
		public bool IsInt;
		/// <summary>数据类型是否为长整数</summary>
		public bool IsLong;
		/// <summary>数据类型是否为字符串</summary>
		public bool IsString;
		/// <summary>数据类型是否为布尔值</summary>
		public bool IsBool;
		/// <summary>数据类型是否为一个类</summary>
		public bool IsClass;
		/// <summary>数据类型是否为字符串字典</summary>
		public bool IsStringDictionary;
		/// <summary>泛型或数组的元素数据类型</summary>
		public Type ElementType;
		/// <summary>用于 JSON 处理的数据转换类型</summary>
		public Type ConversionType;
	}
}