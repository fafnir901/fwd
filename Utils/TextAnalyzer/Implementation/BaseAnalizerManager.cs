using Text.Analizer.Strategies;

namespace Text.Analizer.Implementation
{
	public abstract class BaseAnalizerManager : IEntityContainer
	{
		protected BaseAnalizerManager(IAnalizerContainer container, string language = null)
		{
			Container = container;
			Container.Language = language;
			_bufferedString = container.OriginalString;
		}

		protected string _bufferedString;

		private IAnalizerContainer _container;

		public IAnalizerContainer Container
		{
			get
			{
				if (string.IsNullOrEmpty(_bufferedString))
					_bufferedString = _container.OriginalString;
				return _container;
			}
			set { _container = value; }
		}
	}
}
