using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebRock.Utils.ExpressionTrees.ExpressionVisitors;
using WebRock.Utils.ExpressionTrees.Models;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Internals
{
	internal class VisitHelper
	{
		public List<LastLevelNodeExpressionTreeModel> VisitLefts(Expression exp)
		{
			var converterVisitor = new WebRockExpressionConverterVisitor();
			var list = new List<LastLevelNodeExpressionTreeModel>();
			converterVisitor.Visit(exp);
			var lefts = converterVisitor.Lefts;
			var rights = converterVisitor.Rights;
			rights.Reverse();

			for (int index = 0; index < lefts.Count; ++index)
			{
				var expression1 = lefts[index];
				var condition = converterVisitor.LastLevelNodeTypes.Count > index
								&& converterVisitor.LastLevelNodeTypes[index].ExpressionType == typeof (UnaryExpression);
				if (condition)
				{
					rights.Insert(index, null);
				}
					
				var expression2 = rights.Count <= index || rights.Count == 0 
					? null 
					: rights[index];

				var expressionType = converterVisitor.LastLevelNodeTypes.Count <= index || converterVisitor.LastLevelNodeTypes.Count == 0 
					? ExpressionType.Throw 
					: converterVisitor.LastLevelNodeTypes[index].ValueOfExpressionType;

				var methodInfo = converterVisitor.LastLevelNodeTypes.Count <= index || converterVisitor.LastLevelNodeTypes.Count == 0 
					? null
					: converterVisitor.LastLevelNodeTypes[index].CurrentMethodInfo;

				list.Add(new LastLevelNodeExpressionTreeModel()
				{
					LeftExpression = expression1,
					RightExpression = expression2,
					ExpressionWithMethodModel = new ExpressionWithMethodModel()
					{
						ValueOfExpressionType = expressionType,
						CurrentMethodInfo = methodInfo
					}
				});
			}
			return list;
		}

		public List<LastLevelNodeExpressionTreeModel> VisitRights(Expression exp)
		{
			WebRockExpressionConverterVisitor converterVisitor = new WebRockExpressionConverterVisitor();
			List<LastLevelNodeExpressionTreeModel> list = new List<LastLevelNodeExpressionTreeModel>();
			converterVisitor.Visit(exp);
			for (int index = 0; index < converterVisitor.Rights.Count; ++index)
			{
				Expression expression1 = converterVisitor.Lefts.Count < index || converterVisitor.Lefts.Count == 0 ? (Expression)null : converterVisitor.Lefts[index];
				Expression expression2 = converterVisitor.Rights[index];
				ExpressionType expressionType = converterVisitor.LastLevelNodeTypes.Count < index || converterVisitor.LastLevelNodeTypes.Count == 0 ? ExpressionType.Throw : converterVisitor.LastLevelNodeTypes[index].ValueOfExpressionType;
				MethodInfo methodInfo = converterVisitor.LastLevelNodeTypes.Count < index || converterVisitor.LastLevelNodeTypes.Count == 0 ? (MethodInfo)null : converterVisitor.LastLevelNodeTypes[index].CurrentMethodInfo;
				list.Add(new LastLevelNodeExpressionTreeModel()
				{
					LeftExpression = expression1,
					RightExpression = expression2,
					ExpressionWithMethodModel = new ExpressionWithMethodModel()
					{
						ValueOfExpressionType = expressionType,
						CurrentMethodInfo = methodInfo
					}
				});
			}
			UnaryExpression orDefault = MaybeExtension.GetOrDefault<UnaryExpression>(MaybeExtension.MaybeAs<UnaryExpression>((object)exp, true), (UnaryExpression)null);
			if (orDefault != null)
				list.Add(new LastLevelNodeExpressionTreeModel()
				{
					LeftExpression = orDefault.Operand,
					RightExpression = (Expression)null,
					ExpressionWithMethodModel = new ExpressionWithMethodModel()
					{
						ValueOfExpressionType = orDefault.NodeType,
						CurrentMethodInfo = orDefault.Method
					}
				});
			return list;
		}

		public List<ExpressionWithMethodModel> VisitForConcatedBodyType(Expression exp, List<ExpressionType> concatedList)
		{
			WebRockExpressionConverterVisitor converterVisitor = new WebRockExpressionConverterVisitor();
			converterVisitor.Visit(exp);
			concatedList.AddRange(Enumerable.Select<ExpressionWithMethodModel, ExpressionType>((IEnumerable<ExpressionWithMethodModel>)converterVisitor.ConcatNodeTypes, (Func<ExpressionWithMethodModel, ExpressionType>)(c => c.ValueOfExpressionType)));
			return converterVisitor.LastLevelNodeTypes;
		}
	}
}
