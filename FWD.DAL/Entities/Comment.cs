using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.DAL.Entities
{
	public class Comment
	{
		[Key]
		public Guid CommentId { get; set; }

		[MaxLength(150,ErrorMessage = "Длина комментария не может привышать 150 символов")]
		[Required]
		public string CommentText { get; set; }
		[Required]
		public string GroupName { get; set; }
		[Required]
		public string UserName { get; set; }
		[Required]
		public DateTime AddedDate { get; set; }

		public int UserId { get; set; }
	}
}
