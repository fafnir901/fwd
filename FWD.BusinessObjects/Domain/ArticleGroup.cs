using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class ArticleGroup
	{
		[XmlAttribute]
		public int GroupId { get; set; }
		[XmlAttribute]
		public string GroupName { get; set; }
		[XmlElement]
		public List<string> Groups { get; set; }

		[XmlIgnore]
		public List<Article> Articles { get; set; }
	}
}
