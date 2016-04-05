using Text.Analizer.Termins;

namespace Text.Analizer.Strategies
{
	public interface IWordAnalizerManager : IEntityContainer
	{
		bool IsName();
		int GetWordIndex();
		bool StartWithCappitalLetter();
		Letter[] GetLetters();
	}
}
