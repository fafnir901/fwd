using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.CommonIterfaces
{
	public class CommonContants
	{
		public LoggerConstants LoggerConstants { get; set; }
	}

	public class LoggerConstants
	{
		public const string DATA_NOT_FOUND_WARNING = "Данные не найдены";
		public const string DATA_WILL_BE_REMOVED = "Данные будут удалены";

	}
}
