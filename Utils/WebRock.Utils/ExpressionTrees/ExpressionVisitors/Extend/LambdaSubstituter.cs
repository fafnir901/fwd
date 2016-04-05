using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	public class LambdaSubstituter : ExpressionVisitor
	{
		private readonly ParameterExpression _what;
		private readonly Expression _with;

		private LambdaSubstituter(ParameterExpression what, Expression with)
		{
			this._what = what;
			this._with = with;
		}

		public static Expression ReplaceParameters(LambdaExpression @in, params Expression[] with)
		{
			return LambdaSubstituter.ReplaceParameters(@in, (IEnumerable<Expression>)with);
		}

		public static Expression ReplaceParameters(LambdaExpression @in, IEnumerable<Expression> with)
		{
			Expression expression = @in.Body;
			List<Expression> list = Enumerable.ToList<Expression>(with);
			if (Enumerable.Count<Expression>((IEnumerable<Expression>)list) == @in.Parameters.Count)
			{
				expression = Enumerable.Aggregate(Enumerable.Zip((IEnumerable<ParameterExpression>)@in.Parameters, (IEnumerable<Expression>)list, (parameter, replace) =>
				{
					var local_0 = new
					{
						parameter = parameter,
						replace = replace
					};
					return local_0;
				}), expression, (current, exp) => LambdaSubstituter.Rewrite(current, exp.parameter, exp.replace));
			}
			else
			{
				foreach (ParameterExpression what in @in.Parameters)
				{
					ParameterExpression parameter1 = what;
					using (IEnumerator<Expression> enumerator = Enumerable.Where<Expression>((IEnumerable<Expression>)list, (Func<Expression, bool>)(withParameter => parameter1.Type == withParameter.Type)).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							Expression current = enumerator.Current;
							expression = LambdaSubstituter.Rewrite(expression, what, current);
						}
					}
				}
			}
			return expression;
		}

		internal static Expression Rewrite(Expression @in, ParameterExpression what, Expression with)
		{
			if (what.Type != with.Type && !what.Type.IsAssignableFrom(with.Type))
				with = (Expression)Expression.Convert(with, what.Type);
			return new LambdaSubstituter(what, with).Visit(@in);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == this._what)
				return this._with;
			else
				return base.VisitParameter(node);
		}
	}
}
