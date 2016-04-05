using System;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.Utils.Mappers;

namespace WebRock.UnitTests.MappersTest
{
	[TestClass]
	public class XmlMapperUnitTest
	{
		private class Current
		{
			public string Name { get; set; }
			public int Age { get; set; }
			public Credential Credential { get; set; }
		}

		private class Credential
		{
			public string Key { get; set; }
		}

		[TestMethod]
		public void TestMethod1()
		{
			string xml = @"<Current Name=""Poul"" Age=""21""><Credential Key=""123""/></Current>";

			var mapper = new XmlMapper<Current>();
			mapper.AddMapping("Name", c => c.Name);
			mapper.AddMapping("Age", c => c.Age);
			mapper.AddMapping("Credential", c => c.Credential);
			mapper.AddMapping("Key", c => c.Credential.Key);
			var current = new Current();
			var doc = XDocument.Parse(xml);
			var result = doc.Elements().Where(c => c.Attribute("Name") != null);
			current.GetType().GetProperty("Name").SetValue(current, result.First().Attribute("Name").Value);
			current.GetType().GetProperty("Age").SetValue(current, int.Parse(result.First().Attribute("Age").Value));
			Assert.AreEqual("Poul",current.Name);
		}
	}
}
