using System;
using System.Xml.Serialization;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class Image
	{
		[XmlAttribute]
		public int ImageId { get; set; }
		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public byte[] Data { get; set; }

		public Image Clone()
		{
			return (Image)this.MemberwiseClone();
		}
	}
}
