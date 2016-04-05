using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.UnitTests.TestData;
using WebRock.Utils.ExpressionTrees.Converter;
using WebRock.Utils.Mappers;

namespace WebRock.UnitTests.ExpressionTreesTests
{
	[TestClass]
	public class SimpleConverterTest
	{
		[TestMethod]
		public void NameTest()
		{
			var mapper = new SimpleMapper<UserDto, User>();
			mapper.AddMapping(c => c.FirstName, c => c.Name);
			mapper.AddMapping(c => c.FirstLogin, c => c.Login);
			mapper.AddMapping(c => c.RegisterDateOne, c => c.RegisterDate);
			mapper.AddMapping(c => c.CredentialsDto, c => c.Credentials);
			mapper.AddMapping(c => c.CredentialsDto.Password, c => c.Credentials.Password);
			mapper.AddMapping(c => c.CredentialsDto.Token, c => c.Credentials.Token);
			mapper.AddMapping(c => c.CredentialsDto.Token.CurrentTokenNumber, c => c.Credentials.Token.CurrentTokenNumber);
			mapper.AddMapping(c => c.ActivitiesDto, c => c.Activities);

			var converter = new SimpleConverter<UserDto, User>(mapper);

			Expression<Func<User, bool>> expression = user => user.IsActive == true;

			converter.Convert(expression);
		}
	}
}
