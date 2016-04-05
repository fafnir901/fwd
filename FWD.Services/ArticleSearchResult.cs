using System;
using System.Collections.Generic;
using System.Text;
using FWD.BusinessObjects.Domain;

namespace FWD.Services
{
	public class ArticleSearchResult
	{
		public int ArticleId { get; private set; }
		public string ArtilceName { get; private set; }
		public string PartOfArticleText { get; private set; }
		public string Author { get; private set; }
		public string GeneratedLink { get; private set; }

		public List<Tag> Tags { get; private set; } 

		public ArticleSearchResult(Article article, string searchString, string template)
		{
			try
			{
				var text = article.NonHtmlText.ToLower().Contains(searchString.ToLower());

				ArticleId = article.ArticleId;
				ArtilceName = article.ArticleName;
				if (text)
				{
					var index = article.NonHtmlText.ToLower().IndexOf(searchString.ToLower());
					var startIndex = index - 250 > 0 ? index - 250 : index;
					var count = index + 500 > article.NonHtmlText.Length ? article.NonHtmlText.Length - index : 500;
					var builder = new StringBuilder();
					builder.Append("... ");
					builder.Append(article.NonHtmlText.Substring(startIndex, count));
					builder.Append(" ...");
					PartOfArticleText = builder.ToString();
				}
				else
				{
					PartOfArticleText = article.NonHtmlText.Substring(0, 250);
				}

				Author = article.AuthorName;
				GeneratedLink = template.Fmt(ArticleId);
				Tags = article.Tags;

			}
			catch (Exception)
			{
				throw;
			}

		}
	}
}