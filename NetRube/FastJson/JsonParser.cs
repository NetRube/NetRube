﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace NetRube.FastJson
{
	/// <summary>JSON 处理器</summary>
	internal sealed class JsonParser
	{
		enum Token
		{
			None = -1,           // Used to denote no Lookahead available
			Curly_Open,
			Curly_Close,
			Squared_Open,
			Squared_Close,
			Colon,
			Comma,
			String,
			Number,
			True,
			False,
			Null
		}

		readonly char[] json;
		readonly STR s = new STR();
		Token lookAheadToken = Token.None;
		int index;
		bool _ignorecase = false;


		/// <summary>初始化一个新 <see cref="JsonParser" /> 实例。</summary>
		/// <param name="json">JSON 字符串</param>
		/// <param name="ignorecase">指定是否忽略大小写</param>
		internal JsonParser(string json, bool ignorecase)
		{
			this.json = json.ToCharArray();
			_ignorecase = ignorecase;
		}

		/// <summary>将当前的 JSON 字符串转换为对象</summary>
		/// <returns>返回处理后的对象</returns>
		public object Decode()
		{
			return ParseValue();
		}

		private Dictionary<string, object> ParseObject()
		{
			var table = new Dictionary<string, object>();

			ConsumeToken(); // {

			while(true)
			{
				switch(LookAhead())
				{

					case Token.Comma:
						ConsumeToken();
						break;
					case Token.Curly_Close:
						ConsumeToken();
						return table;
					default:
						// name
						var name = ParseString();
						if(_ignorecase)
							name = name.ToLower();

						// :
						if(NextToken() != Token.Colon)
							throw new Exception(Localization.Resources.ExpectedColonAtIndexException.F(index));

						// value
						table[name] = ParseValue();
						break;
				}
			}
		}

		private List<object> ParseArray()
		{
			var array = new List<object>();
			ConsumeToken(); // [

			while(true)
			{
				switch(LookAhead())
				{
					case Token.Comma:
						ConsumeToken();
						break;
					case Token.Squared_Close:
						ConsumeToken();
						return array;
					default:
						array.Add(ParseValue());
						break;
				}
			}
		}

		private object ParseValue()
		{
			switch(LookAhead())
			{
				case Token.Number:
					return ParseNumber();
				case Token.String:
					return ParseString();
				case Token.Curly_Open:
					return ParseObject();
				case Token.Squared_Open:
					return ParseArray();
				case Token.True:
					ConsumeToken();
					return true;
				case Token.False:
					ConsumeToken();
					return false;
				case Token.Null:
					ConsumeToken();
					return null;
			}

			throw new Exception(Localization.Resources.UnrecognizedTokenAtIndexException.F(index));
		}

		private string ParseString()
		{
			ConsumeToken(); // "

			s.Length = 0;

			int runIndex = -1;

			while(index < json.Length)
			{
				var c = json[index++];

				if(c == '"')
				{
					if(runIndex != -1)
					{
						if(s.Length == 0)
							return new string(json, runIndex, index - runIndex - 1);

						s.Append(json, runIndex, index - runIndex - 1);
					}
					return s.ToString();
				}

				if(c != '\\')
				{
					if(runIndex == -1)
						runIndex = index - 1;

					continue;
				}

				if(index == json.Length) break;

				if(runIndex != -1)
				{
					s.Append(json, runIndex, index - runIndex - 1);
					runIndex = -1;
				}

				switch(json[index++])
				{
					case '"':
						s.Append('"');
						break;
					case '\\':
						s.Append('\\');
						break;
					case '/':
						s.Append('/');
						break;
					case 'b':
						s.Append('\b');
						break;
					case 'f':
						s.Append('\f');
						break;
					case 'n':
						s.Append('\n');
						break;
					case 'r':
						s.Append('\r');
						break;
					case 't':
						s.Append('\t');
						break;
					case 'u':
						var remainingLength = json.Length - index;
						if(remainingLength < 4) break;

						// parse the 32 bit hex into an integer codepoint
						var codePoint = ParseUnicode(json[index], json[index + 1], json[index + 2], json[index + 3]);
						s.Append((char)codePoint);

						// skip 4 chars
						index += 4;

						break;
				}
			}

			throw new Exception(Localization.Resources.UnexpectedlyReachedEndOfStringException);
		}

		private uint ParseSingleChar(char c1, uint multipliyer)
		{
			uint p1 = 0;
			if(c1 >= '0' && c1 <= '9')
				p1 = (uint)(c1 - '0') * multipliyer;
			else if(c1 >= 'A' && c1 <= 'F')
				p1 = (uint)((c1 - 'A') + 10) * multipliyer;
			else if(c1 >= 'a' && c1 <= 'f')
				p1 = (uint)((c1 - 'a') + 10) * multipliyer;
			return p1;
		}

		private uint ParseUnicode(char c1, char c2, char c3, char c4)
		{
			var p1 = ParseSingleChar(c1, 0x1000);
			var p2 = ParseSingleChar(c2, 0x100);
			var p3 = ParseSingleChar(c3, 0x10);
			var p4 = ParseSingleChar(c4, 1);

			return p1 + p2 + p3 + p4;
		}

		private long CreateLong(string s)
		{
			var num = 0L;
			var neg = false;
			foreach(char cc in s)
			{
				if(cc == '-')
					neg = true;
				else if(cc == '+')
					neg = false;
				else
				{
					num *= 10;
					num += (int)(cc - '0');
				}
			}

			return neg ? -num : num;
		}

		private object ParseNumber()
		{
			ConsumeToken();

			// Need to start back one place because the first digit is also a token and would have been consumed
			var startIndex = index - 1;
			var dec = false;
			do
			{
				if(index == json.Length)
					break;
				var c = json[index];

				if((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
				{
					if(c == '.' || c == 'e' || c == 'E')
						dec = true;
					if(++index == json.Length)
						break;                        //throw new Exception("Unexpected end of string whilst parsing number");
					continue;
				}
				break;
			} while(true);

			var s = new string(json, startIndex, index - startIndex);
			if(dec)
				return double.Parse(s, NumberFormatInfo.InvariantInfo);
			return CreateLong(s);
		}

		private Token LookAhead()
		{
			if(lookAheadToken != Token.None) return lookAheadToken;

			return lookAheadToken = NextTokenCore();
		}

		private void ConsumeToken()
		{
			lookAheadToken = Token.None;
		}

		private Token NextToken()
		{
			var result = lookAheadToken != Token.None ? lookAheadToken : NextTokenCore();

			lookAheadToken = Token.None;

			return result;
		}

		private Token NextTokenCore()
		{
			char c;

			// Skip past whitespace
			do
			{
				c = json[index];

				if(c > ' ') break;
				if(c != ' ' && c != '\t' && c != '\n' && c != '\r') break;

			} while(++index < json.Length);

			if(index == json.Length)
				throw new Exception(Localization.Resources.UnexpectedlyReachedEndOfStringException);

			c = json[index];

			index++;

			//if (c >= '0' && c <= '9')
			//    return Token.Number;

			switch(c)
			{
				case '{':
					return Token.Curly_Open;
				case '}':
					return Token.Curly_Close;
				case '[':
					return Token.Squared_Open;
				case ']':
					return Token.Squared_Close;
				case ',':
					return Token.Comma;
				case '"':
					return Token.String;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
				case '+':
				case '.':
					return Token.Number;
				case ':':
					return Token.Colon;
				case 'f':
					if(json.Length - index >= 4 &&
						json[index + 0] == 'a' &&
						json[index + 1] == 'l' &&
						json[index + 2] == 's' &&
						json[index + 3] == 'e')
					{
						index += 4;
						return Token.False;
					}
					break;
				case 't':
					if(json.Length - index >= 3 &&
						json[index + 0] == 'r' &&
						json[index + 1] == 'u' &&
						json[index + 2] == 'e')
					{
						index += 3;
						return Token.True;
					}
					break;
				case 'n':
					if(json.Length - index >= 3 &&
						json[index + 0] == 'u' &&
						json[index + 1] == 'l' &&
						json[index + 2] == 'l')
					{
						index += 3;
						return Token.Null;
					}
					break;
			}

			throw new Exception(Localization.Resources.CouldNotFindTokenAtIndexException.F(--index));
		}
	}
}