using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FWD.BusinessObjects.Domain;
using WebRock.Utils.ExpressionTrees.ExpressionVisitors;
using WebRock.Utils.Monad;

namespace FWD.DAL.Helpers
{
	internal static class PredicateHelper
	{
		public static PredicateSet GetSinglePredicateSet<TFrom>(this Expression<Func<TFrom, bool>> predicate)
		{
			var body = predicate.Body;
			var binBody = body.MaybeAs<BinaryExpression>();
			var unBody = body.MaybeAs<UnaryExpression>();
			var callBody = body.MaybeAs<MethodCallExpression>();
			var memBody = body.MaybeAs<MemberExpression>();

			var listOfExpression = new List<PredicateSet>
			{
				GetSinglePredicateSetFromBinaryExpression(binBody),
				GetSinglePredicateSetFromUnaryExpressionExpression(unBody),
				GetSinglePredicateSetFromMethodCallExpression(callBody),
				GetSinglePredicateSetFromMemberExpression(memBody)
			};
			return listOfExpression.FirstOrDefault(c => c != null);
		}

		private static PredicateSet GetSinglePredicateSetFromMethodCallExpression(Maybe<MethodCallExpression> callBody)
		{
			if (callBody == Maybe.Nothing)
			{
				return null;
			}
			var body = callBody.Value;
			string val;
			if (body.Arguments[0] is MemberExpression)
			{
				var res =
					body.Arguments[0].MaybeAs<MemberExpression>()
						.Bind(c => c.Expression)
						.GetOrDefault(null)
						.MaybeAs<MemberExpression>()
						.Bind(c => c.Expression)
						.GetOrDefault(null)
						.MaybeAs<ConstantExpression>()
						.Bind(c => c.Value)
						.GetOrDefault(null);
				if (res == null)
				{
					res =
					body.Arguments[0].MaybeAs<MemberExpression>()
						.Bind(c => c.Expression)
						.GetOrDefault(null).MaybeAs<ConstantExpression>()
						.Bind(c => c.Value)
						.GetOrDefault(null);
				}

				val = res
				.NothingIfNull()
				.Bind(c => c.GetType().GetField(res.GetType().GetFields()[0].Name))
				.Bind(c => c.GetValue(res))
				.GetOrDefault(null)
				.MaybeAs<Article>()
				.Bind(c => c.ArticleName)
				.GetOrDefault(null);

				if (val == null)
				{
					val = res
						.NothingIfNull()
						.Bind(c => c.GetType().GetField(res.GetType().GetFields()[0].Name))
						.Bind(c => c.GetValue(res).ToString())
						.GetOrDefault(null);
				}

			}
			else
			{
				val = body.Arguments[0].ToString();
			}
			return new PredicateSet
			{
				Method = body.Method.Name,
				Property = body.Object.MaybeAs<MemberExpression>().Bind(c => c.Member.Name).GetOrDefault(null),
				Value = val
			};
		}

		private static PredicateSet GetSinglePredicateSetFromUnaryExpressionExpression(Maybe<UnaryExpression> unBody)
		{
			if (unBody == Maybe.Nothing)
			{
				return null;
			}
			var body = unBody.Value;
			return new PredicateSet
			{
				Method = body.NodeType.ToString(),
				Property = body.Operand.MaybeAs<MemberExpression>().Bind(c => c.Member.Name).GetOrDefault(null),
			};
		}

		private static PredicateSet GetSinglePredicateSetFromBinaryExpression(Maybe<BinaryExpression> binBody)
		{
			if (binBody == Maybe.Nothing)
			{
				return null;
			}
			var body = binBody.Value;

			var obj =
				body.Right.MaybeAs<MethodCallExpression>()
					.Bind(c => c.Arguments[0])
					.GetOrDefault(null)
					.MaybeAs<MemberExpression>()
					.Bind(c => c.Expression)
					.GetOrDefault(null)
					.MaybeAs<ConstantExpression>()
					.Bind(c => c.Value)
					.GetOrDefault(null);
			string fieldName = "value";
			if (obj != null)
			{
				fieldName = obj.GetType().GetFields()[0].Name;
			}
			var value = obj.NothingIfNull().Bind(c => c.GetType().GetField(fieldName)).Bind(c => c.GetValue(obj).ToString()).GetOrDefault(null);
			if (string.IsNullOrEmpty(value))
			{
				obj =
					body.Right.MaybeAs<MemberExpression>()
						.Bind(c => c.Expression)
						.GetOrDefault(null)
						.MaybeAs<ConstantExpression>()
						.Bind(c => c.Value)
						.GetOrDefault(null) 
						??
						body.Right.MaybeAs<MemberExpression>()
						.Bind(c => c.Expression)
						.GetOrDefault(null)
						.MaybeAs<MemberExpression>()
						.Bind(c=>c.Expression)
						.GetOrDefault(null)
						.MaybeAs<ConstantExpression>()
						.Bind(c=>c.Value)
						.GetOrDefault(null);
				if (obj != null)
				{
					fieldName = obj.GetType().GetFields()[0].Name;
				}
				if (obj != null && obj.GetType() != typeof(string) && !obj.GetType().IsPrimitive && obj.GetType().IsClass)
				{
					//TODO достать значение свойства из object
				}
				value = obj.NothingIfNull().Bind(c => c.GetType().GetField(fieldName)).Bind(c => c.GetValue(obj).ToString()).GetOrDefault(null);
			}
			var listOfValues = new List<string>
			{
				body.Right.MaybeAs<ConstantExpression>().Bind(c => c.Value.ToString()).GetOrDefault(null),
				value
			};

			var property = body.Left.MaybeAs<MethodCallExpression>().Bind(c => c.Object).GetOrDefault(null).MaybeAs<MemberExpression>().Bind(c => c.Member.Name).GetOrDefault(null);
			var listOfProperty = new List<string>()
			{
				body.Left.MaybeAs<MemberExpression>().Bind(c=>c.Member.Name).GetOrDefault(null),
				property
			};
			if (property == null)
			{
				var vt = new WebRockExpressionConverterVisitor();
				vt.Visit(body.Left);
				foreach (var expression in vt.Lefts)
				{
					expression.MaybeAs<MemberExpression>().Do((c) => listOfProperty.Add(c.Member.Name));
				}
				
			}
			var rightProperty = body.Right.MaybeAs<MethodCallExpression>().Bind(c => c.Object).GetOrDefault(null).MaybeAs<MemberExpression>().Bind(c => c.Member.Name).GetOrDefault(null);

			return new PredicateSet
			{
				Method = body.Method == null ? body.Left.MaybeAs<MethodCallExpression>().Bind(c => c.Method.Name).GetOrDefault(null) ?? "Contains" : body.Method.Name,
				Property = string.Join("&&", listOfProperty.Where(c => c != null).Distinct()) + "&&" + rightProperty,
				Value = listOfValues.FirstOrDefault(c => c != null)
			};
		}

		private static PredicateSet GetSinglePredicateSetFromMemberExpression(Maybe<MemberExpression> memBody)
		{
			if (memBody == Maybe.Nothing)
			{
				return null;
			}
			var body = memBody.Value;
			return new PredicateSet
			{
				Property = body.Member.Name,
				Value = "True",
				Method = "Equals"
			};
		}
	}
}
