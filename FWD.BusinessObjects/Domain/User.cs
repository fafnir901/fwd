using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class User
	{
		public User()
		{
			Articles = new List<Article>();
			ArticlesNamesList = new List<string>();
		}
		[XmlAttribute]
		public int UserId { get; set; }
		[XmlAttribute]
		public string FirstName { get; set; }
		[XmlAttribute]
		public string LastName { get; set; }
		[XmlAttribute]
		public string Email { get; set; }
		[XmlElement]
		public byte[] Avatar { get; set; }
		[XmlElement]
		public UserRoleEnum UserRole { get; set; }
		[XmlElement]
		public Credential Credential { get; set; }
		[XmlElement]
		public List<string> ArticlesNamesList { get; set; }
		[XmlIgnore]
		public List<Article> Articles { get; set; }

		public User Clone()
		{
			var user = (User)MemberwiseClone();
			user.Credential = this.Credential.Clone();
			return user;
		}

	}

	public static class UserRoleExtensions
	{
		public static string RoleName(this UserRoleEnum role)
		{
			switch (role)
			{
				case UserRoleEnum.Admin:
					return "Admin";
				case UserRoleEnum.Default:
					return "Default";
				default: return "Default";
			}
		}
	}
}
