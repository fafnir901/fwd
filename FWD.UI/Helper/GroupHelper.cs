using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Helper
{
	public class GroupHelper
	{
		private static GroupHelper _helper;
		public static GroupHelper Instance
		{
			get
			{
				if (_helper == null)
				{
					_helper = new GroupHelper();
				}
				return _helper;
			}
		}

		private GroupHelper()
		{
		}

		public GroupView GetGroups(IEnumerable<ArticleGroup> groups, IEnumerable<Article> articles)
		{
			return new GroupView(groups, articles);
		}

		public ArticleGroup InitGroup(ArticleGroup group, IEnumerable<Article> articles)
		{
			bool flag = false;
			if (articles == null || !articles.Any())
			{
				articles = IocHelper.ArticleService.GetArticlesByParams(c => group.Groups.Contains(c.ArticleName), c => c.ArticleId);
				flag = true;
			}
			@group.Articles = flag ? articles.ToList() : articles.Where(c => @group.Groups.Contains(c.ArticleName)).ToList();

			return @group;
		}

	}

	public class GroupViewItem
	{
		private ArticleGroup _group;
		public GroupViewItem(ArticleGroup group, IEnumerable<Article> articles)
		{
			_group = group;
			Articles = articles == null ? new List<Article>() : articles.ToList();
			IsExpanded = false;
			ArticlesCount = Articles.Count;
			Name =  _group.GroupName;
		}

		public bool IsExpanded { get; set; }

		public string Name { get; set; }

		public int ArticlesCount { get; set; }
		public List<Article> Articles { get; set; }
	}

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
	}
}
