using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWD.UI.Web.Controllers
{
	
	public class PseudonimController:Controller
	{
		[Route("pseudonim")]
		[HttpPost]
		public ActionResult SetPseudonim(string pseudonim)
		{
			System.Web.HttpContext.Current.Session["pseudonim"] = pseudonim;
			return Json(pseudonim,JsonRequestBehavior.AllowGet);
		}
	}
}