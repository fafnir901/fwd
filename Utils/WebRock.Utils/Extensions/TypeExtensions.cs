using System;
using System.Reflection;
using WebRock.Utils.Monad;

namespace System
{
	public static class TypeExtensions
	{
		public static object DefaultValue(this Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
		public static Type ResultType(this MemberInfo memberInfo)
		{
			return memberInfo.MaybeAs<PropertyInfo>(true)
				.Bind((PropertyInfo x) => x.PropertyType)
				.OrElse(() =>
					memberInfo.MaybeAs<MethodInfo>(true)
					.Bind((MethodInfo x) => x.ReturnType))
					.OrElse(() =>
						memberInfo.MaybeAs<FieldInfo>(true)
						.Bind((FieldInfo x) => x.FieldType)).Value;
		}
	}
}
