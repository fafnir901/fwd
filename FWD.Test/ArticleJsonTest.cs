using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.DAL.JSON;
using NUnit.Framework;

namespace FWD.Test
{
	[TestFixture]
	public class ArticleJsonTest
	{
		[Test]
		public void ShouldSave()
		{
			var rep = new ArticleJsonRepository();
			var random = new Random();
			var article = new Article
			{
				ArticleName = "Name",
				InitialText = "Descr",
				Link = "email@email.eamil" + random.Next(0,10000000),
				Images = new List<Image>
				{
					new Image
					{
						Name = "fedor",
						Data = WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(@"d:\CURRENT_FWD\Output\Content\Images\2.png")
					}
				}
			};

			rep.Save(article);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void ShouldRiseExceptionWhenSaveExistinLink()
		{
			var rep = new ArticleJsonRepository();
			var article = new Article
			{
				ArticleName = "Name",
				InitialText = "Descr",
				Link = "email@email.eamil",
				Images = new List<Image>
				{
					new Image
					{
						Name = "fedor",
						Data = WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(@"d:\CURRENT_FWD\Output\Content\Images\2.png")
					}
				}
			};

			rep.Save(article);
		}
	}
}
