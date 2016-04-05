using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Absrtact;

namespace FWD.BusinessObjects.Domain.Dto
{
	public class TagsCommonInfo
	{
		public List<ITag> Tags { get; set; }

		public IEnumerable<string> TagColors
		{
			get
			{
				yield return "blue";
				yield return "green";
				yield return "red";
				yield return "yellow";
			}
		}

		public Dictionary<string, string> TagTypes { get; set; }

		public TagsCommonInfo(IEnumerable<ITag> tags)
		{
			Tags = tags == null ? new List<ITag>() : tags.ToList();
			TagTypes = new Dictionary<string, string>
			{
				{"0", "Standard"}
			};

		}
	}
}
