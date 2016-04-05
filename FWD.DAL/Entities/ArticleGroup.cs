using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.DAL.Entities
{
	public class ArticleGroup
	{
		[Key]
		public int GroupId { get; set; }

		[MaxLength(250,ErrorMessage = "Название группы не должно превышать 250 символов")]
		public string GroupName { get; set; }

		public virtual ICollection<Article> Articles { get; set; }

		public override string ToString()
		{
			return string.Format("ID:{0};GroupName:{1}", GroupId, GroupName);
		}
	}
}
