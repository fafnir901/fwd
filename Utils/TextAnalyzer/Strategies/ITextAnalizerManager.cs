using Text.Analizer.Termins;

namespace Text.Analizer.Strategies
{
	public interface ITextAnalizerManager : IEntityContainer
	{
		Sentense[] GetSentenses();
	}


	public enum SentenseType
	{
		Default,
		//Удтвердительное
		/// <summary>
		/// Russion:Вопросительное
		/// </summary>
		Interrogative,
		/// <summary>
		/// Russion:Удтвердительное
		/// </summary>
		Exclamatory,
		Undefined
	}
}
