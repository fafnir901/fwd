using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace WebRock.UnitTests.Common
{
	/// <summary>
	/// The Be class is a synonym for Is intended for use with the Should extension methods for more DSL-like syntax
	/// </summary>
	public class Be : Is
	{
		public static CollectionEquivalentConstraint EquivalentTo<T>(IEnumerable<T> expected)
		{
			if (!expected.GetType().IsArray)
				expected = expected.ToArray();

			return new CollectionEquivalentConstraint(expected);
		}
	}
}
