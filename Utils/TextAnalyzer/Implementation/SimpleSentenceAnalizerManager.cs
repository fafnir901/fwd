using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;
using WebRock.Utils.Monad;

namespace Text.Analizer.Implementation
{
	public class SimpleSentenceAnalizerManager : BaseAnalizerManager, ISentenceAnalyzerManager
	{
		public SimpleSentenceAnalizerManager(IAnalizerContainer container, string language = null)
			: base(container, language)
		{
		}


		public SentenseType GetSentenseType()
		{
			return GetDefaultSentense()
				.OrElse(GetIterrogativeSentense)
				.OrElse(GetExclamatorySentense)
				.OrElse(GetUndefinedSentense)
				.GetOrDefault(SentenseType.Default);

		}

		private Maybe<SentenseType> GetDefaultSentense()
		{
			return !Container.OriginalString.Contains("...") && Container.OriginalString.EndsWith(".") ? SentenseType.Default.ToMaybe() : Maybe.Nothing;
		}

		private Maybe<SentenseType> GetIterrogativeSentense()
		{
			return Container.OriginalString.EndsWith("?") ? SentenseType.Interrogative.ToMaybe() : Maybe.Nothing;
		}

		private Maybe<SentenseType> GetExclamatorySentense()
		{
			return Container.OriginalString.EndsWith("!") ? SentenseType.Exclamatory.ToMaybe() : Maybe.Nothing;
		}

		private Maybe<SentenseType> GetUndefinedSentense()
		{
			return Container.OriginalString.EndsWith("...") ? SentenseType.Undefined.ToMaybe() : Maybe.Nothing;
		}

		public Word[] GetWords()
		{
			var buffer = Container.OriginalString
				.Replace(",", " ")
				.Replace(";", " ")
				.Replace(":", " ")
				.Replace(".", " ")
				.Replace("!", " ")
				.Replace("?", " ")
				.Replace("\"", " ")
				.Replace("-", " ");
			
			return buffer
				.Split(' ')
				.Where(c => c != null && !string.IsNullOrEmpty(c))
				.Select(c => new Word(buffer.IndexOf(c), c, new SimpleWordAnalizerManager(new SimpleAnalizerContainer(), this.Container.Language), this.Container.Current)).ToArray();
		}

		public Sign[] GetSigns()
		{
			var buffer = new CharWithMetaManager(Container.OriginalString.ToCharArray().ToList()).GetCharsWithMeta();
			var bufferList = buffer.SelectMany(c=>Sign.SignDictionary,(currentChar,sign)=>new {currentChar,sign})
				.Where(@t => @t.currentChar.CurrentChar == @t.sign.Value)
				.Select(
					@t =>
						new Sign(@t.currentChar.CurrentIndex, @t.sign.Value.ToString(), new SimpleSignManager(new SimpleAnalizerContainer(), this.Container.Language),
							this.Container.Current)).ToList();
			//var bufferList = (Sign.SignDictionary.SelectMany(sign => buffer, (sign, currentChar) => new {sign, currentChar})
			//	.Where(@t => @t.sign.Value == @t.currentChar)
			//	.Select(
			//		@t =>
			//			new Sign(Container.OriginalString.IndexOf(@t.sign.Value), @t.sign.Value.ToString(), new SimpleSignManager(new SimpleAnalizerContainer(), this.Container.Language),
			//				this.Container.Current))).ToList();
			return bufferList.ToArray();
		}

		public BaseTermin[] GetSequence()
		{
			var words = GetWords();
			var signs = GetSigns();
			var result = words.Concat<BaseTermin>(signs).ToList();
				result.Sort(((termin, baseTermin) => termin.Index.CompareTo(baseTermin.Index)));
			return result.ToArray();
		}

		public IEnumerable<Word> GetPossibleNames()
		{
			var words = GetWords();
			return words.Where(c => c.StartWithCappitalLetter());
		}
	}

	internal class CharWithCustomMeta
	{
		public char CurrentChar { get; private set; }
		public int? CurrentIndex { get; private set; }

		public CharWithCustomMeta(char currentChar, int? index)
		{
			CurrentChar = currentChar;
			CurrentIndex = index;
		}
	}

	internal class CharWithMetaManager
	{
		private List<char> _chars;
		private static readonly Dictionary<SignEnum, char> _signDictionary = new Dictionary<SignEnum, char>
		{
			{SignEnum.Point,'.'},
			{SignEnum.Comma,','},
			{SignEnum.Excamation,'!'},
			{SignEnum.Question,'?'},
			{SignEnum.Colon,':'},
			{SignEnum.Semicolon,';'},
			{SignEnum.Quotes,'"'},
			{SignEnum.Dash,'-'}
		};
		public CharWithMetaManager(List<char> chars)
		{
			_chars = chars;
		}

		public List<CharWithCustomMeta> GetCharsWithMeta()
		{
			var list = new List<CharWithCustomMeta>();
			for (int i = 0; i < _chars.Count(); i++)
			{
				if (_signDictionary.FirstOrDefault(c => c.Value.Equals(_chars[i])).Value != default(char))
				{
					list.Add(new CharWithCustomMeta(_chars[i], i));
				}
				else
				{
					list.Add(new CharWithCustomMeta(_chars[i], null));
				}
			}
			return list;
		}
	}
}
