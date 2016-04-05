using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FWD.BusinessObjects.Absrtact;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class Tag : ITag
	{
		[XmlAttribute]
		public int Id { get; set; }
		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public int TagType { get; set; }
		[XmlAttribute]
		public string TagColor { get; set; }
		[XmlAttribute]
		public int Priority { get; set; }

		//public override string ToString()
		//{
		//	var inner = string.Format("\"Id\":\"{0}\",\"Name\":\"{1}\",\"TagType\":\"{2}\",\"TagColor\":\"{3}\",\"Priority\":\"{4}\"", Id, Name, TagType, TagColor, Priority);
		//	return inner;
		//}
	}
}
