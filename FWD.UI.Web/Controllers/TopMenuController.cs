using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Xml;
using FWD.DAL.Domain;
using FWD.DAL.Xml;
using FWD.Services;
using FWD.UI.Web.Models;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class TopMenuController : Controller
	{
		[ActionName("ToggleSource")]
		[HttpPost]
		[Feature(Features.SwitchToXml, "Переключения источника данных отключено")]
		public ActionResult ToggleSource(string source)
		{
			try
			{
				var helper = new IocHelper();
				if (source == "db")
				{
					helper.ToggleToDb(true);
				}
				else
				{
					helper.ToggleToXml(true);
				}
				return Json(IocHelper.CurrentToggle);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("SaveToXml")]
		[HttpPost]
		[Feature(Features.SaveToXml, "Сохрананение в XML отключено")]
		public ActionResult SaveToXml(int? articleId)
		{
			try
			{
				if (articleId.HasValue)
				{
					var helper = new IocHelper();
					helper.ToggleToDb(true);
					var artilce = helper.ArticleService.GetArticleById(articleId.Value);
					helper.ToggleToXml(true);
					helper.ArticleService.SaveArticle(artilce);
					helper.ToggleToDb(true);
					return Json(true);
				}
				else
				{
					throw new Exception("Статьи не существует");
				}
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("SaveToDb")]
		[HttpPost]
		[Feature(Features.SaveToDb, "Сохранения в БД отключено")]
		public ActionResult SaveToDb(int? articleId)
		{
			try
			{
				if (articleId.HasValue)
				{
					var helper = new IocHelper();
					helper.ToggleToXml(true);
					var artilce = helper.ArticleService.GetArticleById(articleId.Value);
					var group =
						helper.GroupService.GetAllGroups(c => c.GroupId).FirstOrDefault(c => c.Groups.Contains(artilce.ArticleName));
					artilce.ArticleGroup = new ArticleGroup { GroupName = group == null ? "Без группы" : group.GroupName };
					helper.ToggleToDb(true);
					helper.ArticleService.SaveArticle(artilce);
					helper.ToggleToXml(true);
					return Json(true);
				}
				else
				{
					throw new Exception("Статьи не существует");
				}
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("getInfo")]
		[HttpGet]
		[Feature(Features.Information, "Получение информации отключено")]
		public ActionResult GetInfo()
		{
			try
			{
				return Json(CommonHelper.Instance.GetInfo() + "<br>Текущий источник данных:<br>" + IocHelper.CurrentToggle, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("export")]
		[HttpPost]
		[Feature(Features.Export, "Экспорт отключен")]
		public FileResult ExprortFromDb()
		{
			try
			{
				if (IocHelper.CurrentToggle != "db")
				{
					throw new Exception("Экспорт возможен только из БД");
				}
				var helper = new IocHelper();

				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						var demoFile = archive.CreateEntry("db_export.xml", CompressionLevel.Optimal);

						using (var entryStream = demoFile.Open())
						using (var streamWriter = new StreamWriter(entryStream))
						{
							streamWriter.Write(helper.ArticleService.ExportFromDb());
						}
					}
					memoryStream.Seek(0, SeekOrigin.Begin);
					return File(memoryStream.ToArray(), "application/zip", "db_export.zip");
				}
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("export/current")]
		[HttpPost]
		[Feature(Features.Export, "Экспорт отключен")]
		public FileResult ExprortCurrentFromDb(string articleId, string articleName)
		{
			try
			{
				if (IocHelper.CurrentToggle != "db")
				{
					throw new Exception("Экспорт возможен только из БД");
				}
				var helper = new IocHelper();
				int currentId = 0;
				int.TryParse(articleId, out currentId);
				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						var demoFile = archive.CreateEntry("{0}.xml".Fmt(articleName), CompressionLevel.Optimal);

						using (var entryStream = demoFile.Open())
						using (var streamWriter = new StreamWriter(entryStream))
						{
							streamWriter.Write(helper.ArticleService.ExportFromDb(currentId));
						}
					}
					memoryStream.Seek(0, SeekOrigin.Begin);
					return File(memoryStream.ToArray(), "application/zip", "{0}.zip".Fmt(articleName));
				}
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("import")]
		[HttpPost]
		[Feature(Features.Import, "Импоррт оключен")]
		public ActionResult Import(HttpPostedFileBase[] files)
		{
			try
			{
				var buffer = new byte[files.First().ContentLength];
				if (!files.First().ContentType.Contains("xml"))
				{
					throw new Exception("Не верный тип файла " + files.First().ContentType);
				}
				files.First().InputStream.Read(buffer, 0, files.First().ContentLength);

				var isDb = IocHelper.CurrentToggle == "db";
				var helper = new IocHelper();
				var unmatched = helper.ArticleService.Import(isDb, buffer);
				var stringBuilder = new StringBuilder();
				if (unmatched.Count == 0)
				{
					stringBuilder.Append("Импорт прошел успешно");
				}
				else
				{
					foreach (var res in unmatched)
					{
						stringBuilder.Append(string.Format("{0}.<br>", res));
					}
				}
				return Json(stringBuilder.ToString(), JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

	}
}