using System;
using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	internal class Replacer : ExpressionVisitor
	{
		private readonly Func<Expression, bool> _predicate;
		private readonly Func<Expression, Expression> _with;

		internal Replacer(Func<Expression, bool> predicate, Func<Expression, Expression> with)
		{
			this._predicate = predicate;
			this._with = with;
		}

		public override Expression Visit(Expression node)
		{
			if (this._predicate(node))
				return base.Visit(this._with(node));
			else
				return base.Visit(node);
		}

		protected override Expression VisitExtension(Expression node)
		{
			return node;
		}
	}
}
