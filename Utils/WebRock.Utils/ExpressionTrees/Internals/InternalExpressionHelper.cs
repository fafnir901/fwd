using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using WebRock.Utils.ExpressionTrees.Models;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Internals
{
	internal class InternalExpressionHelper
	{
		private readonly IMapper _mapper;

		public InternalExpressionHelper(IMapper mapper = null)
		{
			_mapper = mapper;
		}

		public List<LastLevelNodeExpressionTreeModel> PutSimpleExpressionInList(BinaryExpression binBody, List<LastLevelNodeExpressionTreeModel> leftList, List<LastLevelNodeExpressionTreeModel> rightList)
		{
			var condition = leftList.Count == 1
							&& rightList.Count == 1
							&& (leftList[0].RightExpression == null
							&& ValidateForNotUnaryType(leftList[0].ExpressionWithMethodModel.ValueOfExpressionType))
							&& leftList[0].ExpressionWithMethodModel.ValueOfExpressionType != ExpressionType.Call;
			if (condition)
			{
				leftList[0].RightExpression = rightList[0].RightExpression;
				leftList[0].ExpressionWithMethodModel.ValueOfExpressionType = binBody.NodeType;
				leftList[0].ExpressionWithMethodModel.CurrentMethodInfo = binBody.Method;
				rightList = new List<LastLevelNodeExpressionTreeModel>();
			}
			return rightList;
		}

		private bool ValidateForNotUnaryType(ExpressionType type)
		{
			return type != ExpressionType.Not
				&& type != ExpressionType.NotEqual
				&& type != ExpressionType.Equal;
		}

		public MemberExpression DefineMemberExpression(ParameterExpression parameter, PropertyInfo property, string propertyName)
		{
			MemberExpression seed;
			if (property == null)
			{
				var lst = _mapper.GetPropertyFromMapping(propertyName).ToList();
				property = lst.First();
				lst.Remove(lst.First());
				lst = lst.Distinct()
					.Except(lst.Where(c => c == null))
					.ToList();

				seed = Expression.MakeMemberAccess(parameter, property);

				var condition = property.PropertyType
					.GetProperties()
					.Any(c => lst.Contains(c));

				if (condition)
				{
					seed = lst.Aggregate(seed, Expression.MakeMemberAccess);
				}
			}
			else
			{
				seed = Expression.MakeMemberAccess(parameter, property);
			}
			return seed;
		}

		public Expression DefineTypeOfOperator(ExpressionType type, Expression parameter, IEnumerable<Expression> constants, MethodInfo method)
		{
			switch (type)
			{
				case ExpressionType.And:
					return Expression.And(parameter, constants.First());
				case ExpressionType.AndAlso:
					return Expression.AndAlso(parameter, constants.First());
				case ExpressionType.Call:
					return CreateCallExpression(parameter, constants, method);
				case ExpressionType.Equal:
					return Expression.Equal(parameter, constants.First());
				case ExpressionType.GreaterThan:
					return Expression.GreaterThan(parameter, constants.First());
				case ExpressionType.GreaterThanOrEqual:
					return Expression.GreaterThanOrEqual(parameter, constants.First());
				case ExpressionType.LessThan:
					return Expression.LessThan(parameter, constants.First());
				case ExpressionType.LessThanOrEqual:
					return Expression.LessThanOrEqual(parameter, constants.First());
				case ExpressionType.Not:
					return Expression.Not(parameter);
				case ExpressionType.NotEqual:
					var orDefault = constants
						.First()
						.MaybeAs<ConstantExpression>()
						.GetOrDefault(Expression.Constant(null, parameter.Type));
					return Expression.NotEqual(parameter, orDefault);
				case ExpressionType.Or:
					return Expression.Or(parameter, constants.First());
				case ExpressionType.OrElse:
					return Expression.OrElse(parameter, constants.First());
				default:
					return null;
			}
		}

		private static Expression CreateCallExpression(Expression parameter, IEnumerable<Expression> constants, MethodInfo method)
		{
			var list = constants.ToList();
			var removalList = list.Where(c => c == null).ToList();
			foreach (Expression expression in removalList)
			{
				list.Remove(expression);
			}
			if (!list.Any())
			{
				return Expression.Call(method, parameter);
			}
			if (method.IsStatic)
			{
				parameter = null;
			}

			//TODO Узнать как сделать expression tree для LINQ методов с предикатом

			//var remov = new List<Expression>();
			//foreach (var expression in list)
			//{
			//	var current = expression as ConstantExpression;
			//	if (current.Value == null)
			//	{
			//		remov.Add(expression);
			//	}
			//}

			//foreach (var expression in remov)
			//{
			//	list.Remove(expression);

			//	var newConst = Expression.Constant(null, method.ReturnType);
			//	list.Add(newConst);
			//}

			//list.Reverse();
			return Expression.Call(parameter, method, list);
		}

		public IEnumerable<Expression> CollectListOfExpressions<TTo>(IEnumerable<LastLevelNodeExpressionTreeModel> concated, ParameterExpression parameter)
		{
			var asm = new ExpressionAssembler(_mapper);
			return concated
				.Select(expression =>
					expression.ExpressionWithMethodModel.CurrentMethodInfo != null && expression.ExpressionWithMethodModel.CurrentMethodInfo.GetParameters().Any(c => c.Name == "predicate")
					? asm.AssemblyMultiplyBinaryExpression<TTo>(concated, parameter)
					: asm.AssemblySimpleBinaryExpression<TTo>(expression, parameter))
				.Where(binaryExpression => binaryExpression != null);
		}

		internal static Type GetElementType(Type seqType)
		{

			Type ienum = FindIEnumerable(seqType);

			if (ienum == null) return seqType;

			return ienum.GetGenericArguments()[0];

		}

		private static Type FindIEnumerable(Type seqType)
		{

			if (seqType == null || seqType == typeof(string))

				return null;

			if (seqType.IsArray)

				return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

			if (seqType.IsGenericType)
			{

				foreach (Type arg in seqType.GetGenericArguments())
				{

					Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);

					if (ienum.IsAssignableFrom(seqType))
					{

						return ienum;

					}

				}

			}

			Type[] ifaces = seqType.GetInterfaces();

			if (ifaces != null && ifaces.Length > 0)
			{

				foreach (Type iface in ifaces)
				{

					Type ienum = FindIEnumerable(iface);

					if (ienum != null) return ienum;

				}

			}

			if (seqType.BaseType != null && seqType.BaseType != typeof(object))
			{

				return FindIEnumerable(seqType.BaseType);

			}

			return null;

		}
	}

}
