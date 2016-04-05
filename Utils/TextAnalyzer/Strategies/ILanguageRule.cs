using System.Collections.Generic;
using Text.Analizer.Termins;

namespace Text.Analizer.Strategies
{
	public interface ILanguageRule
	{
		bool IsMultipleWord(Word word);
		bool StartWithUpper(Word word);
		bool IsNoun(Word word);
		bool IsAdjective(Word word);
		bool IsVerb(Word word);

		IList<string> GetSuffixes(Word word);

		IList<string> GetPrefixes(Word word);

		IList<string> GetRoots(Word word);

		string GetEnd(Word word);
	}
}
