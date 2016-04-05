using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.BusinessObjects.Absrtact
{
	public interface IArticle
	{
		int ArticleId { get; set; }
		string ArticleName { get; set; }
		string InitialText { get; set; }
		string AuthorName { get; set; }
		string Link { get; set; }
		DateTime CreationDate { get; set; }
		int Rate { get; set; }
	}
}
