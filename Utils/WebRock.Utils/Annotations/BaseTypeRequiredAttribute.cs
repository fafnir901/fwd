using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.Annotations
{
	[BaseTypeRequired(typeof(Attribute))]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class BaseTypeRequiredAttribute : Attribute
	{
		public Type[] BaseTypes { get; private set; }

		public BaseTypeRequiredAttribute(Type baseType)
		{
			this.BaseTypes = new Type[1]
			{
				baseType
			};
		}
	}
}
