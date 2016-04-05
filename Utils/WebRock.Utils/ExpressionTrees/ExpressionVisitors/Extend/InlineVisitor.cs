using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DelegateDecompiler;
using WebRock.Utils.Annotations.Attributes;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities.Extend;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors.Extend
{
	internal class InlineVisitor : ExpressionVisitor
	{
		private readonly IEnumerable<object> _inlineEnvironment;
		public InlineVisitor(IEnumerable<object> inlineEnvironment)
		{
			this._inlineEnvironment = inlineEnvironment;
		}
		protected override Expression VisitMethodCall(MethodCallExpression target)
		{
			InlineableAttribute customAttribute = target.Method.GetCustomAttribute<InlineableAttribute>();
			Expression result;
			if (customAttribute.ToMaybe<InlineableAttribute>().HasValue)
			{
				result = this.Visit(this.InlineExpression(target.Object, target.Method, customAttribute, target.Arguments));
			}
			else
			{
				result = base.VisitMethodCall(target);
			}
			return result;
		}
		protected override Expression VisitMember(MemberExpression node)
		{
			Expression result;
			if (node.Member.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)node.Member;
				InlineableAttribute customAttribute = propertyInfo.GetCustomAttribute<InlineableAttribute>();
				if (customAttribute.ToMaybe<InlineableAttribute>().HasValue)
				{
					MethodInfo getMethod = propertyInfo.GetGetMethod();
					if (getMethod.IsAbstract)
					{
						PropertyInfo property = node.Expression.Type.GetProperty(node.Member.Name);
						getMethod = property.GetGetMethod();
					}
					ReadOnlyCollection<Expression> targetArguments = new List<Expression>().AsReadOnly();
					result = this.Visit(this.InlineExpression(node.Expression, getMethod, customAttribute, targetArguments));
					return result;
				}
			}
			result = base.VisitMember(node);
			return result;
		}
		private Expression InlineExpression(Expression thisExpression, MethodInfo targetMethod, Maybe<InlineableAttribute> inlinableAttribute, ReadOnlyCollection<Expression> targetArguments)
		{
			Try<LambdaExpression> @try = InlineVisitor.FindMethodToInline(targetMethod, inlinableAttribute.Value.InlineMethodName).Select<LambdaExpression>(delegate(MethodInfo methodInfo)
			{
				object[] parameterValuesToInline = InlineVisitor.GetParameterValuesToInline(targetMethod, targetArguments, methodInfo, this._inlineEnvironment);
				return (LambdaExpression)methodInfo.Invoke(null, parameterValuesToInline);
			}).Recover((Exception exception) => Try.Create<LambdaExpression>(new Func<LambdaExpression>(targetMethod.Decompile)).Recover((Exception _) => new Failure<LambdaExpression>(exception)));
			IEnumerable<Expression> newExpr = (thisExpression == null) ? targetArguments : new Expression[]
			{
				thisExpression
			}.Concat(targetArguments);
			return @try.Value.Splice(newExpr);
		}
		private static object[] GetParameterValuesToInline(MethodInfo targetMethod, ReadOnlyCollection<Expression> targetArguments, MethodInfo methodToInline, IEnumerable<object> inlineEnvironments)
		{
			var inner = targetMethod.GetParameters().Select((ParameterInfo parameter, int index) => new
			{
				value = targetArguments[index],
				parameter = parameter
			}).ToArray();
			var outer = methodToInline.GetParameters().Select((ParameterInfo parameter, int index) => new
			{
				index,
				parameter
			}).ToArray();
			IEnumerable<object> source =
				from inlineParam in outer
				join targetParam in inner on inlineParam.parameter.Name equals targetParam.parameter.Name into targetParameterTmpCollection
				from targetParameter in targetParameterTmpCollection.DefaultIfEmpty()
				let value = (targetParameter == null) ? InlineVisitor.GetValueFromEnvironment(inlineParam.parameter.ParameterType, inlineEnvironments, methodToInline) : InlineVisitor.GetValueByExpression(targetParameter.value, methodToInline)
				orderby inlineParam.index
				select value;
			return source.ToArray<object>();
		}
		private static object GetValueByExpression(Expression expression, MethodInfo methodToInline)
		{
			ConstantExpression constantExpression = expression as ConstantExpression;
			if (constantExpression == null)
			{
				throw new NotSupportedException("Only constant arguments can be passed to inline method {0}.{1}".Fmt(new object[]
				{
					methodToInline.DeclaringType,
					methodToInline.Name
				}));
			}
			return constantExpression.Value;
		}
		private static object GetValueFromEnvironment(Type parameterType, IEnumerable<object> inlineEnvironments, MethodInfo methodToInline)
		{
			object[] array = inlineEnvironments.Where(new Func<object, bool>(parameterType.IsInstanceOfType)).ToArray<object>();
			if (array.Length == 0)
			{
				throw new InvalidOperationException("There is no overload of inlinable method {0}.{1}.".Fmt(new object[]
				{
					methodToInline.DeclaringType,
					methodToInline.Name
				}));
			}
			if (array.Length > 1)
			{
				throw new InvalidOperationException("It's more then 1 elemnt of type {0} in inline environment. Inline method {1}.{2} can't be called.".Fmt(new object[]
				{
					parameterType,
					methodToInline.DeclaringType,
					methodToInline.Name
				}));
			}
			return array.Single<object>();
		}
		private static Try<MethodInfo> FindMethodToInline(MemberInfo member, string inlineMethodName)
		{
			var methodInfo2 = member as MethodInfo;
			Func<MethodInfo, bool> predicate;
			if (methodInfo2 == null)
			{
				predicate = ((MethodInfo methodInfo) => methodInfo.GetParameters().Length == 0);
			}
			else
			{
				var targetParams = methodInfo2.GetParameters().Select(x => new { x.Name, x.ParameterType }).ToArray();
				predicate = (MethodInfo candidateMethod) => (
					candidateMethod.GetParameters()
						.Where(x => !x.GetCustomAttribute<InlineEnvironmentAttribute>().ToMaybe<InlineEnvironmentAttribute>().HasValue)
						.Select(candidateParam => new
						{
							candidateParam.Name,
							candidateParam.ParameterType
						}).All(targetParams.Contains)); //.All(new Func<Func<string, Type>, bool>(targetParams.Contains)));
			}
			string methodName = inlineMethodName ?? member.Name;
			MethodInfo[] array = (
				member.DeclaringType.GetMethods()
					.Where(x => x.Name == methodName && typeof (Expression).IsAssignableFrom(x.ReturnType))).Where(predicate).ToArray<MethodInfo>();
			Try<MethodInfo> result;
			if (array.Length > 1)
			{
				result = new Failure<MethodInfo>(new InvalidOperationException("It's more than 1 overload of inlinable method {0}.{1}.".Fmt(new object[]
				{
					member.DeclaringType,
					methodInfo2.Name
				})));
			}
			else
			{
				if (array.Length == 0)
				{
					result = new Failure<MethodInfo>(new InvalidOperationException("There is no overload of inlinable method {0}.{1}.".Fmt(new object[]
					{
						member.DeclaringType,
						methodInfo2.Name
					})));
				}
				else
				{
					result = new Success<MethodInfo>(array.Single<MethodInfo>());
				}
			}
			return result;
		}
	}
}
