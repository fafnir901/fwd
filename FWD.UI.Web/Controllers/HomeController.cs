﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/
		public ActionResult Index()
		{
			return View();
		}
	}
}