using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Absrtact;

namespace FWD.DAL.Entities
{
	public class Plan : IPlan
	{
		[Key]
		public int Id { get; set; }
		[MaxLength(500)]
		[Required]
		public string Name { get; set; }
		[MaxLength(int.MaxValue)]
		[Required]
		public string Description { get; set; }
		public DateTime AddedDate { get; set; }
		public DateTime? PossibleChangeDate { get; set; }
		public bool IsDone { get; set; }
		public IPlan Clone()
		{
			return (IPlan) this.MemberwiseClone();
		}
	}
}
