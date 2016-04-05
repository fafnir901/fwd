using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;

namespace FWD.Services
{
	public static class UserConfigService
	{
		public static User _loggedUser;
		public static bool IsMissingAuth { get; set; }

		public static User LoggedUser {
			get
			{
				return _loggedUser;
			}
		}

		public static bool CheckExistingUser(string password, string login, UserService service)
		{
			var result = service.GetUserByCredential(password, login);
			if (result == null)
			{
				throw new AuthenticationException("Пользователь с такой комбинацией пароля и логина не был найден.");
			}
			_loggedUser = result;
			return true;
		}

		public static void ResetLogin()
		{
			_loggedUser = null;
		}
	}
}
