using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models.Entities;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	[Authorize]
	public class LeftPanelController : Controller
	{
		[ActionName("Panel")]
		[Feature(Features.ShowListOfAtricles, "Переключения на просмотр группы отключено")]
		public ActionResult GetPanel()
		{
			try
			{
				var helper = new IocHelper();
				var res = helper.ArticleService.GetAllArticles(c => c.ArticleName);
				var list = new List<ForPanel>();
				Func<IEnumerable<Tag>, string> define = (items) =>
				{
					return
						"[{0}]".Fmt(string.Join(",", items
								.OrderByDescending(c => c.Priority)
								.Select(c => string.Format("{0}{5}Name{5}:{5}{1}{5},{5}TagColor{5}:{5}{2}{5},{5}Priority{5}:{5}{3}{5}{4}", "{", c.Name, c.TagColor, c.Priority, "}","'"))));

				};
				if (res != null)
				{
					list = res.Select(article => new ForPanel
					{
						Id = article.ArticleId,
						Name = article.ArticleName,
						Rate = article.Rate,
						CountOfImages = article.Images.Count,
						CreationDate = article.CreationDate,
						Tags = define(article.Tags)
					}).OrderBy(c => c.Name).ToList();
				}
				return Json(list, JsonRequestBehavior.AllowGet);
			}

			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("Groups")]
		[Feature(Features.SwitchAnotherView, "Переключения на просмотр группы отключено")]
		public ActionResult GetGroups()
		{
			try
			{
				var helper = new IocHelper();
				var res = helper.ArticleService.GetAllArticles(c => c.ArticleName);
				var groups = helper.GroupService.GetAllGroups(c => c.GroupId);
				var lst = new List<ForGroupPanel>();
				if (IocHelper.CurrentToggle == "xml")
				{
					var groupView = GroupHelper.Instance.GetXmlGroups(groups, res ?? new List<Article>());
					lst = groupView.GroupViewItems.Select(c => new ForGroupPanel(c)).ToList();
				}
				else
				{
					var groupView = GroupHelper.Instance.GetDbGroups(groups);
					lst = groupView.GroupViewItems.Select(c => new ForGroupPanel(c)).OrderBy(c => c.Name).ToList();
				}

				return Json(lst, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}
	}
}