using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Extend
{
	public static class Evaluator
	{
		public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
		{
			return new Evaluator.SubtreeEvaluator(new Evaluator.Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
		}

		public static Expression PartialEval(this Expression expression)
		{
			Maybe<object> maybe = Evaluator.SimpleEval(expression);
			if (maybe.HasValue)
				return (Expression)Expression.Constant(maybe.Value);
			else
				return Evaluator.PartialEval(expression, new Func<Expression, bool>(Evaluator.CanBeEvaluatedLocally));
		}

		private static bool CanBeEvaluatedLocally(Expression expression)
		{
			return Enum.IsDefined(typeof(ExpressionType), (object)expression.NodeType) && expression.NodeType != ExpressionType.Parameter;
		}

		public static Maybe<object> SimpleEval(Expression lambdaContainer)
		{
			return MaybeExtension.OrElse<object>(
				MaybeExtension.SelectMany(
				MaybeExtension.SelectMany(
				MaybeExtension.SelectMany(
				MaybeExtension.MaybeAs<MethodCallExpression>((object)lambdaContainer, true), (Func<MethodCallExpression, Maybe<ConstantExpression>>)
				(methodCallExpression => MaybeExtension.MaybeAs<ConstantExpression>((object)methodCallExpression.Object, false)),
				(methodCallExpression, @object) =>
				{
					var local_0 = new
					{
						methodCallExpression = methodCallExpression,
						@object = @object
					};
					return local_0;
				}), param0 => Evaluator.GetParameters(param0.methodCallExpression),
				  (param0, parameters) =>
				  {
					  var local_0 = new
					  {
						  par = param0,
						  parameters = parameters
					  };
					  return local_0;
				  }), param0 => MaybeExtension.ToMaybe<object>(param0.par.methodCallExpression.Method.Invoke(param0.par.@object == null ? (object)null : param0.par.@object.Value, Enumerable.ToArray<object>(param0.parameters))), (param0, lambdaExpression) => lambdaExpression), (Func<Maybe<object>>)(() => Evaluator.Evaluate(lambdaContainer)));
		}

		public static Maybe<IEnumerable<object>> GetParameters(MethodCallExpression methodCallExpression)
		{
			var list = new List<object>();
			Maybe<IEnumerable<object>> result;
			foreach (Expression current in methodCallExpression.Arguments)
			{
				Maybe<object> maybe = Evaluator.SimpleEval(current);
				if (!maybe.HasValue)
				{
					result = Maybe.Nothing;
					return result;
				}
				list.Add(maybe.Value);
			}
			result = list;
			return result;
		}

		public static Maybe<object> Evaluate(Expression argument)
		{
			return MaybeExtension.OrElse<object>(MaybeExtension.Bind<ConstantExpression, object>(MaybeExtension.MaybeAs<ConstantExpression>((object)argument, true), (Func<ConstantExpression, object>)(x => x.Value)), (Func<Maybe<object>>)(() => MaybeExtension.SelectMany(MaybeExtension.SelectMany(MaybeExtension.MaybeAs<MemberExpression>((object)argument, true), (Func<MemberExpression, Maybe<ConstantExpression>>)(memberExpression => MaybeExtension.MaybeAs<ConstantExpression>((object)memberExpression.Expression, false)), (memberExpression, @object) =>
			{
				var local_0 = new
				{
					memberExpression = memberExpression,
					@object = @object
				};
				return local_0;
			}), param0 => MaybeExtension.OrElse<object>(MaybeExtension.Bind<PropertyInfo, object>(MaybeExtension.MaybeAs<PropertyInfo>((object)param0.memberExpression.Member, true), (Func<PropertyInfo, object>)(x => x.GetValue(param0.@object.Value, (object[])null))), (Func<Maybe<object>>)(() => MaybeExtension.Bind<FieldInfo, object>(MaybeExtension.MaybeAs<FieldInfo>((object)param0.memberExpression.Member, true), (Func<FieldInfo, object>)(x => x.GetValue(param0.@object.Value))))), (param0, computedValue) => computedValue)));
		}

		private class SubtreeEvaluator : ExpressionVisitor
		{
			private readonly HashSet<Expression> _candidates;

			internal SubtreeEvaluator(HashSet<Expression> candidates)
			{
				this._candidates = candidates;
			}

			internal Expression Eval(Expression exp)
			{
				return this.Visit(exp);
			}

			public override Expression Visit(Expression exp)
			{
				if (exp == null)
					return (Expression)null;
				else
					return this._candidates.Contains(exp) ? Evaluator.SubtreeEvaluator.Evaluate(exp) : base.Visit(exp);
			}

			protected override Expression VisitMemberInit(MemberInitExpression node)
			{
				NewExpression newExpression = this.VisitAndConvert<NewExpression>(node.NewExpression, "VisitMemberInit");
				ReadOnlyCollection<MemberBinding> readOnlyCollection = ExpressionVisitor.Visit<MemberBinding>(node.Bindings, new Func<MemberBinding, MemberBinding>(
					binding => ((SubtreeEvaluator)this).VisitMemberBinding(binding)));
				return (Expression)node.Update(newExpression, (IEnumerable<MemberBinding>)readOnlyCollection);
			}

			protected override Expression VisitExtension(Expression node)
			{
				return node;
			}

			private static Expression Evaluate(Expression e)
			{
				if (e.NodeType == ExpressionType.Constant)
					return e;
				else
					return (Expression)Expression.Constant(Expression.Lambda(e, new ParameterExpression[0]).Compile().DynamicInvoke((object[])null), e.Type);
			}
		}

		private class Nominator : ExpressionVisitor
		{
			private readonly Func<Expression, bool> _fnCanBeEvaluated;
			private HashSet<Expression> _candidates;
			private bool _cannotBeEvaluated;

			internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
			{
				this._fnCanBeEvaluated = fnCanBeEvaluated;
			}

			internal HashSet<Expression> Nominate(Expression expression)
			{
				this._candidates = new HashSet<Expression>();
				this.Visit(expression);
				return this._candidates;
			}

			protected override Expression VisitExtension(Expression node)
			{
				return node;
			}

			protected override Expression VisitMemberInit(MemberInitExpression node)
			{
				Expression expression = base.VisitMemberInit(node);
				if (this._candidates.Contains((Expression)node.NewExpression))
					this._candidates.Remove((Expression)node.NewExpression);
				return expression;
			}

			public override Expression Visit(Expression expression)
			{
				if (expression != null)
				{
					bool flag = this._cannotBeEvaluated;
					this._cannotBeEvaluated = false;
					base.Visit(expression);
					if (!this._cannotBeEvaluated)
					{
						if (this._fnCanBeEvaluated(expression))
							this._candidates.Add(expression);
						else
							this._cannotBeEvaluated = true;
					}
					Evaluator.Nominator nominator = this;
					int num = nominator._cannotBeEvaluated | flag ? 1 : 0;
					nominator._cannotBeEvaluated = num != 0;
				}
				return expression;
			}
		}
	}
}
