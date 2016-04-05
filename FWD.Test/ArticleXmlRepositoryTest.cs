using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects;
using FWD.BusinessObjects.Domain;
using FWD.DAL;
using FWD.DAL.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FWD.Test
{
	[TestClass]
	public class ArticleXmlRepositoryTest
	{
		private ArticleXmlRepository rep;
		private Article article;

		[TestInitialize]
		public void SetUp()
		{
			rep = new ArticleXmlRepository();
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
						Data = WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(@"C:\Users\shkorodenok\Desktop\GramarNazi.jpg")
					}
				}
			};
		}
		[TestMethod]
		public void ShouldSaveTest()
		{
			var id = rep.Save(article);
			Assert.AreNotEqual(-1, id);
			Assert.AreNotEqual(0, id);
		}

		[TestMethod]
		public void ShouldReadTest()
		{
			var id = rep.Save(article);
			var res = rep.Read(id);
			Assert.AreNotEqual(null, res);
		}

		[TestMethod]
		public void ShouldGetAllTest()
		{
			var res = rep.GetAll(null);
			Assert.AreNotEqual(null, res);
			Assert.AreNotEqual(0, res.Count());
		}

		[TestMethod]
		public void ShouldDeleteTest()
		{
			var res = rep.GetAll(null);
			var count = res.Count();
			var first = res.First();
			rep.Delete(first);
			res = rep.GetAll(null);
			Assert.AreEqual(count - 1, res.Count());
			rep.Save(first);
		}

		[TestMethod]
		public void ShouldGetByPredicateTest()
		{
			var res = rep.GetByPredicate(c => c.ArticleName.Contains("Name"), null);
			Assert.AreNotEqual(null, res);
			Assert.AreEqual(true, res.Any());

			res = rep.GetByPredicate(c => c.ArticleName == "Name", null);
			Assert.AreNotEqual(null, res);
			Assert.AreEqual(true, res.Any());

			res = rep.GetByPredicate(c => c.ArticleName == "Name", null);
			Assert.AreNotEqual(null, res);
			Assert.AreEqual(true, res.Any());
		}

		[TestMethod]
		public void ShouldSaveManyTest()
		{
			int suff = 1;
			var list = new List<Article> { article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone(), article.Clone()};
			foreach (var article1 in list)
			{
				article1.ArticleName += suff.ToString();
				article1.Link += suff.ToString();
				++suff;
			}
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
			rep.DeleteMany(list);
		}
	}
}
