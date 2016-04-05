using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.Utils.Monad;

namespace WebRock.UnitTests.MonadsTests.MaybeTest
{
	[TestClass]
	public class MaybeUnitTest
	{

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CreateMaybeTest()
		{
			var lst = new List<string>();
			var maybeListOfString = lst.MaybeAs<List<string>>();
			Assert.AreEqual(true, maybeListOfString.HasValue);
			Assert.AreEqual(lst, maybeListOfString.Value);

			var maybeListOfInt = lst.MaybeAs<List<int>>();
			Assert.AreEqual(false, maybeListOfInt.HasValue);
			Assert.AreEqual(Maybe.Nothing, maybeListOfInt.Value);
		}

		[TestMethod]
		public void MaybeIfTest()
		{
			var name = "Vasia";
			var maybe = name.MaybeAs<string>();
			var result = maybe.If(c => c.StartsWith("V")).GetOrDefault(null);
			Assert.IsNotNull(result);
			result = maybe.If(c => c.EndsWith("ppp")).GetOrDefault(null);
			Assert.AreEqual(null,result);
		}

		[TestMethod]
		public void MaybeSelectTest()
		{
			var lst = new List<string> { "Fedia", "Vasia", "Petia" };
			var maybe = lst.MaybeAs<List<string>>();
			var result = maybe.Select(c => c.Where(p => p.Contains("V")).ToMaybe()).GetOrDefault(null);
			Assert.IsNotNull(result);
			Assert.AreEqual(true,result.Any());
			Assert.AreEqual("Vasia",result.ToList()[0]);
		}
	}
}
