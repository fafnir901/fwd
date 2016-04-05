using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.CommonIterfaces
{
	public interface ILogger
	{
		string Path { get; set; }
		void Trace(string action);
		void TraceError(string action, Exception ex);
		void TraceWarning(string action, string warning);
	}
}
