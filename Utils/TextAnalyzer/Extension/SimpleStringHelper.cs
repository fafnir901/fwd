using System.Collections.Generic;
using System.Text;

namespace Text.Analizer.Extension
{
	public static class SimpleStringHelper
	{

		public static string ConvertStringArrayToString(this IEnumerable<string> strs)
		{
			var builder = new StringBuilder();
			foreach (var str in strs)
			{
				builder.Append(str);
			}
			return builder.ToString();
		}
	}
}
