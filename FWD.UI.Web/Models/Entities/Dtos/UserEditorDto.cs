using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Models.Entities.Dtos
{
	public class UserEditorDto
	{
		public UserEditorDto(User user)
		{
			User = new UserViewDto
			{
				FirstName = user.FirstName,
				UserRole = user.UserRole,
				Id = user.UserId,
				LastName = user.LastName,
				Email = user.Email,
				Login = user.Credential.Login,
				Password = user.Credential.Password
			};
		}
		public bool IsAdminFunctionEnable
		{
			get { return User.UserRole == UserRoleEnum.Admin; }
		}

		public UserViewDto User { get; set; }

		public ReadOnlyCollection<string> AvailableRoles
		{
			get
			{
				return new List<string>
				{
					UserRoleEnum.Admin.RoleName(),
					UserRoleEnum.Default.RoleName()
				}.AsReadOnly();
			}
		}

		public List<UserViewDto> AllUsers
		{
			get
			{
				if (IsAdminFunctionEnable)
				{
					var helper = new IocHelper();
					return helper.UserService.GetAllUsers(0, 999, c => c.LastName).Select(c=> new UserViewDto
					{
						FirstName = c.FirstName,
						UserRole = c.UserRole,
						Id = c.UserId,
						LastName = c.LastName,
						Email = c.Email,
						Login = c.Credential.Login,
						Password = c.Credential.Password
					}).ToList();
				}
				return Enumerable.Empty<UserViewDto>().ToList();
			}
		}
	}

	public class UserViewDto
	{
		public int Id { get; set; }
		public UserRoleEnum UserRole { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }
		public string Login { get; set; }
	}
}