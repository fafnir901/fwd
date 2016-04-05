namespace FWD.UI.Web.Models.Entities.Dtos
{
	public class ImageDto:IDto
	{
		public string ImageName { get; set; }
		public int ImageId { get; set; }

		public string Type
		{
			get { return "image"; }
		}

	}
}