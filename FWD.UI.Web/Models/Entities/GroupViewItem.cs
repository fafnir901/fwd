using System.Collections.Generic;
using System.Linq;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Web.Models.Entities
{
	public class GroupViewItem
	{
		private ArticleGroup _group;
		public GroupViewItem(ArticleGroup group, IEnumerable<Article> articles)
		{
			_group = group;
			Articles = articles == null ? new List<Article>() : articles.ToList();
			IsExpanded = false;
			ArticlesCount = Articles.Count;
			Name = _group.GroupName;
		}

		public bool IsExpanded { get; set; }

		public string Name { get; set; }

		public int ArticlesCount { get; set; }
		public List<Article> Articles { get; set; }
	}
}