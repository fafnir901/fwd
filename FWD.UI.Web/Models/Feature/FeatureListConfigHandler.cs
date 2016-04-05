using System.Configuration;

namespace FWD.UI.Web.Models.Feature
{
	public class FeatureListConfigHandler : ConfigurationSection
	{
		[ConfigurationProperty("Features")]
		public FeatureCollection FeatureItems
		{
			get { return ((FeatureCollection)(base["Features"])); }
		}
	}
}