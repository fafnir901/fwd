using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;

namespace FWD.CommonIterfaces
{
	public interface IManagmentEntityWithUser
	{
		void SetUser(User user);
		User GetUser();
	}
}
