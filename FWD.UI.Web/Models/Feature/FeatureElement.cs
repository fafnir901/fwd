using System.Configuration;

namespace FWD.UI.Web.Models.Feature
{
	public class FeatureElement : ConfigurationElement
	{

		[ConfigurationProperty("Name", DefaultValue = "", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return ((string)(base["Name"])); }
			set { base["Name"] = value; }
		}

		[ConfigurationProperty("IsEnabled", DefaultValue = "true", IsKey = false, IsRequired = false)]
		public bool IsEnabled
		{
			get { return ((bool)(base["IsEnabled"])); }
			set { base["IsEnabled"] = value; }
		}
	}
}