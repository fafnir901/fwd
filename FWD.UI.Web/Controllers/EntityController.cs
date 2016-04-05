using System;
using System.Web.Mvc;
using FWD.UI.Web.Models.Entities.Dtos;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	public class EntityController : Controller
	{
		// GET: Entity
		//[ActionName("GetEntity")]
		[Authorize]
		[Feature(Features.GetAtricleById, "Просмотр статей отключен")]
		public ActionResult GetEntity(string type, string id)
		{
			try
			{
				id = id ?? "id-1";
				id = id == "null" ? "id-1" : id;
				var entity = CommonHelper.Instance.GetEntity(type, id);
				return View(entity);
				//return Json(entity, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				var exDto = new ExceptonDto
				{
					Exception = e
				};
				return View(exDto);
			}
		}
	}
}