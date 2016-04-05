using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Extend
{
	public static class ExpressionExtensions
	{
		public static Maybe<T> TryEval<T>(this Expression expression)
		{
			expression = Evaluator.PartialEval(expression);
			ConstantExpression constantExpression = expression as ConstantExpression;
			return constantExpression == null || !(constantExpression.Value is T) ? (Maybe<T>)Maybe.Nothing : Maybe.Return<T>((T)constantExpression.Value);
		}

		public static Expression<Func<TArg0, TResult>> Expr<TArg0, TResult>(this Expression<Func<TArg0, TResult>> expr)
		{
			return expr;
		}

		public static Expression<Func<TArg0, TArg1, TResult>> Expr<TArg0, TArg1, TResult>(this Expression<Func<TArg0, TArg1, TResult>> expr)
		{
			return expr;
		}

		public static bool IsConstantNull(this Expression e)
		{
			return ExpressionExtensions.IsConstant(e, (object)null);
		}

		public static bool IsConstant(this Expression e, object constant)
		{
			return e.NodeType == ExpressionType.Constant && object.Equals(((ConstantExpression)e).Value, constant);
		}

		public static Expression<Func<T, TResult>> Convert<T, TResult>(this LambdaExpression e)
		{
			bool flag1 = e.Body.Type != typeof(TResult);
			ParameterExpression oldParameter = e.Parameters[0];
			bool flag2 = oldParameter.Type != typeof(T);
			LambdaExpression lambdaExpression = flag1 ? Expression.Lambda((Expression)Expression.Convert(e.Body, typeof(TResult)), (IEnumerable<ParameterExpression>)e.Parameters) : e;
			if (flag2)
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(T), oldParameter.Name);
				lambdaExpression = Expression.Lambda(ExpressionExtensions.Replace<Expression>(lambdaExpression.Body, (Func<Expression, bool>)(x => x == oldParameter), (Expression)Expression.Convert((Expression)parameterExpression, oldParameter.Type)), new ParameterExpression[1]
        {
          parameterExpression
        });
			}
			return (Expression<Func<T, TResult>>)lambdaExpression;
		}

		public static Expression<Func<TNewParam, TResult>> ReplaceLambdaParameter<TNewParam, TResult>(this LambdaExpression e, LambdaExpression replacement)
		{
			ParameterExpression oldParameter = e.Parameters[0];
			ParameterExpression parameterExpression = replacement.Parameters[0];
			return (Expression<Func<TNewParam, TResult>>)Expression.Lambda(ExpressionExtensions.Replace<Expression>(e.Body, (Func<Expression, bool>)(x => x == oldParameter), replacement.Body), new ParameterExpression[1]
      {
        parameterExpression
      });
		}

		public static Expression<Func<T, TResult>> MakeSingleParameter<T, TResult>(this Expression<Func<T, T, TResult>> selector)
		{
			ParameterExpression what = selector.Parameters[1];
			ParameterExpression parameterExpression = selector.Parameters[0];
			return Expression.Lambda<Func<T, TResult>>(ExpressionExtensions.Replace<Expression>(selector.Body, (Func<Expression, bool>)(x => x == what), (Expression)parameterExpression), new ParameterExpression[1]
			{
				parameterExpression
			});
		}

		public static T Replace<T>(this T @in, Expression what, Expression with) where T : Expression
		{
			return (T)new Replacer((Func<Expression, bool>)(e => e == what), (Func<Expression, Expression>)(e => with)).Visit((Expression)@in);
		}

		public static T Replace<T>(this T @in, Func<Expression, bool> predicate, Expression with) where T : Expression
		{
			return (T)new Replacer(predicate, (Func<Expression, Expression>)(e => with)).Visit((Expression)@in);
		}

		public static T Replace<T>(this T @in, Func<Expression, bool> predicate, Func<Expression, Expression> with) where T : Expression
		{
			return (T)new Replacer(predicate, with).Visit((Expression)@in);
		}

		public static Expression ApplyExpression(this LambdaExpression lambda, params Expression[] arguments)
		{
			return ExpressionExtensions.Apply(lambda, Enumerable.AsEnumerable<Expression>((IEnumerable<Expression>)arguments));
		}

		internal static Expression Apply(this LambdaExpression lambda, IEnumerable<Expression> arguments)
		{
			arguments = (IEnumerable<Expression>)Enumerable.ToList<Expression>(arguments);
			if (Enumerable.Count<ParameterExpression>((IEnumerable<ParameterExpression>)lambda.Parameters) == Enumerable.Count<Expression>(arguments))
				return Enumerable.Aggregate(Enumerable.Zip((IEnumerable<ParameterExpression>)lambda.Parameters, arguments, (parameter, argument) =>
				{
					var local_0 = new
					{
						parameter = parameter,
						argument = argument
					};
					return local_0;
				}), lambda.Body, (body, pair) => ExpressionExtensions.Replace<Expression>(body, (Func<Expression, bool>)(x => x == pair.parameter), pair.argument));
			throw new ArgumentException(StringExtensions.Fmt("Expected {0} parameters, given {1} arguments", (object)Enumerable.Count<ParameterExpression>((IEnumerable<ParameterExpression>)lambda.Parameters), (object)Enumerable.Count<Expression>(arguments)));
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6);
		}

		public static TResult Apply<T1, T2, T3, T4, T5, TResult>(this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			return expression.Compile()(t1, t2, t3, t4, t5);
		}

		public static TResult Apply<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4)
		{
			return expression.Compile()(t1, t2, t3, t4);
		}

		public static TResult Apply<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T1 t1, T2 t2, T3 t3)
		{
			return expression.Compile()(t1, t2, t3);
		}

		public static TResult Apply<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T1 t1, T2 t2)
		{
			return expression.Compile()(t1, t2);
		}

		public static TResult Apply<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 t1)
		{
			return expression.Compile()(t1);
		}

		public static TResult Apply<TResult>(this Expression<Func<TResult>> expression)
		{
			return expression.Compile()();
		}

		public static Expression<Func<TResult>> PartialApply<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 paramValue)
		{
			return (Expression<Func<TResult>>)ExpressionExtensions.PartialApplyLambda<T1>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, TResult>> PartialApply<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T2 paramValue)
		{
			return (Expression<Func<T1, TResult>>)ExpressionExtensions.PartialApplyLambda<T2>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, TResult>> PartialApply<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T3 paramValue)
		{
			return (Expression<Func<T1, T2, TResult>>)ExpressionExtensions.PartialApplyLambda<T3>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, TResult>> PartialApply<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression, T4 paramValue)
		{
			return (Expression<Func<T1, T2, T3, TResult>>)ExpressionExtensions.PartialApplyLambda<T4>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, TResult>> PartialApply<T1, T2, T3, T4, T5, TResult>(this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T5 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, TResult>>)ExpressionExtensions.PartialApplyLambda<T5>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T6 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, TResult>>)ExpressionExtensions.PartialApplyLambda<T6>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T7 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, TResult>>)ExpressionExtensions.PartialApplyLambda<T7>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T8 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>)ExpressionExtensions.PartialApplyLambda<T8>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T9 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>)ExpressionExtensions.PartialApplyLambda<T9>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T10 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>)ExpressionExtensions.PartialApplyLambda<T10>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T11 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>)ExpressionExtensions.PartialApplyLambda<T11>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T12 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>)ExpressionExtensions.PartialApplyLambda<T12>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T13 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>)ExpressionExtensions.PartialApplyLambda<T13>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T14 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>)ExpressionExtensions.PartialApplyLambda<T14>((LambdaExpression)expression, paramValue);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression, T15 paramValue)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>)ExpressionExtensions.PartialApplyLambda<T15>((LambdaExpression)expression, paramValue);
		}

		private static LambdaExpression PartialApplyLambda<TParam2>(LambdaExpression expression, TParam2 paramValue)
		{
			ParameterExpression parameterExpression = Enumerable.Last<ParameterExpression>((IEnumerable<ParameterExpression>)expression.Parameters);
			ConstantExpression constantExpression = Expression.Constant((object)paramValue, parameterExpression.Type);
			return Expression.Lambda(ExpressionExtensions.Replace<Expression>(expression.Body, (Expression)parameterExpression, (Expression)constantExpression), Enumerable.Take<ParameterExpression>((IEnumerable<ParameterExpression>)expression.Parameters, expression.Parameters.Count - 1));
		}

		public static T InlineApply<T>(this T expression) where T : Expression
		{
			return (T)new InlineApplyVisitor().Visit((Expression)expression);
		}

		public static Expression EvalBooleanConstants(this Expression expression)
		{
			return new BooleanEvaluator().Visit(expression);
		}

		public static T ProtectFromNullReference<T>(this T expression) where T : Expression
		{
			return (T)new ProtectFromNullReferenceVisitor().Visit((Expression)expression);
		}

		public static IEnumerable<Expression> ToEnumerable(this Expression expression)
		{
			return (IEnumerable<Expression>)new ExpressionCollection(expression);
		}

		public static Expression<Func<T, bool>> CombineAnd<T>(this Expression<Func<T, bool>> head, params Expression<Func<T, bool>>[] tail)
		{
			return ExpressionExtensions.Combine<T>(head, (IEnumerable<Expression<Func<T, bool>>>)tail, new Func<Expression, Expression, BinaryExpression>(Expression.AndAlso));
		}

		public static Expression<Func<T, bool>> CombineOr<T>(this Expression<Func<T, bool>> head, params Expression<Func<T, bool>>[] tail)
		{
			return ExpressionExtensions.Combine<T>(head, (IEnumerable<Expression<Func<T, bool>>>)tail, new Func<Expression, Expression, BinaryExpression>(Expression.OrElse));
		}

		public static Expression CombineOr(this IEnumerable<Expression> expressions)
		{
			return Enumerable.Aggregate<Expression>(expressions, new Func<Expression, Expression, Expression>(Expression.OrElse));
		}

		public static Expression CombineAnd(this IEnumerable<Expression> expressions)
		{
			return Enumerable.Aggregate<Expression>(expressions, new Func<Expression, Expression, Expression>(Expression.AndAlso));
		}

		private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> head, IEnumerable<Expression<Func<T, bool>>> tail, Func<Expression, Expression, BinaryExpression> combiner)
		{
			return Enumerable.Aggregate<Expression<Func<T, bool>>, Expression<Func<T, bool>>>(tail, head, (Func<Expression<Func<T, bool>>, Expression<Func<T, bool>>, Expression<Func<T, bool>>>)((soFar, element) =>
			{
				Expression local_0 = LambdaSubstituter.ReplaceParameters((LambdaExpression)element, (IEnumerable<Expression>)soFar.Parameters);
				return (Expression<Func<T, bool>>)Expression.Lambda((Expression)combiner(soFar.Body, local_0), (IEnumerable<ParameterExpression>)soFar.Parameters);
			}));
		}

		public static Expression ChangeCtorType<TBase, TDerived>(this Expression lambda) where TDerived : TBase
		{
			return new CtorTypeChanger<TBase, TDerived>().Visit(lambda);
		}
	}
}
