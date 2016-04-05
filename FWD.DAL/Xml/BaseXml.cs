using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FWD.DAL.Xml
{
	public abstract class BaseXml
	{
		public XDocument Document { get; set; }

		protected StringBuilder _builder;

		public const string FOLDER = @"XML";

		protected BaseXml(string elementName, string path, bool isForce)
		{
			try
			{
				if (isForce)
				{
					RewriteXmlPath(elementName, path);
				}
				else
				{
					_builder = new StringBuilder();
					if (!Directory.Exists(FOLDER))
					{
						Directory.CreateDirectory(FOLDER);
					}
					if (!File.Exists(Path))
					{
						Document = new XDocument(new XElement(elementName));
						Document.Save(Path);
					}
					Document = XDocument.Load(Path);
				}

			}
			catch (Exception)
			{
				RewriteXmlPath(elementName, path);
			}

		}

		private void RewriteXmlPath(string elementName, string path)
		{
			var currentPath = path + FOLDER;
			var fullPath = path + Path;
			if (!Directory.Exists(currentPath))
			{
				Directory.CreateDirectory(currentPath);
			}
			if (!File.Exists(fullPath))
			{
				Document = new XDocument(new XElement(elementName));
				Document.Save(fullPath);
			}
			Document = XDocument.Load(fullPath);
			Path = fullPath;
		}

		private string _path;
		public virtual string Path
		{
			get
			{
				if (string.IsNullOrEmpty(_path))
				{
					_path = FOLDER + string.Format(@"\{0}", FileName);
				}
				return _path;
			}
			private set
			{
				_path = value;
			}
		}

		public virtual string FileName
		{
			get
			{
				return "data.xml";
			}
		}

		public void AppendElement(XElement element, string attributeId, string innerAttributeId)
		{
			if (Document.Root != null && Document.Root.LastNode != null)
			{
				MakeIdentity(element, attributeId, innerAttributeId);
				Document.Root.LastNode.AddAfterSelf(element);
			}
			else if (Document.Root != null)
			{
				MakeIdentity(element, attributeId, innerAttributeId);
				Document.Root.Add(element);
			}
		}

		private void MakeIdentity(XElement element, string attributeId, string innerAttributeId)
		{
			if (Document.Root.LastNode != null)
			{
				var lastValue = int.Parse(Document.Root.Elements().Last().Attribute(attributeId).Value);
				element.Attribute(attributeId).SetValue(++lastValue);
			}
			else
			{
				element.Attribute(attributeId).SetValue(1);
			}
			if (!string.IsNullOrEmpty(innerAttributeId))
			{
				int prevId = 0;
				bool firstTime = true;
				foreach (var xElement in element.Elements())
				{
					MakeIdentityForInnerEntity(xElement, innerAttributeId, ref prevId, ref firstTime);
				}
			}
		}

		private void MakeIdentityForInnerEntity(XElement element, string attributeId, ref int prevId, ref bool firstTime)
		{
			if (firstTime)
			{
				if (Document.Root.Elements().SelectMany(c => c.Elements()).Any())
				{
					prevId = Document.Root.Elements()
						.SelectMany(c => c.Elements().Where(x => x.HasAttributes)).Max(c => int.Parse(c.Attribute(attributeId).Value));
				}
				firstTime = false;
			}

			element.Attribute(attributeId).SetValue(++prevId);
		}

		public void SetAttributeValue(XElement element, Dictionary<string, string> nameAndValues)
		{
			foreach (var nameAndValue in nameAndValues)
			{
				if (element.Attribute(nameAndValue.Key) == null)
				{
					element.Add(new XAttribute(nameAndValue.Key, ""));
				}
				element.Attribute(nameAndValue.Key).SetValue(nameAndValue.Value ?? "");
			}
		}
	}
}
