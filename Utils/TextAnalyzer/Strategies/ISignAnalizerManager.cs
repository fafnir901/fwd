using Text.Analizer.Termins;

namespace Text.Analizer.Strategies
{
	public interface ISignAnalizerManager:IEntityContainer
	{
		BaseTermin GetBeforeSign();
		BaseTermin GetAfterSign();
	}
}
