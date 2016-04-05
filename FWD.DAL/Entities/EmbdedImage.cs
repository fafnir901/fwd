using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.DAL.Entities
{
	public class EmbdedImage
	{
		[Key]
		public int ImageId { get; set; }
		public string Name { get; set; }
		[MaxLength(int.MaxValue)]
		public byte[] Data { get; set; }
		public virtual Article Article { get; set; }
	}
}
