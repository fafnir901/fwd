using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Absrtact;

namespace FWD.DAL.Entities
{
	public class Article : IArticle
	{
		[Key]
		public int ArticleId { get; set; }
		[MaxLength(int.MaxValue)]
		public string ArticleName { get; set; }
		[MaxLength(int.MaxValue)]
		public string InitialText { get; set; }
		[MaxLength(int.MaxValue)]
		public string AuthorName { get; set; }
		[MaxLength(500)]
		public string Link { get; set; }
		public DateTime CreationDate { get; set; }
		public int Rate { get; set; }
		public virtual ICollection<EmbdedImage> EmbdedImages { get; set; }
		public virtual ICollection<Tag> Tags { get; set; }
		public virtual ArticleGroup Group { get; set; }

	}
}
