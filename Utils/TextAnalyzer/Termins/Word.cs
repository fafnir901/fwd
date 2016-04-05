using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using WebRock.Utils.Monad;

namespace Text.Analizer.Termins
{
	public class Word : BaseTermin
	{
		private int _index;
		public Word(int index, string word, IWordAnalizerManager manager, BaseTermin parent, bool isHtml = false)
			: base(manager, parent)
		{
			manager.Container.SetOriginalString(word,isHtml);//OriginalString = word;
			manager.Container.Current = this;
			manager.Container.Parent = parent;
			_index = index;
		}

		public IEnumerable<Letter> _letters;

		public override int Index
		{
			get { return _index; }
		}

		public IEnumerable<Letter> Letters
		{
			get
			{
				if (_letters == null || !_letters.Any())
					_letters = EntityManager.MaybeAs<IWordAnalizerManager>().Bind(c => c.GetLetters()).GetOrDefault(null);
				return _letters;
			}
		}

		public bool StartWithCappitalLetter()
		{
			return EntityManager
				.MaybeAs<IWordAnalizerManager>()
				.Bind(c => c.StartWithCappitalLetter())
				.GetOrDefault(false);
		}

		public bool IsName()
		{
			return EntityManager
				.MaybeAs<IWordAnalizerManager>()
				.Bind(c => c.IsName())
				.GetOrDefault(false);
		}

	}
}
