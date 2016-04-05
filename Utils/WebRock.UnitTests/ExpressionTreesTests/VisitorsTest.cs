using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.UnitTests.TestData;
using WebRock.Utils.ExpressionTrees.ExpressionVisitors;

namespace WebRock.UnitTests.ExpressionTreesTests
{
	[TestClass]
	public class VisitorsTest
	{
		[TestMethod]
		public void CountOfLists()
		{
			var visitor = new WebRockExpressionConverterVisitor();
			Expression<Func<User, bool>> expression = user => user.IsActive;
			visitor.Visit(expression);

			Assert.AreEqual(true,visitor.Lefts.Count==1);
			Assert.AreEqual(true, visitor.Rights.Count == 0);
			Assert.AreEqual(true, visitor.LastLevelNodeTypes.Count == 0);
			Assert.AreEqual(true, visitor.ConcatNodeTypes.Count == 0);


			visitor = new WebRockExpressionConverterVisitor();
			expression = user => user.IsActive && user.Name=="123";
			visitor.Visit(expression);
			Assert.AreEqual(true, visitor.Lefts.Count == 2);
			Assert.AreEqual(true, visitor.Rights.Count == 1);
			Assert.AreEqual(true, visitor.ConcatNodeTypes.Count == 1);

			visitor = new WebRockExpressionConverterVisitor();
			expression = user => user.Name.Contains("123");
			visitor.Visit(expression);
			Assert.AreEqual(true, visitor.Lefts.Count == 1);
			Assert.AreEqual(true, visitor.Rights.Count == 1);

			visitor = new WebRockExpressionConverterVisitor();
			expression = user => user.Credentials.Password == "1";
			visitor.Visit(expression);
			Assert.AreEqual(true, visitor.Lefts.Count == 2);
			Assert.AreEqual(true, visitor.Rights.Count == 1);

			visitor = new WebRockExpressionConverterVisitor();
			expression = user => user.Credentials.Token.CurrentTokenNumber == "1";
			visitor.Visit(expression);
			Assert.AreEqual(true, visitor.Lefts.Count == 3);
			Assert.AreEqual(true, visitor.Rights.Count == 1);

			visitor = new WebRockExpressionConverterVisitor();
			expression = user => user.Activities.FirstOrDefault() !=null;
			visitor.Visit(expression);
			Assert.AreEqual(true, visitor.Lefts.Count == 1);
			Assert.AreEqual(true, visitor.Rights.Count == 1);
			Assert.AreEqual(true, visitor.ConcatNodeTypes.Count == 0);
			Assert.AreEqual(true, visitor.LastLevelNodeTypes.Count == 2);
		}

		[TestMethod]
		public void FullCheck()
		{
			var visitor = new WebRockExpressionConverterVisitor();
			Expression<Func<User, bool>> expression = user => !user.IsActive 
				&& user.Credentials.Token.CurrentTokenNumber.Equals("6") 
				&& user.RegisterDate.ToShortDateString() == "12/10/2012";

			visitor.Visit(expression);

			Assert.AreEqual(5, visitor.Lefts.Count);
			Assert.AreEqual(2, visitor.Rights.Count);
			Assert.AreEqual(4, visitor.LastLevelNodeTypes.Count);
			Assert.AreEqual(2, visitor.ConcatNodeTypes.Count);

			Assert.AreEqual("user.IsActive", visitor.Lefts[0].ToString());
			Assert.AreEqual("user.Credentials.Token.CurrentTokenNumber", visitor.Lefts[1].ToString());
			Assert.AreEqual("user.Credentials.Token", visitor.Lefts[2].ToString());
			Assert.AreEqual("user.Credentials", visitor.Lefts[3].ToString());
			Assert.AreEqual("user.RegisterDate", visitor.Lefts[4].ToString());

			Assert.AreEqual("\"6\"", visitor.Rights[0].ToString());
			Assert.AreEqual("\"12/10/2012\"", visitor.Rights[1].ToString());

			Assert.AreEqual(ExpressionType.Not, visitor.LastLevelNodeTypes[0].ValueOfExpressionType);
			Assert.AreEqual(ExpressionType.Call, visitor.LastLevelNodeTypes[1].ValueOfExpressionType);
			Assert.AreEqual(ExpressionType.Equal, visitor.LastLevelNodeTypes[2].ValueOfExpressionType);
			Assert.AreEqual(ExpressionType.Call, visitor.LastLevelNodeTypes[3].ValueOfExpressionType);

			Assert.AreEqual("UnaryExpression", visitor.LastLevelNodeTypes[0].ExpressionType.Name);
			Assert.AreEqual("MethodCallExpression", visitor.LastLevelNodeTypes[1].ExpressionType.Name);
			Assert.AreEqual("BinaryExpression", visitor.LastLevelNodeTypes[2].ExpressionType.Name);
			Assert.AreEqual("MethodCallExpression", visitor.LastLevelNodeTypes[3].ExpressionType.Name);
		}
	}
}
