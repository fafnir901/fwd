using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors
{
	public class WebRockExpressionCollectorVisitor : ExpressionVisitor
	{
		private ParameterExpression _expression;

		public WebRockExpressionCollectorVisitor(ParameterExpression expression)
		{
			this._expression = expression;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			return _expression;
		}
	}
}
