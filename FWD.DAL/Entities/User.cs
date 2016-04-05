using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.DAL.Entities
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public string Email { get; set; }
		public byte[] Avatar { get; set; }
		[Required]
		public int UserRole { get; set; }
		[Required]
		public string Login { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
