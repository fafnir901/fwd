using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.UnitTests.TestData;
using WebRock.Utils.ExpressionTrees.Internals;
using WebRock.Utils.Monad;

namespace WebRock.UnitTests.ExpressionTreesTests
{
	[TestClass]
	public class MemberExpressionHelperTest
	{
		[TestMethod]
		public void GetMemberNameTest()
		{
			var memberHelper = new MemberExpressionHelper();

			Expression<Func<User, bool>> expression = user => user.IsActive == true;

			Assert.AreEqual("IsActive", memberHelper.GetMemberName(expression.Body.MaybeAs<BinaryExpression>().Bind(c => c.Left).GetOrDefault(null)));

			expression = user => user.IsActive;
			Assert.AreEqual("IsActive", memberHelper.GetMemberName(expression.Body.MaybeAs<MemberExpression>().GetOrDefault(null)));
			expression = user => user.IsActive.Equals(true);
			Assert.AreEqual("IsActive", memberHelper.GetMemberName(expression.Body.MaybeAs<MethodCallExpression>().Bind(c => c.Object).GetOrDefault(null)));
			expression = user => user.Credentials.Password == "123";
			Assert.AreEqual("Password", memberHelper.GetMemberName(expression.Body.MaybeAs<BinaryExpression>().Bind(c => c.Left).GetOrDefault(null)));
			expression = user => user.Credentials != null;
			Assert.AreEqual("Credentials", memberHelper.GetMemberName(expression.Body.MaybeAs<BinaryExpression>().Bind(c => c.Left).GetOrDefault(null)));
		}

		[TestMethod]
		public void GetMemberTypeTest()
		{
			var memberHelper = new MemberExpressionHelper();

			Expression<Func<User, bool>> expression = user => user.IsActive == true;
			Assert.AreEqual(typeof(bool), memberHelper.GetMemberType(expression.Body.MaybeAs<BinaryExpression>().Bind(c => c.Left).GetOrDefault(null)));
			expression = user => user.IsActive;
			var t = expression.Body.MaybeAs<MemberExpression>().GetOrDefault(null);
			Assert.AreEqual(typeof(bool), memberHelper.GetMemberType(t));
		}
	}
}
