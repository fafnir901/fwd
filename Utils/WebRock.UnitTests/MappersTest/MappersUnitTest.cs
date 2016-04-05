using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.Utils.Mappers;

namespace WebRock.UnitTests.MappersTest
{
	[TestClass]
	public class MappersUnitTest
	{
		private class User
		{
			public string Name { get; set; }
			public string Login { get; set; }
			public DateTime RegisterDate { get; set; }
		}

		private class UserDto
		{
			public string FirstName { get; set; }
			public string Login { get; set; }
			public DateTime RegisterDate { get; set; }
		}

		[TestMethod]
		public void SimpleMapperTest()
		{
			var simpleMapper = new SimpleMapper<UserDto, User>();
			simpleMapper.AddMapping(c => c.FirstName, c => c.Name);
			var result = simpleMapper.GetMappedProperties();
			Assert.AreEqual(true,result.Any());
			var rs = simpleMapper.GetPropertyFromMapping("FirstName");
			Assert.IsNotNull(rs);
		}
	}
}
