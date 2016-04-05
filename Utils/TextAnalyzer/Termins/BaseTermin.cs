using Text.Analizer.Strategies;

namespace Text.Analizer.Termins
{
	public abstract class BaseTermin
	{
		public IEntityContainer EntityManager { get; private set; }

		protected BaseTermin(IEntityContainer textAnalyzerManager, BaseTermin current)
		{
			EntityManager = textAnalyzerManager;
			Current = current;
		}

		public BaseTermin Current { get; set; }

		public virtual int Index {
			get { return -1; }
		}

		public virtual string Value
		{
			get
			{
				return EntityManager.Container.OriginalString;
			}
		}

		public virtual BaseTermin Parent
		{
			get
			{
				return EntityManager.Container.Parent;
			}
		}

		public virtual ILanguageRule Rules
		{
			get
			{
				return EntityManager.Container.Rules;
			}
		}

	}
}
