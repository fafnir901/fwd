using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebRock.Utils.ExpressionTrees.Extend;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	internal class InlineApplyVisitor : ExpressionVisitor
	{
		private static readonly HashSet<MethodInfo> ApplyMethods = new HashSet<MethodInfo>(Enumerable.Select<MethodInfo, MethodInfo>(Enumerable.Where<MethodInfo>((IEnumerable<MethodInfo>)typeof(ExpressionExtensions).GetMethods(), (Func<MethodInfo, bool>)(x => x.Name == "Apply" && x.IsGenericMethod)), (Func<MethodInfo, MethodInfo>)(x => x.GetGenericMethodDefinition())));

		static InlineApplyVisitor()
		{
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.IsGenericMethod && InlineApplyVisitor.ApplyMethods.Contains(node.Method.GetGenericMethodDefinition()))
				return ExpressionExtensions.Apply(MaybeExtension.GetOrThrow<LambdaExpression, Exception>(MaybeExtension.Bind<object, LambdaExpression>(Evaluator.SimpleEval(node.Arguments[0]), (Func<object, Maybe<LambdaExpression>>)(x => MaybeExtension.MaybeAs<LambdaExpression>(x, true))), (Func<Exception>)(() => new Exception("Could not Simplify expression " + (object)node))), Enumerable.Skip<Expression>((IEnumerable<Expression>)node.Arguments, 1));
			else
				return base.VisitMethodCall(node);
		}
	}
}
