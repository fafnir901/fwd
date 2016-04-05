using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	[RouteArea("search")]
	[Authorize]
	public class SearchController : Controller
	{
		[Route("getResult")]
		[HttpPost]
		[Feature(Features.Search, "Поиск отключен")]
		public ActionResult GetResults(string searchString)
		{
			try
			{
				var initailAddress = CommonHelper.GetBaseUrl(Request) + "entity/type-article/id-{0}";
				var helper = new IocHelper();
				var res =
					helper.ArticleService.GetBySearchStringWithTags(searchString, c => c.ArticleName, fmt: initailAddress).ToList();
				if (res == null || res.Count == 0)
					throw new Exception("Статей не найдено");
				return Json(res, JsonRequestBehavior.AllowGet);

			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("byTags/getResult")]
		[HttpPost]
		[Feature(Features.Search, "Поиск отключен")]
		public ActionResult GetResultsByTags(IEnumerable<int> ids)
		{
			try
			{
				var initailAddress = CommonHelper.GetBaseUrl(Request) + "entity/type-article/id-{0}";
				var helper = new IocHelper();
				var res =
					helper.ArticleService.GetByTagIDsWithTags(ids, c => c.ArticleName, fmt: initailAddress).ToList();
				if (res == null || res.Count == 0)
					throw new Exception("Статей не найдено");
				return Json(res, JsonRequestBehavior.AllowGet);

			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}
	}

	public class TagSearchResult
	{
		public int[] Ids { get; set; }
	}
}