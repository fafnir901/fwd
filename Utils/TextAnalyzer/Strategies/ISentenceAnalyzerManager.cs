using System.Collections.Generic;
using Text.Analizer.Termins;

namespace Text.Analizer.Strategies
{
	public interface ISentenceAnalyzerManager:IEntityContainer
	{
		SentenseType GetSentenseType();
		Word[] GetWords();
		Sign[] GetSigns();
		IEnumerable<Word> GetPossibleNames();
		BaseTermin[] GetSequence();
	}
}
