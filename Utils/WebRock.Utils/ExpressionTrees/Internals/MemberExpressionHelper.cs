using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Internals
{
	public class MemberExpressionHelper : IMemberHelper
	{
		public MemberExpression DefineMemberExpression(ParameterExpression parameter, PropertyInfo property, string propertyName, IEnumerable<PropertyInfo> listOfProp)
		{
			MemberExpression memberExpression;
			if (property == (PropertyInfo)null)
			{
				List<PropertyInfo> list = Enumerable.ToList<PropertyInfo>(listOfProp);
				property = Enumerable.First<PropertyInfo>((IEnumerable<PropertyInfo>)list);
				list.Remove(Enumerable.First<PropertyInfo>((IEnumerable<PropertyInfo>)list));
				MemberExpression seed = Expression.MakeMemberAccess((Expression)parameter, (MemberInfo)property);
				memberExpression = Enumerable.Aggregate<PropertyInfo, MemberExpression>((IEnumerable<PropertyInfo>)list, seed, new Func<MemberExpression, PropertyInfo, MemberExpression>(Expression.MakeMemberAccess));
			}
			else
				memberExpression = Expression.MakeMemberAccess((Expression)parameter, (MemberInfo)property);
			return memberExpression;
		}

		public void GetProperty<TTo>(Type type, string memberName, IList<PropertyInfo> properties)
		{
			if (type != typeof(TTo))
			{
				List<Type> exceptTypeList = new List<Type>();
				PropertyInfo property = type.GetProperty(memberName);
				PropertyInfo propertyInfo = Enumerable.FirstOrDefault<PropertyInfo>((IEnumerable<PropertyInfo>)typeof(TTo).GetProperties(), (Func<PropertyInfo, bool>)(c => c.PropertyType == type));
				Type nestedType = this.GetPropertyType(typeof(TTo));
				Type exceptType = nestedType;
				if (nestedType != (Type)null)
					properties.Add(this.GetPropertyInfoFromType(typeof(TTo)));
				while (propertyInfo == (PropertyInfo)null)
				{
					bool flag = false;
					if (nestedType != (Type)null)
					{
						using (IEnumerator<PropertyInfo> enumerator = Enumerable.Where<PropertyInfo>((IEnumerable<PropertyInfo>)nestedType.GetProperties(), (Func<PropertyInfo, bool>)(pInfo => pInfo.PropertyType == type)).GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								propertyInfo = enumerator.Current;
								properties.Add(propertyInfo);
								flag = true;
							}
						}
						if (nestedType == type)
						{
							propertyInfo = Enumerable.FirstOrDefault<PropertyInfo>((IEnumerable<PropertyInfo>)nestedType.GetProperties());
							properties.Add(propertyInfo);
							flag = true;
						}
						nestedType = this.GetPropertyType(nestedType);
						if (nestedType != (Type)null && !flag)
							properties.Add(this.GetPropertyInfoFromType(nestedType));
					}
					else
					{
						nestedType = this.GetPropertyType(typeof(TTo), exceptType);
						if (nestedType.IsInterface)
							nestedType = Enumerable.First<Type>((IEnumerable<Type>)nestedType.GenericTypeArguments);
						exceptTypeList.Add(exceptType);
						while (exceptTypeList.Contains(nestedType))
						{
							nestedType = this.GetPropertyType(typeof(TTo), exceptTypeList);
							exceptTypeList.Add(exceptType);
							exceptType = nestedType;
						}
						exceptType = nestedType;
					}
				}
				properties.Add(property);
			}
			else
				properties.Add(typeof(TTo).GetProperty(memberName));
		}

		public PropertyInfo GetPropertyInfoFromType(Type nestedType)
		{
			return MaybeExtension.GetOrDefault<PropertyInfo>(MaybeExtension.MaybeAs<PropertyInfo>((object)Enumerable.FirstOrDefault<PropertyInfo>((IEnumerable<PropertyInfo>)nestedType.GetProperties(), (Func<PropertyInfo, bool>)(c => !this.IsStandardType(c.PropertyType))), true), (PropertyInfo)null);
		}

		public Type GetPropertyType(Type nestedType)
		{
			return MaybeExtension.GetOrDefault<Type>(MaybeExtension.Bind<PropertyInfo, Type>(MaybeExtension.MaybeAs<PropertyInfo>((object)Enumerable.FirstOrDefault<PropertyInfo>((IEnumerable<PropertyInfo>)nestedType.GetProperties(), (Func<PropertyInfo, bool>)(c => !this.IsStandardType(c.PropertyType))), true), (Func<PropertyInfo, Type>)(c => c.PropertyType)), (Type)null);
		}

		public Type GetPropertyType(Type nestedType, Type exceptType)
		{
			return MaybeExtension.GetOrDefault<Type>(MaybeExtension.Bind<PropertyInfo, Type>(MaybeExtension.MaybeAs<PropertyInfo>((object)Enumerable.FirstOrDefault<PropertyInfo>((IEnumerable<PropertyInfo>)nestedType.GetProperties(), (Func<PropertyInfo, bool>)(c => !this.IsStandardType(c.PropertyType) && c.PropertyType != exceptType)), true), (Func<PropertyInfo, Type>)(c => c.PropertyType)), (Type)null);
		}

		public Type GetPropertyType(Type nestedType, List<Type> exceptTypeList)
		{
			return MaybeExtension.GetOrDefault<Type>(MaybeExtension.Bind<PropertyInfo, Type>(MaybeExtension.MaybeAs<PropertyInfo>((object)Enumerable.FirstOrDefault<PropertyInfo>((IEnumerable<PropertyInfo>)nestedType.GetProperties(), (Func<PropertyInfo, bool>)(c => !this.IsStandardType(c.PropertyType) && !exceptTypeList.Contains(c.PropertyType))), true), (Func<PropertyInfo, Type>)(c => c.PropertyType)), (Type)null);
		}

		public string GetMemberName(Expression expression)
		{
			return Enumerable.FirstOrDefault<string>((IEnumerable<string>)new List<string>()
      {
        MaybeExtension.GetOrDefault<string>(MaybeExtension.Bind<MemberExpression, string>(MaybeExtension.MaybeAs<MemberExpression>((object) expression, true), (Func<MemberExpression, string>) (p => p.Member.Name)), (string) null),
        MaybeExtension.GetOrDefault<string>(MaybeExtension.Bind<UnaryExpression, string>(MaybeExtension.MaybeAs<UnaryExpression>((object) expression, true), (Func<UnaryExpression, Maybe<string>>) (t => MaybeExtension.Bind<MemberExpression, string>(MaybeExtension.MaybeAs<MemberExpression>((object) t.Operand, true), (Func<MemberExpression, string>) (p => p.Member.Name)))), (string) null),
        MaybeExtension.GetOrDefault<string>(MaybeExtension.Bind<MethodCallExpression, string>(MaybeExtension.MaybeAs<MethodCallExpression>((object) expression, true), (Func<MethodCallExpression, Maybe<string>>) (p => MaybeExtension.Bind<MemberExpression, string>(MaybeExtension.MaybeAs<MemberExpression>((object) MaybeExtension.GetOrDefault<Expression>(MaybeExtension.Bind<LambdaExpression, Expression>(MaybeExtension.MaybeAs<LambdaExpression>((object) Enumerable.Last<Expression>((IEnumerable<Expression>) p.Arguments), true), (Func<LambdaExpression, Expression>) (c => c.Body)), (Expression) null), true), (Func<MemberExpression, string>) (c => c.Member.Name)))), (string) null)
      }, (Func<string, bool>)(c => c != null));
		}

		public Type GetMemberType(Expression expression)
		{
			return MaybeExtension.GetOrDefault<Type>(MaybeExtension.Bind<MemberExpression, Type>(MaybeExtension.MaybeAs<MemberExpression>((object)expression, true), (Func<MemberExpression, Type>)(p => p.Type)), (Type)null) ?? MaybeExtension.GetOrDefault<Type>(MaybeExtension.Bind<UnaryExpression, Type>(MaybeExtension.MaybeAs<UnaryExpression>((object)expression, true), (Func<UnaryExpression, Type>)(t => t.Operand.Type)), (Type)null);
		}

		private bool IsStandardType(Type type)
		{
			return type == typeof(string) || type == typeof(DateTime) || type == typeof(TimeSpan) || type.IsPrimitive;
		}
	}
}
