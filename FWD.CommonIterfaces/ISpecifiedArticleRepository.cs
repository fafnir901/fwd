using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using WebRock.Utils.UtilsEntities;

namespace FWD.CommonIterfaces
{
	public interface ISpecifiedArticleRepository
	{
		IEnumerable<Article> GetWithTags(string searchString, QueryParams<Article> param);
		IEnumerable<Article> GetByTagWithTags(IEnumerable<int> ids, QueryParams<Article> param);
	}
}
