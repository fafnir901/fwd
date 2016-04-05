using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FWD.UI.Web.Models.Feature;

namespace FWD.UI.Web.Controllers
{
	[Authorize]
	[RouteArea("features")]
	public class FeatureListController : Controller
	{
		[HttpGet]
		[Route("featureList")]
		public ActionResult GetFeatureList()
		{
			try
			{
				return Json(FeatureListModel.Instance.FeatureList, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}
	}
}