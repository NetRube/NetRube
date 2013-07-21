using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace NetRube.FastJson
{
	/// <summary>JSON 序列化/反序列化</summary>
	public sealed class Json
	{
		private Parameters PARAMS;

		/// <summary>初始化一个新 <see cref="Json"/> 实例。</summary>
		/// <param name="param">参数</param>
		public Json(Parameters param = null)
		{
			this.PARAMS = param ?? new Parameters();
		}

		/// <summary>将对象转换成 JSON 字符串</summary>
		/// <param name="obj">要转换的对象</param>
		/// <returns>转换后的 JSON 字符串</returns>
		public string ToJson(object obj)
		{
			if(obj == null) return "null";

			return new JsonSerializer(this.PARAMS).ConvertToJson(obj);
		}

		/// <summary>将 JSON 字符串转换成对象</summary>
		/// <param name="json">要转换的 JSON 字符串</param>
		/// <returns>转换后的对象</returns>
		public object Parse(string json)
		{
			return new JsonParser(json, false).Decode();
		}

		/// <summary>将 JSON 字符串转换成对象</summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="json">要转换的 JSON 字符串</param>
		/// <returns>转换后的对象</returns>
		public T ToObject<T>(string json)
		{
			return (T)ToObject(json, typeof(T));
		}

		/// <summary>将 JSON 字符串转换成对象</summary>
		/// <param name="json">要转换的 JSON 字符串</param>
		/// <returns>转换后的对象</returns>
		public object ToObject(string json)
		{
			return ToObject(json, null);
		}

		/// <summary>将 JSON 字符串转换成对象</summary>
		/// <param name="json">要转换的 JSON 字符串</param>
		/// <param name="type">对象类型</param>
		/// <returns>转换后的对象</returns>
		public object ToObject(string json, Type type)
		{
			var obj = Parse(json);
			if(obj == null)
				return null;

			if(obj is IDictionary)
			{
				if(type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
					return RootDict(obj, type);
				return ParseDict(obj as Dictionary<string, object>, type, null);
			}

			if(obj is List<object>)
			{
				if(type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
					return RootDict(obj, type);
				if(type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
					return RootList(obj, type);
				return (obj as List<object>).ToArray();
			}

			if(type != null && obj.GetType() != type)
				return ChangeType(obj, type);

			return obj;
		}

		/// <summary>填充对象</summary>
		/// <param name="input">要填充的对象</param>
		/// <param name="json">用于填充的 JSON 代码</param>
		/// <returns>填充后的对象</returns>
		public object FillObject(object input, string json)
		{
			Dictionary<string, object> ht = Parse(json) as Dictionary<string, object>;
			if(ht == null) return null;
			return ParseDict(ht, input.GetType(), input);
		}

		#region 内部处理
		private object RootList(object parse, Type type)
		{
			var types = type.GetGenericArguments();
			var ls = type.FastInvoke() as IList;
			foreach(var k in parse as IList)
			{
				var v = k;
				if(k is Dictionary<string, object>)
					v = ParseDict(k as Dictionary<string, object>, types[0], null);
				else
					v = ChangeType(k, types[0]);

				ls.Add(v);
			}
			return ls;
		}

		private object RootDict(object parse, Type type)
		{
			var types = type.GetGenericArguments();
			if(parse is Dictionary<string, object>)
			{
				var dict = type.FastInvoke() as IDictionary;
				foreach(var kv in parse as Dictionary<string, object>)
				{
					object v;
					object k = ChangeType(kv.Key, types[0]);
					if(kv.Value is Dictionary<string, object>)
						v = ParseDict(kv.Value as Dictionary<string, object>, types[1], null);
					else if(kv.Value is List<object>)
						v = CreateArray(kv.Value as List<object>, typeof(object));
					else
						v = ChangeType(kv.Value, types[1]);

					dict.Add(k, v);
				}
				return dict;
			}
			if(parse is List<object>)
				return CreateDict(parse as List<object>, type, types);

			return null;
		}

		private object ParseDict(Dictionary<string, object> d, Type type, object input)
		{
			if(type == null)
				throw new ArgumentNullException(Localization.Resources.CannotDetermineType);
			
			var typeName = type.FastGetFullName();
			var obj = input;
			if(obj == null)
				obj = type.FastInvoke();

			var jas = Cache.Instance.GetJsonAccessors(type);
			string name;
			foreach(var n in d.Keys)
			{
				name = n;
				JsonAccessor ja;
				if(!jas.TryGetValue(name, out ja))
					continue;

				if(ja.CanWrite)
				{
					var v = d[name];
					if(v != null)
					{
						object oset = null;

						if(ja.IsInt)
							oset = (int)((long)v);
						else if(ja.IsLong)
							oset = (long)v;
						else if(ja.IsString)
							oset = (string)v;
						else if(ja.IsBool)
							oset = (bool)v;
						else if(ja.IsGenericType && !ja.IsValueType && !ja.IsDictionary && v is List<object>)
							oset = CreateGenericList(v as List<object>, ja.DataType, ja.ElementType);
						else if(ja.IsByteArray)
							oset = Convert.FromBase64String((string)v);
						else if(ja.IsArray && !ja.IsValueType)
							oset = CreateArray(v as List<object>, ja.ElementType);
						else if(ja.IsGuid)
							oset = CreateGuid((string)v);
						else if(ja.IsStringDictionary)
							oset = CreateStringKeyDict(v as Dictionary<string, object>, ja.DataType, ja.GenericTypes);
						else if(ja.IsDictionary || ja.IsHashtable)
							oset = CreateDict(v as List<object>, ja.DataType, ja.GenericTypes);
						else if(ja.IsEnum)
							oset = CreateEnum(ja.DataType, v);
						else if(ja.IsDateTime)
							oset = CreateDateTime((object)v);
						else if(ja.IsClass && v is Dictionary<string, object>)
							oset = ParseDict(v as Dictionary<string, object>, ja.DataType, ja.GetValue(obj));
						else if(ja.IsValueType)
							oset = ChangeType(v, ja.ConversionType);
						else if(v is List<object>)
							oset = CreateArray(v as List<object>, typeof(object));
						else
							oset = v;

						ja.SetValue(obj, oset);
					}
				}
			}
			return obj;
		}

		private DateTime CreateDateTime(object v)
		{
			if(v is long)
				return new DateTime((long)v * 10000 + 621355968000000000, DateTimeKind.Utc).ToLocalTime();
			return v.ToDate_();
		}

		private object CreateEnum(Type type, object v)
		{
			if(v is long)
				return Enum.ToObject(type, (long)v);
			return Enum.Parse(type, (string)v);
		}

		private object CreateDict(List<object> list, Type dataType, Type[] types)
		{
			var ls = dataType.FastInvoke() as IDictionary;
			Type t1 = null;
			Type t2 = null;
			if(types != null)
			{
				t1 = types[0];
				t2 = types[1];
			}

			foreach(Dictionary<string, object> item in list)
			{
				var key = item["k"];
				var val = item["v"];

				if(key is Dictionary<string, object>)
					key = ParseDict(key as Dictionary<string, object>, t1, null);
				else
					key = ChangeType(key, t1);

				if(val is Dictionary<string, object>)
					val = ParseDict(val as Dictionary<string, object>, t2, null);
				else
					val = ChangeType(val, t2);

				ls.Add(key, val);
			}
			return ls;
		}

		private object CreateStringKeyDict(Dictionary<string, object> dict, Type dataType, Type[] types)
		{
			var ls = dataType.FastInvoke() as IDictionary;
			Type t1 = null;
			Type t2 = null;
			if(types != null)
			{
				t1 = types[0];
				t2 = types[1];
			}

			foreach(KeyValuePair<string, object> item in dict)
			{
				var key = item.Key;
				var val = item.Value;
				object obj = null;
				if(val is Dictionary<string, object>)
					obj = ParseDict(val as Dictionary<string, object>, t2, null);
				else
					obj = ChangeType(val, t2);
				ls.Add(key, obj);
			}
			return ls;
		}

		private Guid CreateGuid(string s)
		{
			return new Guid(s);
		}

		private object CreateArray(List<object> list, Type elType)
		{
			var len = list.Count;
			var arr = Array.CreateInstance(elType, len);
			for(int i = 0; i < len; i++)
			{
				var obj = list[i];
				if(obj is IDictionary)
					arr.SetValue(ParseDict(obj as Dictionary<string, object>, elType, null), i);
				else
					arr.SetValue(ChangeType(obj, elType), i);
			}
			return arr;
		}

		private object ChangeType(object value, Type conversionType)
		{
			if(conversionType == typeof(int))
				return (int)((long)value);
			else if(conversionType == typeof(long))
				return (long)value;
			else if(conversionType == typeof(string))
				return (string)value;
			else if(conversionType == typeof(DateTime))
				return CreateDateTime((long)value);
			else if(conversionType == typeof(Guid))
				return CreateGuid((string)value);
			else if(conversionType.IsEnum)
				return CreateEnum(conversionType, value);
			return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
		}

		private object CreateGenericList(List<object> list, Type dataType, Type elType)
		{
			var ls = dataType.FastInvoke() as IList;
			foreach(var obj in list)
			{
				if(obj is IDictionary)
					ls.Add(ParseDict(obj as Dictionary<string, object>, elType, null));
				else if(obj is List<object>)
					ls.Add((obj as List<object>).ToArray());
				else
					ls.Add(ChangeType(obj, elType));
			}
			return ls;
		}

		//private DataSet CreateDataset(Dictionary<string, object> dict)
		//{
		//	var ds = new DataSet();
		//	ds.EnforceConstraints = false;
		//	ds.BeginInit();

		//	var schema = dict["$schema"];
		//	var dss = ParseDict(schema as Dictionary<string, object>, typeof(DatasetSchema), null) as DatasetSchema;
		//	ds.DataSetName = dss.Name;
		//	for(int i = 0, l = dss.Info.Count; i < l; i += 3)
		//	{
		//		if(!ds.Tables.Contains(dss.Info[i]))
		//			ds.Tables.Add(dss.Info[i]);
		//		ds.Tables[dss.Info[i]].Columns.Add(dss.Info[i + 1], FastReflection.FastGetType(dss.Info[i + 2]));
		//	}

		//	foreach(KeyValuePair<string, object> item in dict)
		//	{
		//		var key = item.Key;
		//		if(key == "$schema") continue;

		//		var val = item.Value;
		//		var rows = (List<object>)val;
		//		if(rows == null) continue;

		//		var dt = ds.Tables[key];
		//		ReadDataTable(rows, dt);
		//	}

		//	ds.EndInit();

		//	return ds;
		//}

		//private DataTable CreateDataTable(Dictionary<string, object> dict)
		//{
		//	var dt = new DataTable();

		//	var schema = dict["$schema"];
		//	var dss = ParseDict(schema as Dictionary<string, object>, typeof(DatasetSchema), null) as DatasetSchema;
		//	dt.TableName = dss.Name;
		//	for(int i = 0, l = dss.Info.Count; i < l; i += 3)
		//		dt.Columns.Add(dss.Info[i + 1], FastReflection.FastGetType(dss.Info[i + 2]));

		//	foreach(var item in dict)
		//	{
		//		if(item.Key == "$schema") continue;

		//		var rows = item.Value as List<object>;
		//		if(rows == null) continue;
		//		if(!dt.TableName.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase))
		//			continue;

		//		ReadDataTable(rows, dt);
		//	}
		//	return dt;
		//}

		//private void ReadDataTable(List<object> rows, DataTable dt)
		//{
		//	dt.BeginInit();
		//	dt.BeginLoadData();
		//	var guidCols = new List<int>();
		//	var dateCols = new List<int>();

		//	foreach(DataColumn col in dt.Columns)
		//	{
		//		var type = col.DataType;
		//		if(type == typeof(Guid) || type == typeof(Guid?))
		//			guidCols.Add(col.Ordinal);
		//		else if(type == typeof(DateTime) || type == typeof(DateTime?))
		//			dateCols.Add(col.Ordinal);
		//	}

		//	foreach(List<object> row in rows)
		//	{
		//		var v = new object[row.Count];
		//		row.CopyTo(v, 0);
		//		foreach(var i in guidCols)
		//		{
		//			string s = (string)v[i];
		//			if(s != null)
		//				v[i] = CreateGuid(s);
		//		}
		//		foreach(var i in dateCols)
		//		{
		//			long n = (long)v[i];
		//			if(n != null)
		//				v[i] = CreateDateTime(n);
		//		}
		//		dt.Rows.Add(v);
		//	}

		//	dt.EndLoadData();
		//	dt.EndInit();
		//}
		#endregion

		/// <summary>参数</summary>
		public class Parameters
		{
			/// <summary>GUID 格式，默认为 GuidType.D</summary>
			public GuidType GuidFormat = GuidType.D;
			/// <summary>枚举格式，默认为 EnumType.Value</summary>
			public EnumType EnumFormat = EnumType.Value;
			/// <summary>是否使用 Ticks 日期格式，默认为 false</summary>
			public bool UseDateTimeTicks = false;
			/// <summary>序列化 null 值 (默认为 True)</summary>
			public bool SerializeNullValues = true;
		}

		/// <summary>
		/// GUID 格式类型
		/// </summary>
		public enum GuidType
		{
			/// <summary>
			/// <para>32 位：</para>
			/// <para>00000000000000000000000000000000</para>
			/// </summary>
			N,
			/// <summary>
			/// <para>由连字符分隔的 32 位数字：</para>
			/// <para>00000000-0000-0000-0000-000000000000</para>
			/// </summary>
			D,
			/// <summary>
			/// <para>括在大括号中、由连字符分隔的 32 位数字：</para>
			/// <para>{00000000-0000-0000-0000-000000000000}</para>
			/// </summary>
			B,
			/// <summary>
			/// <para>括在圆括号中、由连字符分隔的 32 位数字：</para>
			/// <para>(00000000-0000-0000-0000-000000000000)</para>
			/// </summary>
			P,
			/// <summary>
			/// <para>括在大括号的 4 个十六进制值，其中第 4 个值是 8 个十六进制值的子集（也括在大括号中）位：</para>
			/// <para>{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}</para>
			/// </summary>
			X
		}

		/// <summary>枚举格式类型</summary>
		public enum EnumType
		{
			/// <summary>名称</summary>
			Name,
			/// <summary>值</summary>
			Value
		}
	}
}