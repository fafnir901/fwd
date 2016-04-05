using System;
using System.Xml.Serialization;
using FWD.BusinessObjects.Absrtact;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class Plan : IPlan
	{
		[XmlAttribute]
		public int Id { get; set; }
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public DateTime AddedDate { get; set; }
		[XmlIgnore]
		public DateTime? PossibleChangeDate { get; set; }

		[XmlAttribute]
		public DateTime ChangedData { get; set; }

		[XmlAttribute]
		public string Description { get; set; }
		[XmlAttribute]
		public bool IsDone { get; set; }

		public IPlan Clone()
		{
			return (Plan)this.MemberwiseClone();
		}
	}
}
