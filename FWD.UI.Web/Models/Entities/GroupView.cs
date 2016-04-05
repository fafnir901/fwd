using System.Collections.Generic;
using System.Linq;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Web.Models.Entities
{
	public class GroupView
	{
		public List<GroupViewItem> GroupViewItems { get; private set; }

		public GroupView(IEnumerable<ArticleGroup> groups, IEnumerable<Article> articles)
		{
			GroupViewItems = new List<GroupViewItem>();

			var lst = new List<Article>();
			foreach (var articleGroup in groups)
			{
				var currentArticles = articles.Where(c => articleGroup.Groups.Contains(c.ArticleName)).ToList();
				if (currentArticles.Any())
				{
					GroupViewItems.Add(new GroupViewItem(articleGroup, currentArticles));
					lst.AddRange(currentArticles);
				}
			}

			var withOutcurrentArticles = articles.Except(lst);
			if (withOutcurrentArticles.Any())
			{
				var res = GroupViewItems.FirstOrDefault(c => c.Name == "Без группы");
				if (res == null)
				{
					res = new GroupViewItem(groups.First(c => c.GroupName == "Без группы"), new List<Article>());
					GroupViewItems.Add(res);
				}
				res.Articles.AddRange(withOutcurrentArticles);
				res.ArticlesCount = res.Articles.Count;
			}

			var withOutArticles = groups.Where(c => !GroupViewItems.Select(x => x.Name).Contains(c.GroupName));
			foreach (var withOutArticle in withOutArticles)
			{
				GroupViewItems.Add(new GroupViewItem(withOutArticle, null));
			}

			GroupViewItems = GroupViewItems.OrderBy(c => c.Name).ToList();
		}

		public GroupView(IEnumerable<ArticleGroup> groups)
		{
			GroupViewItems = new List<GroupViewItem>();
			foreach (var articleGroup in groups)
			{
				GroupViewItems.Add(new GroupViewItem(articleGroup, articleGroup.Articles));
			}

		}
	}
}