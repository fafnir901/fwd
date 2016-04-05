using Text.Analizer.Implementation;

namespace Text.Analizer.Termins
{
	public class Letter:BaseTermin
	{
		public Letter(string letter, BaseAnalizerManager manager, BaseTermin parent, bool isHtml = false)
			: base(manager, parent)
		{
			manager.Container.SetOriginalString(letter, isHtml);//OriginalString = letter;
			manager.Container.Parent = parent;
			manager.Container.Current = this;
		}

	}
}
