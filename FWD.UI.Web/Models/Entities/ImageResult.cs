using System;

namespace FWD.UI.Web.Models.Entities
{
	public class ImageResult
	{
		public byte[] ImageData { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public Guid GuidId { get; set; }

		public ImageResult Copy(ImageResult result)
		{
			var img = new ImageResult { ImageData = result.ImageData, Id = result.Id, GuidId = result.GuidId,Name = result.Name};
			return img;
		}

		public ImageResult Clone()
		{
			return (ImageResult)this.MemberwiseClone();
		}
	}
}