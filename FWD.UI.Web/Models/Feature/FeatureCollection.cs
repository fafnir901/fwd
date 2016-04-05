using System.Configuration;

namespace FWD.UI.Web.Models.Feature
{
	[ConfigurationCollection(typeof(FeatureElement), AddItemName = "Feature")]
	public class FeatureCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new FeatureElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((FeatureElement)(element)).Name;
		}

		public FeatureElement this[int idx]
		{
			get { return (FeatureElement)BaseGet(idx); }
		}
	}
}