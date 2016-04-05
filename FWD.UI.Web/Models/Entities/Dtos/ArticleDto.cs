using System.Collections.Generic;
using System.Linq;
using System.Web.Razor;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models.Helper;
using WebRock.Utils.Monad;

namespace FWD.UI.Web.Models.Entities.Dtos
{
	public class ArticleDto : IDto
	{
		public ArticleDto()
		{
			Images = new List<ImageDto>();
		}

		public ArticleDto(Article article)
		{
			Images = new List<ImageDto>();
			ArticleId = article.ArticleId.ToString();
			ArticleName = article.ArticleName;
			Link = article.Link;
			Content = article.HtmlText;
			AuthorName = article.AuthorName;
			Group = article.ArticleGroup.MaybeAs<ArticleGroup>()
						.Bind(c => c.GroupName)
						.GetOrDefault(null);
			Rate = article.Rate;
			if (article.Images != null && article.Images.Any())
			{
				foreach (var image in article.Images)
				{
					Images.Add(new ImageDto
					{
						ImageId = image.ImageId,
						ImageName = image.Name
					});
				}
			}
			Tags = article.Tags;
		}
		public string ArticleId { get; set; }
		public string ArticleName { get; set; }
		public int Rate { get; set; }
		public string AuthorName { get; set; }
		public string Link { get; set; }
		public string Content { get; set; }
		public List<ImageDto> Images { get; set; }

		public List<string> ImageIds { get; set; }

		public string Group { get; set; }

		public List<Tag> Tags { get; set; }

		public Article Convert()
		{
			int articleId;
			var article = new Article
			{
				ArticleId = this.ArticleId.MaybeAs<string>().Bind(c => string.IsNullOrEmpty(c) ? 0 : int.Parse(c)).GetOrDefault(0),
				AuthorName = this.AuthorName,
				InitialText = this.Content,
				ArticleName = this.ArticleName,
				Link = this.Link,
				ArticleGroup = this.Group == null ? null : new ArticleGroup { GroupName = this.Group},
				Rate = this.Rate
			};

			if (this.ImageIds != null)
			{
				var results = CommonHelper.Instance.TempFiles.Where(c => ImageIds.Contains(c.GuidId.ToString()));

				article.Images = new List<Image>();
				article.Images.AddRange(results.Select(c => new Image()
				{
					Data = c.ImageData,
					Name = c.Name,
				}));

				if (article.ArticleId != 0)
				{
					var helper = new IocHelper();
					var expArticle = helper.ArticleService.GetArticleById(article.ArticleId);
					article.Images.AddRange(expArticle.Images);
				}
			}

			if (this.Tags != null)
			{
				article.Tags = Tags;
			}
			return article;
		}

		public string Type
		{
			get { return "article"; }
		}

	}
}