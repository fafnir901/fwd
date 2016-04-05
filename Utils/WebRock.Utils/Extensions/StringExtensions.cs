using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebRock.Utils.Annotations;
using WebRock.Utils.Extensions;
using WebRock.Utils.Monad;

namespace System
{
	public static class StringExtensions
	{
		private static readonly Dictionary<char, char> SpecialCharacters;
		private static readonly Dictionary<char, char> SpecialCharactersInverted;
		static StringExtensions()
		{
			StringExtensions.SpecialCharacters = new Dictionary<char, char>
			{
				
				{
					'0',
					'\0'
				},
				
				{
					'a',
					'\a'
				},
				
				{
					'b',
					'\b'
				},
				
				{
					'f',
					'\f'
				},
				
				{
					'n',
					'\n'
				},
				
				{
					'r',
					'\r'
				},
				
				{
					't',
					'\t'
				},
				
				{
					'v',
					'\v'
				},
				
				{
					'\\',
					'\\'
				},
				
				{
					'"',
					'"'
				}
			};
			StringExtensions.SpecialCharactersInverted = StringExtensions.SpecialCharacters.ToDictionary((KeyValuePair<char, char> x) => x.Value, (KeyValuePair<char, char> x) => x.Key);
		}
		[StringFormatMethod("format")]
		public static string Fmt(this string format, params object[] args)
		{
			return string.Format(format, args);
		}
		public static bool IsNullOrEmpty(this string format)
		{
			return string.IsNullOrEmpty(format);
		}
		public static bool IsNullOrWhitespace(this string value)
		{
			return value == null || value.Trim().Length == 0;
		}
		[StringFormatMethod("format")]
		public static StringBuilder AppendLine(this StringBuilder stringBuilder, string format, params object[] args)
		{
			return stringBuilder.AppendFormat(format, args).AppendLine();
		}
		public static bool Contains(this string source, string value, StringComparison comparisonType)
		{
			return source.IndexOf(value, comparisonType) >= 0;
		}
		public static string ToStringSafe(this object s)
		{
			string result;
			if (s == null)
			{
				result = null;
			}
			else
			{
				result = s.ToString();
			}
			return result;
		}
		public static string TrimSafe(this string s, params char[] chars)
		{
			string result;
			if (s == null)
			{
				result = null;
			}
			else
			{
				result = s.Trim(chars);
			}
			return result;
		}
		public static bool EqualsIgnoreCase(this string a, string b)
		{
			return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
		public static string FilterInvalidXmlCharacters(this string value)
		{
			string result;
			if (value.IsNullOrEmpty())
			{
				result = value;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(value.Length);
				foreach (char current in
					from c in value
					where c == '\t' || c == '\n' || c == '\r' || (c >= ' ' && c <= '퟿') || (c >= '' && c <= '�')
					select c)
				{
					stringBuilder.Append(current);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
		public static string Escape(this string s)
		{
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				char value;
				if (StringExtensions.SpecialCharactersInverted.TryGetValue(c, out value))
				{
					stringBuilder.Append('\\');
					stringBuilder.Append(value);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
		public static string Unescape(this string s)
		{
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			foreach (char current in StringExtensions.NormalizeChars(s))
			{
				stringBuilder.Append(current);
			}
			return stringBuilder.ToString();
		}
		private static IEnumerable<char> NormalizeChars(string s)
		{
			CharEnumerator enumerator = null;
			foreach (char current in s)
			{
				if (current == '\\')
				{
					if (!enumerator.MoveNext())
					{
						throw new ArgumentException("Character literal must contain exactly one character");
					}
					yield return StringExtensions.SpecialCharacters.GetValue(enumerator.Current).GetOrThrow(() => new ArgumentException("Syntax error '{0}'".Fmt(new object[]
					{
						enumerator.Current
					})));
				}
				else
				{
					yield return current;
				}
			}
			yield break;
		}
		public static string CamelCase(this string value)
		{
			string result;
			if (string.IsNullOrEmpty(value))
			{
				result = value;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(value);
				stringBuilder[0] = char.ToLowerInvariant(stringBuilder[0]);
				result = stringBuilder.ToString();
			}
			return result;
		}
		public static string FirstLetterToUpper(this string value)
		{
			string result;
			if (string.IsNullOrEmpty(value))
			{
				result = value;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(value);
				stringBuilder[0] = char.ToUpperInvariant(stringBuilder[0]);
				result = stringBuilder.ToString();
			}
			return result;
		}
		public static string CamelCaseSplitIntoWords(this string str)
		{
			return Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
		}
		public static IEnumerable<string> SplitIntoWords(this string str, string separator)
		{
			IEnumerable<string> result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				result = str.Split(separator.ToCharArray());
			}
			return result;
		}
		public static string CreateStringFromChars(this IEnumerable<char> chars)
		{
			var stringBuilder = new StringBuilder();
			foreach (char current in chars)
			{
				stringBuilder.Append(current);
			}
			return stringBuilder.ToString();
		}

		public static string StripTagsCharArray(this string source)
		{
			var array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}
		public static bool ApplyToString(this string str, string method, string value)
		{
			bool result;
			if (method != null)
			{
				if (method == "Contains")
				{
					result = str.Contains(value);
					return result;
				}
				if (method == "EndsWith")
				{
					result = str.EndsWith(value);
					return result;
				}
				if (method == "StartWith")
				{
					result = str.StartsWith(value);
					return result;
				}
				if (method == "op_Equality" || method == "Equals")
				{
					result = str.Equals(value);
					return result;
				}
			}
			result = false;
			return result;
		}

		public static string ToOneString(this IEnumerable<string> strs)
		{
			var builder = new StringBuilder();
			foreach (var str in strs)
			{
				builder.Append(str);
			}
			return builder.ToString();
		}

		public static IEnumerable<string> WithoutEmptyStrings(this IEnumerable<string> strs)
		{
			return strs.Where(str => !string.IsNullOrEmpty(str));
		}
	}
}
