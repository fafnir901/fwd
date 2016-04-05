using Text.Analizer.Termins;

namespace Text.Analizer.Strategies
{
	public interface IAnalizerContainer
	{
		string OriginalString { get;}
		string Language { get; set; }
		BaseTermin Current { get; set; }
		BaseTermin Parent { get; set; }
		ILanguageRule Rules { get;}
		void SetOriginalString(string str, bool isHtml = false);
	}
}
