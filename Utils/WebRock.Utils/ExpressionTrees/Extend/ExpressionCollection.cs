using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.Extend
{
	internal sealed class ExpressionCollection : ExpressionVisitor, IEnumerable<Expression>, IEnumerable
	{
		private readonly List<Expression> _expressions = new List<Expression>();

		public ExpressionCollection(Expression expression)
		{
			this.Visit(expression);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)this.GetEnumerator();
		}

		public IEnumerator<Expression> GetEnumerator()
		{
			return (IEnumerator<Expression>)this._expressions.GetEnumerator();
		}

		public override Expression Visit(Expression node)
		{
			if (null != node)
				this._expressions.Add(node);
			return base.Visit(node);
		}

		protected override Expression VisitExtension(Expression node)
		{
			return node;
		}
	}

}
