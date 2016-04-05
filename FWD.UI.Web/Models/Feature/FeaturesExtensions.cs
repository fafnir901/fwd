namespace FWD.UI.Web.Models.Feature
{
	public static class FeaturesExtensions
	{
		public static string GetName(this Features item)
		{
			string str = string.Empty;
			switch (item)
			{
				case Features.Add:
					str = "Add";
					break;
				case Features.SwitchToXml:
					str = "SwitchToXml";
					break;
				case Features.Edit:
					str = "Edit";
					break;
				case Features.SwitchAnotherView:
					str = "SwitchAnotherView";
					break;
				case Features.SwitchEmail:
					str = "SwitchEmail";
					break;
				case Features.SaveToXml:
					str = "SaveToXml";
					break;
				case Features.SaveToDb:
					str = "SaveToDb";
					break;
				case Features.ShowComments:
					str = "ShowComments";
					break;
				case Features.Statistics:
					str = "Statistics";
					break;
				case Features.Information:
					str = "Information";
					break;
				case Features.Export:
					str = "Export";
					break;
				case Features.Import:
					str = "Import";
					break;
				case Features.Tranc:
					str = "Tranc";
					break;
				case Features.GetAtricleById:
					str = "GetAtricleById";
					break;
				case Features.ShowListOfAtricles:
					str = "ShowListOfAtricles";
					break;
				case Features.Plan:
					str = "Plan";
					break;
				case Features.Search:
					str = "Search";
					break;
				case Features.Remove:
					str = "Remove";
					break;
				case Features.Reminder:
					str = "Reminder";
					break;
				case Features.Tag:
					str = "Tag";
					break;
			}
			return str;
		}
	}
}