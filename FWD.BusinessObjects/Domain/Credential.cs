using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class Credential
	{
		[XmlAttribute]
		public string Password { get; set; }
		[XmlAttribute]
		public string Login { get; set; }

		public Credential Clone()
		{
			return (Credential)MemberwiseClone();
		}
	}
}
