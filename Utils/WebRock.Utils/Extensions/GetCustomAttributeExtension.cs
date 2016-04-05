using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebRock.Utils;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities.Extend;

namespace System.Reflection
{
	internal static class GetCustomAttributeExtension
	{
		public static Maybe<TAttribute> GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true) where TAttribute : Attribute
		{
			return MaybeExtension.FirstOrNothing<TAttribute>(GetCustomAttributeExtension.GetCustomAttributes<TAttribute>(provider, inherit));
		}

		public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true) where TAttribute : Attribute
		{
			Guard.NotNull<ICustomAttributeProvider>((Expression<Func<ICustomAttributeProvider>>)(() => provider), provider);
			return Enumerable.Cast<TAttribute>((IEnumerable)provider.GetCustomAttributes(typeof(TAttribute), inherit));
		}

		public static Maybe<TAttribute> GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
		{
			Type type = value.GetType();
			string name = Enum.GetName(type, (object)value);
			return MaybeExtension.FirstOrNothing<TAttribute>(Enumerable.OfType<TAttribute>((IEnumerable)type.GetField(name).GetCustomAttributes(true)));
		}
	}
}
