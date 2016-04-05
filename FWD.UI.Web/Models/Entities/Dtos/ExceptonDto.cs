using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWD.UI.Web.Models.Entities.Dtos
{
	public class ExceptonDto:IDto
	{
		public string Type
		{
			get { return "exception"; }
		}

		public string ExceptionMessage { get; set; }
		public Exception Exception { get; set; }
	}
}