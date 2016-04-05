using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebRock.Utils.Monad;

namespace WebRock.Utils.FileUtils
{
	public class XmlUtils
	{
		private XDocument _document;

		public XmlUtils(string xmlData)
		{
			this._document = new XDocument(XDocument.Parse(xmlData));
		}

		public IEnumerable<string> GetValuesByKeyAndAttribute(string key, string attrName)
		{
			return Enumerable.Select<XElement, string>(Enumerable.Where<XElement>(this.GetElemetsByKey(key), (Func<XElement, bool>)(c => c.HasAttributes)), (Func<XElement, string>)(c => MaybeExtension.GetOrDefault<string>(MaybeExtension.Bind<XAttribute, string>(MaybeExtension.MaybeAs<XAttribute>((object)c.Attribute((XName)attrName), true), (Func<XAttribute, string>)(x => x.Value)), (string)null)));
		}

		public IEnumerable<XElement> GetElemetsByKey(string key)
		{
			XElement root = this._document.Root;
			if (root != null)
				return root.Elements((XName)key);
			else
				return Enumerable.Empty<XElement>();
		}

		public IEnumerable<string> GetValuesByKeyAndPredicate(string key, Func<XElement, bool> preFunc)
		{
			return Enumerable.SelectMany<XElement, string>(Enumerable.Where<XElement>(this.GetElemetsByKey(key), preFunc), (Func<XElement, IEnumerable<string>>)(c => Enumerable.Select<XAttribute, string>(c.Attributes(), (Func<XAttribute, string>)(x => MaybeExtension.GetOrDefault<string>(MaybeExtension.Bind<XAttribute, string>(MaybeExtension.MaybeAs<XAttribute>((object)x, true), (Func<XAttribute, string>)(z => z.Value)), (string)null)))));
		}
	}
}
