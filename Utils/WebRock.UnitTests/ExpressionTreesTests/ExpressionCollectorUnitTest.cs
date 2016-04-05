using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.Utils.ExpressionTrees;
using WebRock.Utils.ExpressionTrees.Models;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;

namespace WebRock.UnitTests.ExpressionTreesTests
{
	[TestClass]
	public class ExpressionCollectorUnitTest
	{
		private class User
		{
			public string Name { get; set; }
			public string Login { get; set; }
			public DateTime RegisterDate { get; set; }
			public bool ForTest { get; set; }
		}

		private class UserDto
		{
			public string FirstName { get; set; }
			public string FirstLogin { get; set; }
			public DateTime RegisterDateOne { get; set; }
			public bool ForTest { get; set; }
		}

		private IEnumerable<User> GetCurrentUser(Expression<Func<UserDto, bool>> predicate, IEnumerable<User> users)
		{
			var mapper = new SimpleMapper<UserDto, User>();
			mapper.AddMapping(c => c.FirstName, c => c.Name);
			mapper.AddMapping(c => c.FirstLogin, c => c.Login);
			mapper.AddMapping(c => c.RegisterDateOne, c => c.RegisterDate);
			return users.Where(predicate.Convert<User, UserDto>(mapper).Compile());
		}

		[TestMethod]
		public void AddExpressionTest()
		{
			var eCollector = new ExpressionCollector<User>();
			eCollector.AddExpression(new ExpressionAdderModel<User>
			{
				Expression = c => c.Name == "123",
				CurrentTypeOfExpression = ExpressionType.AndAlso
			});
			eCollector.AddExpression(c=>c.Login.Contains("P"),ExpressionType.AndAlso);
			eCollector.AddExpression(new ExpressionAdderModel<User>
			{
				Expression = c => !c.ForTest,
				CurrentTypeOfExpression = ExpressionType.OrElse
			});
			Assert.AreEqual(true, eCollector.CurrentListOfExpression.Count == 3);
		}

		[TestMethod]
		public void RemoveExpressionTest()
		{
			var eCollector = new ExpressionCollector<User>();
			eCollector.AddExpression(new ExpressionAdderModel<User>
			{
				Expression = c => c.Name == "123",
				CurrentTypeOfExpression = ExpressionType.AndAlso
			});
			eCollector.AddExpression(new ExpressionAdderModel<User>
			{
				Expression = c => c.Login.Contains("P"),
				CurrentTypeOfExpression = ExpressionType.AndAlso
			});
			eCollector.AddExpression(new ExpressionAdderModel<User>
			{
				Expression = c => !c.ForTest,
				CurrentTypeOfExpression = ExpressionType.OrElse
			});
			eCollector.RemoveExpression(new ExpressionAdderModel<User>
			{
				Expression = c => c.Name == "123",
				CurrentTypeOfExpression = ExpressionType.AndAlso
			});
			Assert.AreEqual(true, eCollector.CurrentListOfExpression.Count == 2);
		}

		[TestMethod]
		public void CollectTest()
		{
			var eCollector = new ExpressionCollector<UserDto>();
			eCollector.AddExpression(new ExpressionAdderModel<UserDto>
			{
				Expression = c => c.FirstName == "123",
				CurrentTypeOfExpression = ExpressionType.AndAlso
			});
			eCollector.AddExpression(new ExpressionAdderModel<UserDto>
			{
				Expression = c => c.FirstLogin.Contains("P"),
				CurrentTypeOfExpression = ExpressionType.AndAlso
			});
			eCollector.AddExpression(new ExpressionAdderModel<UserDto>
			{
				Expression = c => !c.ForTest,
				CurrentTypeOfExpression = ExpressionType.Or
			});
			var resuylt = eCollector.Collect();
			Assert.AreEqual(typeof(bool), resuylt.Body.MaybeAs<BinaryExpression>().Bind(c => c.Left.Type).GetOrDefault(null));

			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "Petia",
					Name = "123",
					RegisterDate = DateTime.Now,
					ForTest = false
					
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today,
					ForTest = true
				}
			};

			var result = GetCurrentUser(resuylt, lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual(true,result.Any());
			Assert.AreEqual("Petia", result[0].Login);
		}

		[TestMethod]
		public void SimpleCollectTest()
		{
			var eCollector = new ExpressionCollector<UserDto>();
			eCollector.AddExpression(new ExpressionAdderModel<UserDto>
			{
				Expression = c => c.FirstName == "123",
				CurrentTypeOfExpression = ExpressionType.AndAlso
			});
			var resuylt = eCollector.Collect();
			Assert.AreEqual(typeof(string),resuylt.Body.MaybeAs<BinaryExpression>().Bind(c=>c.Left.Type).GetOrDefault(null));
		}
	}
}
