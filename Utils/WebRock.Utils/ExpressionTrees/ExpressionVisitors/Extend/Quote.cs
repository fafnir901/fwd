using System.Collections.Generic;
using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	public static class Quote
	{
		public static Expression Splice(this LambdaExpression e, params Expression[] newExpr)
		{
			return LambdaSubstituter.ReplaceParameters(e, newExpr);
		}

		public static Expression Splice(this LambdaExpression e, IEnumerable<Expression> newExpr)
		{
			return LambdaSubstituter.ReplaceParameters(e, newExpr);
		}
	}
}
