using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.Annotations
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class LocalizationRequiredAttribute : Attribute
	{
		[UsedImplicitly]
		public bool Required { get; set; }

		public LocalizationRequiredAttribute(bool required)
		{
			this.Required = required;
		}

		public override bool Equals(object obj)
		{
			LocalizationRequiredAttribute requiredAttribute = obj as LocalizationRequiredAttribute;
			return requiredAttribute != null && requiredAttribute.Required == this.Required;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
