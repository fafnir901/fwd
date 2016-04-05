using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.Annotations
{
	[Flags]
	public enum ImplicitUseTargetFlags
	{
		Default = 1,
		Itself = Default,
		Members = 2,
		WithMembers = Members | Itself,
	}
}
