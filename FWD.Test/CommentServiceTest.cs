using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.DAL.Domain;
using FWD.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FWD.Test
{
	[TestClass]
	public class CommentServiceTest
	{
		private CommentDbRepository _repository;
		[TestInitialize]
		public void SetUp()
		{
			_repository = new CommentDbRepository();
		}
		[TestMethod]
		public void GetByGroupTest()
		{
			var comment = new Comment
			{
				AddedDate = DateTime.Now,
				CommentId = Guid.NewGuid(),
				GroupName = "eternity",
				CommentText = "Hi all",
				UserName = "Fedia"
			};
			_repository.Save(comment, true);

			var res = CommentService.Instance(_repository).GetByGroup("eternity");
			Assert.IsNotNull(res);
			Assert.AreEqual(true,res.Any());

			_repository.Delete(comment);
		}
	}
}
