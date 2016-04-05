using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FWD.BusinessObjects.Absrtact;
using WebRock.Utils.Monad;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class Article : IArticle
	{
		[XmlAttribute]
		public int ArticleId { get; set; }
		[XmlAttribute]
		public string ArticleName { get; set; }
		[XmlAttribute]
		public string InitialText { get; set; }
		[XmlAttribute]
		public string Link { get; set; }
		[XmlAttribute]
		public string AuthorName { get; set; }
		[XmlAttribute]
		public DateTime CreationDate { get; set; }

		[XmlAttribute]
		public int Rate { get; set; }
		[XmlElement]
		public List<Image> Images { get; set; }

		[XmlElement]
		public List<Tag> Tags { get; set; }
		[XmlElement]
		public ArticleGroup ArticleGroup { get; set; }

		[XmlIgnore]
		public string HtmlText { get { return InitialText; } }

		[XmlIgnore]
		public string NonHtmlText { get { return ScrubHtml(InitialText); } }

		[XmlAttribute]
		public int GroupId { get; set; }

		public Article Clone()
		{
			return new Article
			{
				ArticleId = this.ArticleId,
				ArticleName = this.ArticleName,
				Link = this.Link,
				InitialText = this.InitialText,
				Images = this.Images == null ? null : this.Images.Select(c => c.Clone()).ToList(),
				CreationDate = this.CreationDate
			};
		}
		public static List<Article> Clone(IEnumerable<Article> actual)
		{
			return actual.Select(c => c.Clone()).ToList();
		}

		private static string ScrubHtml(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
				var step2 = Regex.Replace(step1, @"\s{2,}", " ");
				return step2;
			}
			return string.Empty;
		}
	}
}
