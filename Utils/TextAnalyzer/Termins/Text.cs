using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using WebRock.Utils.Monad;

namespace Text.Analizer.Termins
{
	public class Text : BaseTermin
	{
		public Text(string text, ITextAnalizerManager textAnalyzerManager, BaseTermin current = null, bool isHtml = false)
			: base(textAnalyzerManager, current)
		{
			textAnalyzerManager.Container.SetOriginalString(text, isHtml);//OriginalString = text;
			textAnalyzerManager.Container.Current = this;
		}

		private IEnumerable<Sentense> _sentenses;
		public IEnumerable<Sentense> Sentenses
		{
			get
			{
				if (_sentenses == null || !_sentenses.Any())
				{
					_sentenses = EntityManager.MaybeAs<ITextAnalizerManager>().Bind(c => c.GetSentenses()).GetOrDefault(null);
				}
				return _sentenses;
			}
		}

	}
}
