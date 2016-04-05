using System;
using System.Collections.Generic;
using System.Configuration;

namespace FWD.UI.Web.Models.Feature
{
	public class FeatureListModel
	{
		private static readonly Lazy<FeatureListModel> _instance = new Lazy<FeatureListModel>(() => new FeatureListModel());

		private FeatureListModel()
		{
			FeatureList = new Dictionary<string, bool>();
		}

		public Dictionary<string, bool> FeatureList { get; set; }

		public static FeatureListModel Instance
		{
			get
			{
				var ins = _instance.Value;
				ins.LoadFeatureList();
				return ins;
			}
		}

		private void LoadFeatureList()
		{
			if (FeatureList.Count == 0)
			{
				var section = (FeatureListConfigHandler)ConfigurationManager.GetSection("FeatureList");
				if (section != null)
				{
					foreach (FeatureElement featureItem in section.FeatureItems)
					{
						if (!FeatureList.ContainsKey(featureItem.Name))
							FeatureList.Add(featureItem.Name, featureItem.IsEnabled);
					}
				}
			}
		}

		public void ReloadFeatureList()
		{
			FeatureList = new Dictionary<string, bool>();
			LoadFeatureList();
		}

		public bool CheckFeatureForEnabled(Features item, bool needToLoad = false)
		{
			if (needToLoad)
			{
				FeatureList = new Dictionary<string, bool>();
				LoadFeatureList();
			}
			return FeatureList[item.GetName()];
		}
	}
}