using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;

namespace FWD.BusinessObjects.Absrtact
{
	public interface ITag : IEntity
	{
		int TagType { get; set; }
		string TagColor { get; set; }
		int Priority { get; set; }
	}


	public enum TagType
	{
		Default = 0
	}

	public enum TagPriority
	{
		Low = 0,
		Medium = 1,
		High = 2
	}
}
