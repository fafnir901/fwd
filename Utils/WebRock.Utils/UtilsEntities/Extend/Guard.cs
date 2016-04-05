using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace WebRock.Utils.UtilsEntities.Extend
{
	[DebuggerStepThrough]
	internal static class Guard
	{
		public static void NotNull<T>(Expression<Func<T>> reference, T value)
		{
			if ((object) value == null)
				throw new ArgumentNullException(Guard.GetParameterName((Expression) reference), "Parameter cannot be null.");
		}

		public static void NotNullOrEmpty(Expression<Func<string>> reference, string value)
		{
			Guard.NotNull<string>(reference, value);
			if (value.Length == 0)
				throw new ArgumentException("Parameter cannot be empty.", Guard.GetParameterName((Expression) reference));
		}

		public static void IsValid<T>(Expression<Func<T>> reference, T value, Func<T, bool> validate, string message)
		{
			if (!validate(value))
				throw new ArgumentException(message, Guard.GetParameterName((Expression) reference));
		}

		public static void IsValid<T>(Expression<Func<T>> reference, T value, Func<T, bool> validate, string format,
			params object[] args)
		{
			if (!validate(value))
				throw new ArgumentException(string.Format(format, args), Guard.GetParameterName((Expression) reference));
		}

		private static string GetParameterName(Expression reference)
		{
			return ((reference as LambdaExpression).Body as MemberExpression).Member.Name;
		}
	}
}
