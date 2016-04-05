using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models.Entities;
using FWD.UI.Web.Models.Entities.Dtos;

namespace FWD.UI.Web.Models.Helper
{
	public class GroupHelper
	{
		private readonly static Lazy<GroupHelper> _helper = new Lazy<GroupHelper>(()=>new GroupHelper());
		public static GroupHelper Instance
		{
			get
			{
				return _helper.Value;
			}
		}

		private GroupHelper()
		{
		}

		public GroupView GetXmlGroups(IEnumerable<ArticleGroup> groups, IEnumerable<Article> articles)
		{
			return new GroupView(groups, articles);
		}

		public GroupView GetDbGroups(IEnumerable<ArticleGroup> groups)
		{
			return new GroupView(groups);
		}

		public ArticleGroup InitGroup(ArticleGroup group, IEnumerable<Article> articles)
		{
			bool flag = false;
			if (articles == null || !articles.Any())
			{
				var helper = new IocHelper();
				articles = helper.ArticleService.GetArticlesByParams(c => group.Groups.Contains(c.ArticleName), c => c.ArticleId);
				flag = true;
			}
			@group.Articles = flag ? articles.ToList() : articles.Where(c => @group.Groups.Contains(c.ArticleName)).ToList();

			return @group;
		}

		private ArticleGroup GetGroup(string group)
		{
			var helper = new IocHelper();
			return @group != null
				? helper.GroupService.GetGroupsByParams(c => c.GroupName == @group, c => c.GroupId, 0, 1).FirstOrDefault()
				: helper.GroupService.GetGroupsByParams(c => c.GroupName == "Без группы", c => c.GroupId, 0, 1).FirstOrDefault();
		}

		public void InitGroups(ArticleDto article)
		{
			if (IocHelper.CurrentToggle == "xml")
			{
				var @group = GetGroup(article.Group);
				if (@group.Groups == null)
				{
					@group.Groups = new List<string>();
				}
				@group.Groups.Add(article.ArticleName);
				var helper = new IocHelper();
				helper.GroupService.UpdateGroup(@group);
			}
			
		}

	}
}
