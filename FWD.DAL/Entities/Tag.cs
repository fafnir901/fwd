using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FWD.BusinessObjects.Absrtact;

namespace FWD.DAL.Entities
{
	public class Tag : ITag
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int TagType { get; set; }
		[Required]
		public string TagColor { get; set; }
		[Required]
		public int Priority { get; set; }

		public virtual ICollection<Article> Articles { get; set; }

		public override string ToString()
		{
			var inner = string.Format("Id:{0};Name:{1};TagType:{2};TagColor:{3};Priority:{4}",Id, Name, TagType, TagColor,Priority);
			return inner;
		}
	}
}
