using System;

namespace FWD.UI.Web.Models.Entities
{
	public class ForPanel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Rate { get; set; }
		public DateTime CreationDate { get; set; }
		public int CountOfImages { get; set; }
		public string Tags { get; set; }
	}
}