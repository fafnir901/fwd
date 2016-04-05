using System;
using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;
using WebRock.Utils.Monad;

namespace Text.Analizer.Rules
{
	public class EnglishRules:ILanguageRule
	{
		public bool IsMultipleWord(Word word)
		{
			return word.Letters.Last().EntityManager.Container.OriginalString == "s";
		}

		public bool StartWithUpper(Word word)
		{
			return char.IsUpper(word.Letters
				.First().Value.ToCharArray()
				.First());
		}

		public bool IsNoun(Word word)
		{
			throw new System.NotImplementedException();
		}

		public bool IsAdjective(Word word)
		{
			throw new System.NotImplementedException();
		}

		public bool IsVerb(Word word)
		{
			throw new System.NotImplementedException();
		}

		public IList<string> GetSuffixes(Word word)
		{
			throw new System.NotImplementedException();
		}

		public IList<string> GetPrefixes(Word word)
		{
			throw new System.NotImplementedException();
		}

		public IList<string> GetRoots(Word word)
		{
			throw new System.NotImplementedException();
		}

		public string GetEnd(Word word)
		{
			var currentWord = word.Value;

			var maybeEnd = new Func<Maybe<string>>(() => currentWord.EndsWith("s") ? "s".ToMaybe() : Maybe.Nothing)()
				.OrElse(() => currentWord.EndsWith("ies") ? "ies".ToMaybe() : Maybe.Nothing)
				.GetOrDefault(null);
			return maybeEnd;
		}
	}
}
