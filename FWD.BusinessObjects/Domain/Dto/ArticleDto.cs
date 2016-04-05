using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.BusinessObjects.Domain.Dto
{
	public class ArticleDto
	{
		public int ArticleId { get; set; }
		public string ArticleName { get; set; }
		public DateTime AddedDate { get; set; }
	}
}
