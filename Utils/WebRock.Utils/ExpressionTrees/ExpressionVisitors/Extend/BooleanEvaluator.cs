using System;
using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	internal class BooleanEvaluator : ExpressionVisitor
	{
		protected override Expression VisitConditional(ConditionalExpression node)
		{
			Expression expression = base.VisitConditional(node);
			if (expression.NodeType == ExpressionType.Conditional)
			{
				ConditionalExpression conditionalExpression = (ConditionalExpression)expression;
				if (conditionalExpression.Test.NodeType == ExpressionType.Constant)
					return (bool)((ConstantExpression)conditionalExpression.Test).Value ? conditionalExpression.IfTrue : conditionalExpression.IfFalse;
			}
			return expression;
		}

		protected override Expression VisitExtension(Expression node)
		{
			return node;
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			Expression expression = base.VisitBinary(node);
			switch (expression.NodeType)
			{
				case ExpressionType.AndAlso:
					return BooleanEvaluator.VisitBoolBinary(false, (BinaryExpression)expression);
				case ExpressionType.OrElse:
					return BooleanEvaluator.VisitBoolBinary(true, (BinaryExpression)expression);
				default:
					return expression;
			}
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			Expression expression = base.VisitUnary(node);
			if (expression.NodeType == ExpressionType.Not)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;
				if (unaryExpression.Operand.NodeType == ExpressionType.Constant)
					return (bool)((ConstantExpression)unaryExpression.Operand).Value ? (Expression)Expression.Constant((object)false) : (Expression)Expression.Constant((object)true);
			}
			return expression;
		}

		private static Expression VisitBoolBinary(bool zero, BinaryExpression binary)
		{
			ConstantExpression constantExpression = binary.Left as ConstantExpression ?? binary.Right as ConstantExpression;
			if (constantExpression != null)
				return (bool)constantExpression.Value == zero ? (Expression)Expression.Constant((object)Convert.ToBoolean(zero ? 1 : 0)) : (binary.Left.NodeType == ExpressionType.Constant ? binary.Right : binary.Left);
			else
				return (Expression)binary;
		}
	}
}
