using System.Collections.Generic;

namespace FWD.UI.Web.Models.Entities
{
	internal class StatViewModelWrapper
	{
		public List<StatViewModel> TranViewModels { get; set; }
		public int TotalCount { get; set; }

		public double ReadablePercent { get; set; }
		public double UpdatablePercent { get; set; }
		public double WritablePercent { get; set; }
		public double DeletablePercent { get; set; }

		public int ReadableCount { get; set; }
		public int UpdatableCount { get; set; }
		public int WritableCount { get; set; }
		public int DeletableCount { get; set; }
	}
}