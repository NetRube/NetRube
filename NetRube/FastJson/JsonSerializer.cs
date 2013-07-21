using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace NetRube.FastJson
{
	internal sealed class JsonSerializer
	{
		private STR OUTPUT = new STR();
		readonly int MAX_DEPTH = 10;
		int CURRENT_DEPTH = 0;
		Json.Parameters PARAMS;

		internal JsonSerializer(Json.Parameters param)
		{
			this.PARAMS = param;
		}

		internal string ConvertToJson(object obj)
		{
			WriteValue(obj);
			return this.OUTPUT.ToString();
		}

		private void WriteValue(object obj)
		{
			if(obj.IsNullOrDBNull_())
				Write("null");
			else if(obj is string || obj is char)
				WriteString(obj.ToString());
			else if(obj is Guid)
				WriteGuid((Guid)obj);
			else if(obj is bool)
				Write(((bool)obj) ? "true" : "false");
			else if(
					obj is int || obj is long || obj is double ||
					obj is decimal || obj is float ||
					obj is byte || obj is short ||
					obj is sbyte || obj is ushort ||
					obj is uint || obj is ulong
				)
				Write(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));
			else if(obj is DateTime)
				WriteDateTime((DateTime)obj);
			else if(
					obj is IDictionary &&
					obj.GetType().IsGenericType &&
					obj.GetType().GetGenericArguments()[0] == typeof(string)
				)
				WriteStringDict(obj as IDictionary);
			else if(obj is IDictionary)
				WriteDict(obj as IDictionary);
			else if(obj is DataSet)
				WriteDataset(obj as DataSet);
			else if(obj is DataTable)
				WriteDataTable(obj as DataTable);
			else if(obj is byte[])
				WriteBytes((byte[])obj);
			else if(obj is Array || obj is IList || obj is ICollection || obj is IEnumerable)
				WriteArray(obj as IEnumerable);
			else if(obj is Enum)
				WriteEnum((Enum)obj);
			else
				WriteObject(obj);
		}

		private void WriteEnum(Enum e)
		{
			switch(this.PARAMS.EnumFormat)
			{
				case Json.EnumType.Name:
					WriteStringFast(e.ToString_());
					break;
				case Json.EnumType.Value:
					Write(Convert.ChangeType(e, Enum.GetUnderlyingType(e.GetType())).ToString());
					break;
			}
		}

		private void WriteGuid(Guid guid)
		{
			WriteStringFast(guid.ToString(this.PARAMS.GuidFormat.ToString_()));
		}

		private void WriteBytes(byte[] bs)
		{
			WriteStringFast(Convert.ToBase64String(bs, 0, bs.Length, Base64FormattingOptions.None));
		}

		private void WriteDateTime(DateTime dt)
		{
			if(this.PARAMS.UseDateTimeTicks)
				Write(((dt.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString());
			WriteStringFast(dt.ToJson_());
		}

		private void WriteDataset(DataSet ds)
		{
			Write('{');
			var pendingSeperator = false;
			foreach(DataTable dt in ds.Tables)
			{
				if(pendingSeperator) Write(',');
				pendingSeperator = true;

				WriteDataTableData(dt);
			}
			Write('}');
		}

		private void WriteDataTableData(DataTable dt)
		{
			Write('\"');
			Write(dt.TableName);
			Write("\":[");
			var cols = dt.Columns;
			var rowSeparator = false;
			var pendingSeperator = false;
			foreach(DataRow row in dt.Rows)
			{
				if(rowSeparator) Write(',');
				rowSeparator = true;
				Write('[');

				pendingSeperator = false;
				foreach(DataColumn col in cols)
				{
					if(pendingSeperator) Write(',');
					pendingSeperator = true;

					WriteValue(row[col]);
				}
				Write(']');
			}
			Write(']');
		}

		void WriteDataTable(DataTable dt)
		{
			Write('{');
			WriteDataTableData(dt);
			Write('}');
		}

		private void WriteObject(object obj)
		{
			this.CURRENT_DEPTH++;
			if(this.CURRENT_DEPTH > this.MAX_DEPTH)
				throw new ArgumentOutOfRangeException(Localization.Resources.SerializerEncounteredMaximumDepth.F(this.MAX_DEPTH));

			var t = obj.GetType();

			Write('{');

			var pendingSeperator = false;
			var jas = Cache.Instance.GetJsonAccessors(t);
			foreach(var ja in jas.Values)
			{
				var val = ja.GetValue(obj);

				if(!this.PARAMS.SerializeNullValues && val.IsNullOrDBNull_())
					continue;

				if(pendingSeperator) Write(',');
				pendingSeperator = true;

				WritePair(ja.Name, val);
			}

			this.CURRENT_DEPTH--;
			Write('}');
			this.CURRENT_DEPTH--;
		}

		private void WritePairFast(string name, string value)
		{
			if(!this.PARAMS.SerializeNullValues && value == null)
				return;

			WriteStringFast(name);
			Write(':');
			WriteStringFast(value);
		}

		private void WritePair(string name, object value)
		{
			if(!this.PARAMS.SerializeNullValues && value.IsNullOrDBNull_())
				return;

			WriteStringFast(name);
			Write(':');
			WriteValue(value);
		}

		private void WriteArray(IEnumerable array)
		{
			Write('[');

			var pendingSeperator = false;

			foreach(var obj in array)
			{
				if(pendingSeperator) Write(',');
				pendingSeperator = true;

				WriteValue(obj);
			}
			Write(']');
		}

		private void WriteStringDict(IDictionary dict)
		{
			Write('[');

			var pendingSeparator = false;

			foreach(DictionaryEntry entry in dict)
			{
				if(pendingSeparator) Write(',');
				pendingSeparator = true;

				WritePair(entry.Key.ToString(), entry.Value);
			}
			Write(']');
		}

		private void WriteDict(IDictionary dict)
		{
			Write('[');

			var pendingSeparator = false;

			foreach(DictionaryEntry entry in dict)
			{
				if(pendingSeparator) Write(',');
				pendingSeparator = true;

				Write('{');
				WritePair("k", entry.Key);
				Write(',');
				WritePair("v", entry.Value);
				Write('}');
			}
			Write(']');
		}

		private void WriteStringFast(string s)
		{
			Write('\"');
			Write(s);
			Write('\"');
		}

		private void WriteString(string s)
		{
			Write('\"');

			int runIndex = -1;
			char c;

			for(int i = 0, l = s.Length; i < l; ++i)
			{
				c = s[i];

				if(c != '\t' && c != '\n' && c != '\r' && c != '\"' && c != '\\')// && c != ':' && c!=',')
				{
					if(runIndex == -1)
						runIndex = i;

					continue;
				}

				if(runIndex != -1)
				{
					Write(s, runIndex, i - runIndex);
					runIndex = -1;
				}

				switch(c)
				{
					case '\t':
						Write("\\t");
						break;
					case '\r':
						Write("\\r");
						break;
					case '\n':
						Write("\\n");
						break;
					case '"':
					case '\\':
						Write('\\');
						Write(c);
						break;
				}
			}

			if(runIndex != -1)
				Write(s, runIndex, s.Length - runIndex);

			Write('\"');
		}

		private void Write(char c)
		{
			this.OUTPUT.Append(c);
		}

		private void Write(string str)
		{
			this.OUTPUT.Append(str);
		}

		private void Write(string str, int start, int end)
		{
			this.OUTPUT.Append(str, start, end);
		}
	}
}