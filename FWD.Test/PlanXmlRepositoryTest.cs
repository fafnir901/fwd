using System;
using System.Collections.Generic;
using System.Linq;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.DAL.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.Utils.UtilsEntities;

namespace FWD.Test
{
	[TestClass]
	public class PlanXmlRepositoryTest
	{

		private PlanXmlRepository rep;
		private Plan _plan;

		[TestInitialize]
		public void SetUp()
		{
			rep = new PlanXmlRepository();
			_plan = new Plan
			{
				Name = "Create plan",
				AddedDate = DateTime.Now,
				Description = "Create plan",
				PossibleChangeDate = null,
				IsDone = false
			};
		}
		[TestMethod]
		public void SavePlan()
		{
			var id = rep.Save(_plan);
			Assert.AreNotEqual(0,id);
			rep.Delete(_plan);
		}

		[TestMethod]
		public void SaveManyPlans()
		{
			var lst = new List<IPlan>()
			{
				_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan
			};
			lst = rep.SaveMany(lst).ToList();
			var res = rep.GetAll(null);
			Assert.AreEqual(res.Count(),lst.Count);
			rep.DeleteMany(lst);
		}
		[TestMethod]
		public void ReadPlan()
		{
			var id = rep.Save(_plan);
			var current = rep.Read(id);
			Assert.AreEqual(current.Description, _plan.Description);
			Assert.AreEqual(current.AddedDate, _plan.AddedDate);
			Assert.AreEqual(current.IsDone, _plan.IsDone);
			Assert.AreEqual(current.Id, _plan.Id); 
			Assert.AreEqual(current.Name, _plan.Name);
			Assert.AreEqual(current.PossibleChangeDate, _plan.PossibleChangeDate);
			rep.Delete(_plan);
		}

		[TestMethod]
		public void DeletePlan()
		{
			rep.Save(_plan);
			rep.Delete(_plan);
			var res = rep.Read(_plan.Id);
			Assert.AreEqual(null,res);
		}

		[TestMethod]
		public void DeleteManyPlans()
		{
			var lst = new List<IPlan>()
			{
				_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan
			};
			lst = rep.SaveMany(lst).ToList();
			rep.DeleteMany(lst);
			var res = rep.GetAll(null);
			Assert.AreEqual(null, res);
		}

		[TestMethod]
		public void UpdatePlan()
		{
			rep.Save(_plan);
			_plan.Description = "Fedia";
			rep.Update(_plan);
			var current = rep.Read(_plan.Id);
			rep.Delete(_plan);
			Assert.AreEqual(_plan.Description, current.Description);
			Assert.AreNotEqual(default(DateTime),current.PossibleChangeDate);
		}

		[TestMethod]
		public void GetAllPlans()
		{
			var lst = new List<IPlan>()
			{
				_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan,_plan
			};
			lst = rep.SaveMany(lst).ToList();
			var res = rep.GetAll(null);
			rep.DeleteMany(lst);
			Assert.AreEqual(res.Count(), lst.Count);
			Assert.AreEqual(true, res.Any());
		}
		[TestMethod]
		public void GetByPredicate()
		{
			var lst = new List<IPlan>()
			{
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone(),
				_plan.Clone()
			};

			lst[0].IsDone = true;
			lst = rep.SaveMany(lst.Select(c=>c as Plan)).ToList().Select(c=>c as IPlan).ToList();

			var res = rep.GetByPredicate(c => c.IsDone, null).FirstOrDefault();

			rep.DeleteMany(lst.Select(c=>c as Plan));

			Assert.AreEqual(res.Id,lst.First().Id);
		}
	}
}
