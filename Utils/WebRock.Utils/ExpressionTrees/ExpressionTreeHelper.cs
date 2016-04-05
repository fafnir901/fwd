using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using WebRock.Utils.ExpressionTrees.Internals;
using WebRock.Utils.ExpressionTrees.Models;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees
{
	public static class ExpressionTreeHelper
	{
		private static IMapper _mapper;

		public static Expression<Func<TTo, bool>> Convert<TTo, TFrom>(this Expression<Func<TFrom, bool>> predicate, IMapper mapper = null)
		{
			_mapper = mapper ?? new SimpleMapper<TTo, TFrom>(null);
			Expression body = predicate.Body;
			return 
				(
					new List<Expression>()
					{
						ConvertForBinaryExpression<TTo>(body.MaybeAs<BinaryExpression>().GetOrDefault(null)),
						ConvertForUnaryExpression<TTo>(body.MaybeAs<UnaryExpression>().GetOrDefault(null)),
						ConvertForCallExpression<TTo>(body.MaybeAs<MethodCallExpression>().GetOrDefault(null))
					}
				).FirstOrDefault(c => c != null)
				.MaybeAs<Expression<Func<TTo, bool>>>()
				.GetOrDefault(null);
		}

		private static Expression ConvertForBinaryExpression<TTo>(BinaryExpression expression)
		{
			if (expression == null)
				return null;
			var expressionDisassembler = new ExpressionDisassembler(_mapper);
			var expressionAssembler = new ExpressionAssembler(_mapper);
			var expressionHelper = new InternalExpressionHelper(_mapper);
			var list1 = new List<ExpressionType>();
			var parameter = Expression.Parameter(typeof(TTo), "x");
			var list2 = expressionDisassembler.DisassembleBinaryExpression(expression, list1).ToList();
			if (list2.Count() == 1)
			{
				return Expression.Lambda<Func<TTo, bool>>(expressionAssembler.AssemblySimpleBinaryExpression<TTo>(list2[0], parameter)
					.MaybeAs<BinaryExpression>()
					.GetOrDefault(null), parameter);
			}
			else
			{
				List<Expression> listConvertedExpressions = expressionHelper.CollectListOfExpressions<TTo>(list2, parameter).ToList();

				if (list1.Count == 0)
					list1.Add(list2.Last().ExpressionWithMethodModel.ValueOfExpressionType);

				return Expression.Lambda<Func<TTo, bool>>(expressionAssembler.AssemblyExpression(listConvertedExpressions, list1), parameter);
			}
		}

		private static Expression ConvertForUnaryExpression<TTo>(UnaryExpression expression)
		{
			if (expression != null)
			{
				var expressionDisassembler = new ExpressionDisassembler(_mapper);
				var expressionAssembler = new ExpressionAssembler(_mapper);
				var concatedList = new List<ExpressionType>();
				var parameter = Expression.Parameter(typeof(TTo), "x");
				var list = expressionDisassembler.DisassembleUnaryExpression(expression, concatedList).ToList();
				if (list.Count() == 1)
					return Expression.Lambda<Func<TTo, bool>>((expressionAssembler.AssemblySimpleUnaryExpression<TTo>(list[0], parameter,null))
						.MaybeAs<UnaryExpression>()
						.GetOrDefault(null), parameter);
			}
			return null;
		}

		private static Expression ConvertForCallExpression<TTo>(MethodCallExpression expression)
		{
			if (expression != null)
			{
				var expressionDisassembler = new ExpressionDisassembler(_mapper);
				var expressionAssembler = new ExpressionAssembler(_mapper);
				var concatedList = new List<ExpressionType>();
				var parameter = Expression.Parameter(typeof(TTo), "x");
				var list = expressionDisassembler.DisassembleCallExpression(expression, concatedList).ToList();
				if (list.Count() == 1)
				{
					ReadOnlyCollection<Expression> arguments = expression.Arguments;
					return Expression.Lambda<Func<TTo, bool>>(expressionAssembler.AssemblySimpleCallExpression<TTo>(list[0], parameter, arguments)
						.MaybeAs<MethodCallExpression>()
						.GetOrDefault(null), parameter);
				}
			}
			return null;
		}
	}
}
