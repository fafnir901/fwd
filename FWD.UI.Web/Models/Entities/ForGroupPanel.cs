using System;
using System.Collections.Generic;
using System.Linq;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Web.Models.Entities
{
	public class ForGroupPanel
	{
		public string Name { get; set; }
		public bool IsExpanded { get; set; }
		public int ArticleCount { get; set; }
		public List<ForPanel> Articles { get; set; }

		public ForGroupPanel()
		{
			Articles = new List<ForPanel>();
		}

		public ForGroupPanel(GroupViewItem view)
		{
			Articles = new List<ForPanel>();
			this.Name = view.Name;
			this.IsExpanded = view.IsExpanded;
			this.ArticleCount = view.ArticlesCount;
			Func<IEnumerable<Tag>, string> define = (items) =>
			{
				return
					"[{0}]".Fmt(string.Join(",", items
							.OrderByDescending(c => c.Priority)
							.Select(c => string.Format("{0}{5}Name{5}:{5}{1}{5},{5}TagColor{5}:{5}{2}{5},{5}Priority{5}:{5}{3}{5}{4}", "{", c.Name, c.TagColor, c.Priority, "}", "'"))));

			};
			Articles = view.Articles.Select(c => new ForPanel { Id = c.ArticleId, Name = c.ArticleName, Rate = c.Rate, CountOfImages = c.Images.Count, CreationDate = c.CreationDate, Tags = define(c.Tags) }).OrderBy(c => c.Name).ToList();
		}
	}
}