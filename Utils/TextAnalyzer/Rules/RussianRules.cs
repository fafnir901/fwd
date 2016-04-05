using System;
using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;
using WebRock.Utils.Monad;

namespace Text.Analizer.Rules
{
	public class RussianRules:ILanguageRule
	{
		public bool IsMultipleWord(Word word)
		{
			return false;
		}

		public bool StartWithUpper(Word word)
		{
			return char.IsUpper(word.Letters
				.First().Value.ToCharArray()
				.First());
		}

		public bool IsNoun(Word word)
		{
			throw new NotImplementedException();
		}

		public bool IsAdjective(Word word)
		{
			throw new NotImplementedException();
		}

		public bool IsVerb(Word word)
		{
			throw new NotImplementedException();
		}

		public IList<string> GetSuffixes(Word word)
		{
			throw new NotImplementedException();
		}

		public IList<string> GetPrefixes(Word word)
		{
			throw new NotImplementedException();
		}

		public IList<string> GetRoots(Word word)
		{
			throw new NotImplementedException();
		}

		public string GetEnd(Word word)
		{
			var currentWord = word.Value;

			var maybeEnd = new Func<Maybe<string>>(() => currentWord.EndsWith("ы") ? "Ы".ToMaybe() : Maybe.Nothing)()
				.OrElse(() => currentWord.EndsWith("ов") ? "ов".ToMaybe() : Maybe.Nothing)
				.OrElse(() => currentWord.EndsWith("ев") ? "ев".ToMaybe() : Maybe.Nothing)
				.OrElse(() => currentWord.EndsWith("ых") ? "ых".ToMaybe() : Maybe.Nothing)
				.GetOrDefault(null);
			return maybeEnd;
		}

	}
}
