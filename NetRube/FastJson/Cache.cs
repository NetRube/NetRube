using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NetRube.FastJson
{
	internal sealed class Cache
	{
		public readonly static Cache Instance = new Cache();

		private Cache() { }

		private static ConcurrentDictionary<string, Dictionary<string, JsonAccessor>> AC;
		internal Dictionary<string, JsonAccessor> GetJsonAccessors(Type type)
		{
			if(AC == null) AC = new ConcurrentDictionary<string, Dictionary<string, JsonAccessor>>();

			Dictionary<string, JsonAccessor> jas;
			var typeName = type.FastGetFullName();
			if(!AC.TryGetValue(typeName, out jas))
			{
				var ls = type.FastGetAccessors();
				jas = new Dictionary<string, JsonAccessor>(ls.Count);
				foreach(var a in ls.Values)
					jas.Add(a.Name, GetJsonAccessor(a));
				AC.TryAdd(typeName, jas);
			}

			return jas;
		}

		private JsonAccessor GetJsonAccessor(Accessor a)
		{
			var ja = new JsonAccessor
			{
				Name = a.Name,
				Member = a.Member,
				DataType = a.DataType,
				AccessorType = a.AccessorType,
				CanRade = a.CanRade,
				CanWrite = a.CanWrite,
				IsVirtual = a.IsVirtual
			};
			var t = ja.DataType;
			ja.IsDictionary = t.Name.Contains("Dictionary");
			if(ja.IsDictionary)
				ja.GenericTypes = t.GetGenericArguments();
			ja.IsValueType = t.IsValueType;
			ja.IsGenericType = t.IsGenericType;
			ja.IsArray = t.IsArray;
			if(ja.IsArray)
				ja.ElementType = t.GetElementType();
			if(ja.IsGenericType)
				ja.ElementType = t.GetGenericArguments()[0];
			ja.IsByteArray = t == typeof(byte[]);
			ja.IsGuid = t == typeof(Guid) || t == typeof(Guid?);
			ja.IsHashtable = t == typeof(System.Collections.Hashtable);
			ja.IsDataSet = t == typeof(System.Data.DataSet);
			ja.IsDataTable = t == typeof(System.Data.DataTable);
			ja.IsEnum = t.IsEnum;
			ja.IsDateTime = t == typeof(DateTime) || t == typeof(DateTime?);
			ja.IsInt = t == typeof(int) || t == typeof(int?);
			ja.IsLong = t == typeof(long) || t == typeof(long?);
			ja.IsString = t == typeof(string);
			ja.IsBool = t == typeof(bool) || t == typeof(bool?);
			ja.IsClass = t.IsClass;
			if(ja.IsDictionary && ja.GenericTypes.Length > 0 && ja.GenericTypes[0] == typeof(string))
				ja.IsStringDictionary = true;
			if(t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				ja.ConversionType = t.GetGenericArguments()[0];
			else
				ja.ConversionType = t;

			return ja;
		}
	}
}