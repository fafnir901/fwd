using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using WebRock.Utils.Monad;

namespace Text.Analizer.Termins
{
	public class Sentense : BaseTermin
	{
		public Sentense(string str, ISentenceAnalyzerManager manager, BaseTermin parent, bool isHtml = false)
			: base(manager, parent)
		{
			manager.Container.SetOriginalString(str, isHtml);//OriginalString = str;
			manager.Container.Parent = parent;
			manager.Container.Current = this;

		}

		private IEnumerable<Word> _words;
		private IEnumerable<Sign> _signs;
		private IEnumerable<BaseTermin> _termins;
		public IEnumerable<Word> Words
		{
			get
			{
				if (_words == null || !_words.Any())
					_words = EntityManager.MaybeAs<ISentenceAnalyzerManager>().Bind(c => c.GetWords()).GetOrDefault(null);
				return _words;
			}
		}

		public IEnumerable<Sign> Signs
		{
			get
			{
				if (_signs == null || !_signs.Any())
					_signs = EntityManager.MaybeAs<ISentenceAnalyzerManager>().Bind(c => c.GetSigns()).GetOrDefault(null);
				return _signs;
			}
		}
		public IEnumerable<BaseTermin> Sequence
		{
			get
			{
				if (_termins == null || !_termins.Any())
					_termins = EntityManager.MaybeAs<ISentenceAnalyzerManager>().Bind(c => c.GetSequence()).GetOrDefault(null);
				return _termins;
			}
		}

		public SentenseType SentenseType
		{
			get
			{
				var type = EntityManager.MaybeAs<ISentenceAnalyzerManager>().Bind(c => c.GetSentenseType()).GetOrDefault(SentenseType.Default);
				return type;
			}
		}

		public override string ToString()
		{
			return EntityManager.Container.OriginalString;
		}
	}
}
