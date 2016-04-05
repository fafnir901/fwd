using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using FWD.BusinessObjects.Domain;
using WebRock.Utils.Monad;

namespace FWD.BusinessObjects.Xml
{
	[Serializable]
	public class XmlArticle : XmlBase<XmlArticle, Article>
	{
		[XmlElement]
		public Article Article { get; set; }

	}
}
