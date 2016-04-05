using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Domain.Dto;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	[RouteArea("tags")]
	public class TagController : Controller
	{
		[Route("info")]
		[Feature(Features.Tag, "Просмотр тэгов отключен")]
		public ActionResult GetCommonTagInfo()
		{
			try
			{
				var ioc = new IocHelper();

				return Json(ioc.TagService.GetInfo(), JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("get/{id}")]
		[Feature(Features.Tag, "Просмотр тэгов отключен")]
		public ActionResult GetTag(string id)
		{
			try
			{
				var currentId = int.Parse(id);
				var ioc = new IocHelper();

				return Json(ioc.TagService.GetTag(currentId), JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("save")]
		[HttpPost]
		[Feature(Features.Tag, "Сохранение тэгов отключено")]
		public ActionResult Save(Tag tag)
		{
			try
			{
				var ioc = new IocHelper();
				ioc.TagService.SaveTag(tag);
				return Json(tag.Id);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("update")]
		[HttpPost]
		[Feature(Features.Tag, "Сохранение тэгов отключено")]
		public ActionResult Update(Tag tag)
		{
			try
			{
				var ioc = new IocHelper();
				ioc.TagService.UpdateTag(tag);
				return Json(tag.Id);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("delete/{id}")]
		[HttpPost]
		[Feature(Features.Tag, "Удаление тэгов отключено")]
		public ActionResult Delete(string id)
		{
			try
			{
				var currentId = int.Parse(id);
				var ioc = new IocHelper();
				ioc.TagService.DeleteTag(currentId);
				return Json(true);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}


		[Route("stat/shedule/radial")]
		[Feature(Features.Statistics, "Просмотр статистики отключен")]
		public ActionResult GetTagRadialSheduleData()
		{
			try
			{
				var helper = new IocHelper();
				var data = helper.TagService.GetRadialSheduleData();

				return Json(data, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
			
		}
	}
}