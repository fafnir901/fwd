using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	internal class CtorTypeChanger<TBase, TDerived> : ExpressionVisitor where TDerived : TBase
	{
		protected override Expression VisitNew(NewExpression node)
		{
			if (!(node.Type == typeof(TBase)))
				return base.VisitNew(node);
			ConstructorInfo apropriateCtor = this.FindApropriateCtor(node.Constructor);
			if (node.Members != null)
				return (Expression)Expression.New(apropriateCtor, (IEnumerable<Expression>)node.Arguments, (IEnumerable<MemberInfo>)node.Members);
			else
				return (Expression)Expression.New(apropriateCtor, (IEnumerable<Expression>)node.Arguments);
		}

		private ConstructorInfo FindApropriateCtor(ConstructorInfo constructor)
		{
			return typeof(TDerived).GetConstructor(Enumerable.ToArray<Type>(Enumerable.Select<ParameterInfo, Type>((IEnumerable<ParameterInfo>)constructor.GetParameters(), (Func<ParameterInfo, Type>)(x => x.ParameterType))));
		}
	}
}
