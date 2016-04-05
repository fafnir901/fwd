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
	public class CurrentPlan : IPlan
	{
		[XmlAttribute]
		public int Id { get; set; }
		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public string Description { get; set; }
		[XmlAttribute]
		public DateTime AddedDate { get; set; }
		[XmlAttribute]
		public DateTime? PossibleChangeDate { get; set; }
		[XmlAttribute]
		public bool IsDone { get; set; }
		public IPlan Clone()
		{
			return (IPlan)this.MemberwiseClone();
		}
	}
}
