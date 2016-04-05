using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.Models
{
	public class ExpressionAdderModel<TFrom>
	{
		public Expression<Func<TFrom, bool>> Expression { get; set; }

		public ExpressionType CurrentTypeOfExpression { get; set; }

		public override bool Equals(object obj)
		{
			ExpressionAdderModel<TFrom> eq = obj as ExpressionAdderModel<TFrom>;
			if (eq == null)
				return false;
			else
				return this.Equals(eq);
		}

		public bool Equals(ExpressionAdderModel<TFrom> eq)
		{
			return object.ReferenceEquals((object) this, (object) eq) ||
			       Enumerable.Count<ParameterExpression>((IEnumerable<ParameterExpression>) this.Expression.Parameters) ==
			       Enumerable.Count<ParameterExpression>((IEnumerable<ParameterExpression>) eq.Expression.Parameters) &&
			       this.Expression.Name == eq.Expression.Name &&
			       (this.Expression.ReturnType.FullName == eq.Expression.ReturnType.FullName &&
			        this.Expression.Body.NodeType == eq.Expression.Body.NodeType) &&
			       this.Expression.Body.Type.FullName == eq.Expression.Body.Type.FullName &&
			       this.CurrentTypeOfExpression == eq.CurrentTypeOfExpression;
		}

		public override int GetHashCode()
		{
			return Enumerable.Count<ParameterExpression>((IEnumerable<ParameterExpression>) this.Expression.Parameters)*
			       this.Expression.Name.GetHashCode()*this.Expression.Body.Type.FullName.GetHashCode()*
			       this.Expression.Body.NodeType.GetHashCode()*this.CurrentTypeOfExpression.GetHashCode()*7;
		}
	}
}
