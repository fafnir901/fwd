using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using FWD.BusinessObjects;
using FWD.BusinessObjects.Domain;
using FWD.DAL;
using FWD.DAL.Domain;
using FWD.DAL.Helpers;
using FWD.Services;
using FWD.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using WebRock.Utils.UtilsEntities;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace FWD.Test
{
	[TestFixture]
	public class ArticleRepositoryTest : BaseDbRollbackFixture
	{
		private ArticleDBRepository rep;
		private Article article;

		[SetUp]
		public void SetUp()
		{
			rep = new ArticleDBRepository();
			article = new Article
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
		}

		[Test]
		public void SaveTest()
		{
			var id = rep.Save(article);

			id.Should(Be.Not.EqualTo(0));
			id.Should(Be.Not.EqualTo(-1));

		}

		[Test]
		public void SaveManyTest()
		{
			var list = new List<Article> { article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone(), };
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			list = rep.SaveMany(list).ToList();
			stopWatch.Stop();
			Console.WriteLine(stopWatch.ElapsedMilliseconds);
			foreach (var i in list)
			{
				Assert.AreNotEqual(0, i.ArticleId);
				Assert.AreNotEqual(-1, i.ArticleId);
			}
		}

		[Test]
		public void ShouldNotDeleteNotExistingArticleTest()
		{
			var article = new Article
			{
				ArticleName = "Name",
				InitialText = "Descr",
				Link = "email@email.eamil"
			};
			rep.Delete(article);
		}

		[Test]
		public void ShouldReadAllTest()
		{
			var list = new List<Article> { article, article, article, article, article, article, article, article, article, article, article };
			list = rep.SaveMany(list).ToList();

			var list2 = rep.GetAll(null);
			Assert.AreEqual(10, list2.Count());

			var list3 = rep.GetAll(new QueryParams<Article>(10, 1, null));
			Assert.AreEqual(1, list3.Count());

		}

		[Test]
		public void ShouldGetByPredicateTest()
		{
			var list = new List<Article> { article.Clone(), article.Clone(), article.Clone(), article.Clone(), 
				article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone(), 
				article.Clone() };
			list[0].ArticleName = "Her";
			list[0].InitialText = "SSS";
			list[1].ArticleName = "His";
			list[2].ArticleName = "Here";
			list[2].InitialText = "MMM";
			list[3].ArticleName = "How";
			list[4].ArticleName = "Fight";
			list = rep.SaveMany(list).ToList();
			var list2 = rep.GetByPredicate(c => c.ArticleName.Contains("Her") && c.InitialText.Contains("SSS"), null);
			Assert.AreEqual(1, list2.Count());

			int id = list[0].ArticleId;
			var list3 = rep.GetByPredicate(c => c.ArticleName.Equals("Her") && c.ArticleId.Equals(id), null);
			Assert.AreEqual(1, list3.Count());
		}

		[Test]
		public void ShouldGetBySearchString()
		{
			string searchString = "C#";
			var res = rep.GetByPredicate(c => c.ArticleName.ToUpper().Contains(searchString.ToUpper())
				|| c.AuthorName.ToUpper().Contains(searchString.ToUpper())
				|| c.InitialText.ToUpper().Contains(searchString.ToUpper())
				|| c.Tags.FirstOrDefault(x => x.Name.ToUpper().Contains(searchString.ToUpper())) != null, null);


			res.Count().Should(Be.GreaterThan(0));
		}

		[Test]
		public void ShouldUpdateTest()
		{
			rep.Save(article);
			article.ArticleName = "POpd";
			rep.Update(article);
			var res = rep.Read(article.ArticleId);
			Assert.AreEqual(article.ArticleName, res.ArticleName);

		}

		[Test]
		public void ShouldExport()
		{
			var str = rep.Export();
			Assert.AreEqual(true, str.Length > 0);
		}

		[Test]
		public void ShoulImportData()
		{

			var groupRep = new GroupDbRepository();

			var group = new ArticleGroup
			{
				GroupName = "Федя"
			};

			var id = groupRep.Save(group);


			var str = "<XmlArticle><Article Link=\"http://ontime.spanishpoint.ie/\" AuthorName=\"Хабр\" ArticleName=\"Федя\" ArticleId=\"1\" GroupId=\"{0}\" InitialText=\"Федя\"></Article></XmlArticle>".Fmt(id);
			rep.Import(true, str.GetBytes());

			var actual = rep.GetByPredicate(c => c.ArticleName == "Федя", new QueryParams<Article>(0, 10, x => x.ArticleId)).FirstOrDefault();


			actual.Should(Be.Not.Null);
			actual.ArticleGroup.Should(Be.Not.Null);
		}

		[Test]
		public void ShouldSearch()
		{
			var artService = new ArticleService(rep, "http://localhost/fwd/entity/type-article/id-{0}");
			var res = artService.GetBySearchString("Хабр", c => c.ArticleName, 0, 1, "http://localhost/fwd/entity/type-article/id-{0}");
			res.Count().Should(Be.GreaterThan(0));
		}

	}


}
