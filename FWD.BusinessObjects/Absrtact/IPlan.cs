using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRock.Utils.Annotations;

namespace FWD.BusinessObjects.Absrtact
{
	public interface IPlan : IEntity
	{
		string Description { get; set; }
		DateTime AddedDate { get; set; }
		DateTime? PossibleChangeDate { get; set; }
		bool IsDone { get; set; }
		IPlan Clone();
	}
}
