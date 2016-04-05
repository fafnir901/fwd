using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FWD.BusinessObjects.Domain;

namespace FWD.BusinessObjects.Xml
{
	[Serializable]
	public class XmlArtcileGroup : XmlBase<XmlArtcileGroup, ArticleGroup>
	{
		[XmlElement]
		public ArticleGroup ArticleGroup { get; set; }
	}
}
