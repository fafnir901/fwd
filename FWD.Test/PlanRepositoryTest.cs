using System;
using System.Linq;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.DAL.Domain;
using FWD.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.Test
{
	[TestFixture]
	public class PlanRepositoryTest : BaseDbRollbackFixture
	{
		private PlanDbRepository rep;
		private CurrentPlan plan;
		[SetUp]
		public void SetUp()
		{
			rep = new PlanDbRepository();
			plan = new CurrentPlan
			{
				AddedDate = DateTime.Now,
				Name = "Упячка",
				Description = "Упячка",
				IsDone = false,
				PossibleChangeDate = null
			};
		}
		[Test]
		public void ShouldSave()
		{
			var id = rep.Save(plan);
			id.Should(Be.GreaterThan(0));
		}

		[Test]
		public void ShouldGetByPrediacte()
		{
			rep.Save(plan);

			plan.Name = "Упячка 1";
			rep.Save(plan);

			plan.Name = "Упячка 2";
			rep.Save(plan);

			plan.Name = "Дота";
			rep.Save(plan);

			plan.Name = "Дота 3";
			plan.IsDone = true;
			rep.Save(plan);

			var res1 = rep.GetByPredicate(c => c.IsDone && c.Name.Contains("Дота"), new QueryParams<IPlan>());
			var res2 = rep.GetByPredicate(c => c.Name.Contains("Дота"), new QueryParams<IPlan>());
			var res3 = rep.GetByPredicate(c => c.Name.Contains("Упячка"), new QueryParams<IPlan>());

			res1.Count().Should(Be.EqualTo(1));
			res2.Count().Should(Be.EqualTo(2));
			res3.Count().Should(Be.EqualTo(3));
		}
	}
}
