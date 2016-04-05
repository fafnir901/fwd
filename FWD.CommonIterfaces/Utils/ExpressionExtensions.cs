using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FWD.CommonIterfaces.Utils
{
	public static class ExpressionExtensions
	{
		public static PropertyInfo GetProperty<T>(this Expression<Func<T, object>> expression)
		{
			MemberExpression memberExpression = null;

			if (expression.Body.NodeType == ExpressionType.Convert)
			{
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			}
			else if (expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new ArgumentException("Not a member access", "expression");
			}

			return memberExpression.Member as PropertyInfo;
		}

		public static Expression<Func<T, object>> FuncToExpression<T>(this Func<T, object> f)
		{
			return x => f(x);
		} 
	}
}
