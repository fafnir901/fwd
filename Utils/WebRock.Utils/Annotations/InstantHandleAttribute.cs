using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.Annotations
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
	public sealed class InstantHandleAttribute : Attribute
	{
	}
}
