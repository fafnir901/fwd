using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebRock.Utils.ExpressionTrees.ExpressionVisitors;
using WebRock.Utils.ExpressionTrees.Models;

namespace WebRock.Utils.ExpressionTrees
{
	public class ExpressionCollector<TFrom>
	{
		private readonly List<ExpressionAdderModel<TFrom>> _listOfExpression = new List<ExpressionAdderModel<TFrom>>();
		private readonly WebRockExpressionCollectorVisitor _visitor;

		public List<ExpressionAdderModel<TFrom>> CurrentListOfExpression
		{
			get
			{
				return this._listOfExpression;
			}
		}

		public ExpressionCollector()
		{
			this._visitor = new WebRockExpressionCollectorVisitor(this.GetParameterExpression());
		}

		public void AddExpression(ExpressionAdderModel<TFrom> adder)
		{
			if (Enumerable.FirstOrDefault<ExpressionAdderModel<TFrom>>((IEnumerable<ExpressionAdderModel<TFrom>>)this._listOfExpression, (Func<ExpressionAdderModel<TFrom>, bool>)(c => c.Equals(adder))) != null)
				return;
			this._listOfExpression.Add(adder);
		}

		public void AddExpression(Expression<Func<TFrom, bool>> expression, ExpressionType type)
		{
			this.AddExpression(new ExpressionAdderModel<TFrom>()
			{
				Expression = expression,
				CurrentTypeOfExpression = type
			});
		}

		public void RemoveExpression(ExpressionAdderModel<TFrom> adder)
		{
			if (Enumerable.FirstOrDefault<ExpressionAdderModel<TFrom>>((IEnumerable<ExpressionAdderModel<TFrom>>)this._listOfExpression, (Func<ExpressionAdderModel<TFrom>, bool>)(c => c.Equals(adder))) == null)
				return;
			this._listOfExpression.Remove(adder);
		}

		public void RemoveExpression(Expression<Func<TFrom, bool>> expression, ExpressionType type)
		{
			this.RemoveExpression(new ExpressionAdderModel<TFrom>()
			{
				Expression = expression,
				CurrentTypeOfExpression = type
			});
		}

		private ParameterExpression GetParameterExpression()
		{
			return Expression.Parameter(typeof(TFrom), "c");
		}

		private Expression TryCollectExpression()
		{
			try
			{
				Expression expression = this._listOfExpression[0].Expression.Body;
				this._visitor.Visit(expression);
				if (this._listOfExpression.Count > 1)
				{
					for (int index = 1; index < this._listOfExpression.Count; ++index)
					{
						this._visitor.Visit((Expression)this._listOfExpression[index].Expression);
						expression = (Expression)Expression.MakeBinary(this._listOfExpression[index].CurrentTypeOfExpression, expression, this._listOfExpression[index].Expression.Body);
					}
				}
				return expression;
			}
			catch (Exception ex)
			{
				return (Expression)null;
			}
		}

		public Expression<Func<TFrom, bool>> Collect()
		{
			Expression<Func<TFrom, bool>> expression;
			if (this.TryCollectExpression() == null)
				expression = (Expression<Func<TFrom, bool>>)null;
			else
				expression = Expression.Lambda<Func<TFrom, bool>>(this.TryCollectExpression(), new ParameterExpression[1]
        {
          this.GetParameterExpression()
        });
			return expression;
		}
	}

}
