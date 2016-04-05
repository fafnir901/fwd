using System;
using System.Text.RegularExpressions;
using Text.Analizer.Rules;
using Text.Analizer.Strategies;
using WebRock.Utils.Monad;

namespace Text.Analizer
{
	public class LanguageFactory
	{
		public static ILanguageRule GetRules(string language, string text)
		{
			switch (language)
			{
				case "en-En":
					return new EnglishRules();
				case "ru-Ru":
					return new RussianRules();
				default:
					language = DefineLanguage(text);
					return GetRules(language, text);
			}
		}

		public static string DefineLanguage(string text)
		{
			var eng = new Regex("[a-zA-Z]");
			var rus = new Regex("[а-яА-Я]");

			return new Func<Maybe<string>>(() =>
				rus.IsMatch(text)
					? "ru-Ru".ToMaybe()
					: Maybe.Nothing)()
				.OrElse(() => eng.IsMatch(text)
					? "en-En".ToMaybe()
					: Maybe.Nothing)
				.GetOrDefault("en-En");
		}
	}
}