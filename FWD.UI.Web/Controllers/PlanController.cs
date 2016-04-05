using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	[RouteArea("plans")]
	[Authorize(Roles = "Admin")]
	public class PlanController : Controller
	{
		[ActionName("GetAllPlans")]
		[Feature(Features.Plan, "Просмотр плана отключен")]
		public ActionResult GetAllPlans(string skip,string take,string order)
		{
			try
			{
				int pSkip, pTake;
				int.TryParse(skip, out pSkip);
				int.TryParse(take, out pTake);
				pTake = pTake > 0 ? pTake : 999;
				var helper = new IocHelper();
				var res = helper.PlanService.GetAllPlans(c => c.Name, pSkip, pTake);
				var totalCount = helper.PlanService.TotalCount;
				var plan = new {Plans = res, TotalCount = totalCount};
				return Json(plan, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("UpdatePlan")]
		[HttpPost]
		public ActionResult UpdatePlan(CurrentPlan plan)
		{
			try
			{
				var helper = new IocHelper();
				helper.PlanService.UpdatePlan(plan);
				return Json(plan, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}
		[Route("savePlan")]
		[HttpPost]
		public ActionResult SavePlan(CurrentPlan plan)
		{
			try
			{
				plan.AddedDate = DateTime.Now;
				plan.IsDone = false;
				var helper = new IocHelper();
				helper.PlanService.SavePlan(plan);
				return Json(plan, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("deletePlan")]
		[HttpPost]
		public ActionResult DeletePlan(int id)
		{
			try
			{
				var plan = new CurrentPlan{Id = id};
				var helper = new IocHelper();
				helper.PlanService.DeletePlan(plan);
				return Json(true, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}
	}
}