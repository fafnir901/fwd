using System;
using System.Linq;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;
using WebRock.Utils.Monad;

namespace Text.Analizer.Implementation
{
	public class SimpleWordAnalizerManager:BaseAnalizerManager,IWordAnalizerManager
	{

		public SimpleWordAnalizerManager(IAnalizerContainer container, string language = null)
			: base(container, language)
		{
		}

		private Maybe<Word> Current
		{
			get
			{
				return Container.Current.MaybeAs<Word>().GetOrDefault(null);
			}
		}

		private Maybe<Sentense> Parent
		{
			get
			{
				return Container.Parent.MaybeAs<Sentense>();
			}
		}

		public bool IsName()
		{
			var word = Current.GetOrDefault(null);
			if (word == null)
			{
				throw new ArgumentException("This method can be appled only into Word entity.");
			}
			var indexCheck = Parent.Bind(c => c.Words.ToList().IndexOf(word)).Bind(c => c > 0).GetOrDefault(false);
			return indexCheck && StartWithCappitalLetter();
		}

		public int GetWordIndex()
		{
			var words = Parent.Bind(c=>c.Words).GetOrDefault(null);
			return words.ToList().IndexOf(Current.GetOrDefault(null));
		}

		public bool StartWithCappitalLetter()
		{
			var word = Current.GetOrDefault(null);
			if (word == null)
			{
				throw new ArgumentException("This method can be appled only into Word entity.");
			}
			return Container.Rules.StartWithUpper(word);
		}

		public Letter[] GetLetters()
		{
			return Container.OriginalString.ToCharArray()
				.Select(c => new Letter(c.ToString(), new SimpleTextAnalizerManager(new SimpleAnalizerContainer(), this.Container.Language), this.Container.Current))
				.ToArray();
		}
	}
}
