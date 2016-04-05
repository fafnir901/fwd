using System;
using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	internal class ProtectFromNullReferenceVisitor : ExpressionVisitor
	{
		protected override Expression VisitMember(MemberExpression memberExpression)
		{
			Expression left = this.Visit(memberExpression.Expression);
			Type type = TypeExtensions.ResultType(memberExpression.Member);
			if (left.Type.IsValueType)
				return (Expression)memberExpression;
			else
				return (Expression)Expression.Condition((Expression)Expression.ReferenceEqual(left, (Expression)Expression.Constant(TypeExtensions.DefaultValue(left.Type))), (Expression)Expression.Constant(TypeExtensions.DefaultValue(type), type), (Expression)memberExpression);
		}
	}
}
