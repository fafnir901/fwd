﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FWD.UI.Web.Controllers
{
	public class StaticController : Controller
	{
		// GET: Static
		public ActionResult PageNotFound()
		{
			return View();
		}
	}
}