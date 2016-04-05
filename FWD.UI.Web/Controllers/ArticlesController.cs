using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FWD.Services;
using FWD.UI.Web.Models.Entities;
using FWD.UI.Web.Models.Entities.Dtos;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;
using WebRock.Utils.Monad;
using Article = FWD.BusinessObjects.Domain.Article;
using ArticleGroup = FWD.BusinessObjects.Domain.ArticleGroup;

namespace FWD.UI.Web.Controllers
{
	[System.Web.Mvc.Authorize]
	[RouteArea("articles")]
	public class ArticlesController : Controller
	{
		private static Regex _regexForTitle = new Regex(@"<title>([a-zA-Zа-яА-Я-\s]+)<\/title>", RegexOptions.Compiled);
		private static Regex _regexForHtmlTags = new Regex(@"<.*?>", RegexOptions.Compiled);


		private List<StatViewModel> listTranViewsModels
		{
			get
			{
				var lst = Session["listTranViewsModels"] as List<StatViewModel> ?? new List<StatViewModel>();
				return lst;
			}
			set { Session["listTranViewsModels"] = value; }
		}

		[ActionName("Articles")]
		[Route("{articleId}")]
		[Feature(Features.GetAtricleById, "Просмотр статей отключен")]
		public ActionResult GetArticleById(string articleID)
		{
			try
			{
				var maybeId = articleID.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
				int currentId;
				int.TryParse(maybeId, out currentId);
				var helper = new IocHelper();
				CommonHelper.Instance.CurrentArticle = helper.ArticleService.GetArticleById(currentId);
				var dto = new ArticleDto(CommonHelper.Instance.CurrentArticle);
				var groups = helper.GroupService.GetAllGroups(c => c.GroupName, 0, 999).ToList();
				if (IocHelper.CurrentToggle == "xml")
				{
					dto.Group = groups.FirstOrDefault(c => c.Groups.FirstOrDefault(x => x == dto.ArticleName) != null)
						.MaybeAs<ArticleGroup>()
						.Bind(c => c.GroupName)
						.GetOrDefault(null);
				}

				return Json(dto, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("updateArticleRate")]
		[HttpPost]
		public ActionResult UpdateArticleRate(int? articleId, int? rate)
		{
			try
			{
				var helper = new IocHelper();
				var art = helper.ArticleService.GetArticleById(articleId ?? 0);
				art.Rate = rate ?? 0;
				helper.ArticleService.UpdateArticle(art);
				return Json(true, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("stat/{articleId}")]
		[Feature(Features.Statistics, "Просмотр статистики отключен")]
		public ActionResult GetStat(int? articleId)
		{
			try
			{
				var maybeId = articleId.MaybeAs<int>().Bind(c => c).GetOrDefault(-1);
				var helper = new IocHelper();
				var res = helper.ArticleService.GetArticleById(maybeId);
				if (res != null)
				{
					var countOfLetters = helper.ArticleHelperService.GetCountOfLetters(res);
					var countOfWords = helper.ArticleHelperService.GetCountOfWords(res);
					var countOfSentenses = helper.ArticleHelperService.GetCountOfSentences(res);
					var result = new { CountOfLetters = countOfLetters, CountOfWords = countOfWords, countOfSentenses = countOfSentenses, ArticleName = res.ArticleName };
					return Json(result, JsonRequestBehavior.AllowGet);
				}
				throw new Exception("Статьи с данным идентификатором не существует");

			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("stat/fullStat")]
		[Feature(Features.Statistics, "Просмотр статистики отключен")]
		public ActionResult GetFullStat()
		{
			try
			{
				var helper = new IocHelper();
				if (listTranViewsModels.Count == 0)
				{
					listTranViewsModels = helper.ArticleService.GetAllArticles(c => c.ArticleId).Select(c => new StatViewModel { Name = c.ArticleName, LetterCount = helper.ArticleHelperService.GetCountOfLetters(c) }).ToList();
				}

				var totalEntityes = (double)helper.TransactionService.GetTrancCountOfArticles();
				var counts = helper.TransactionService.GetTrancCount();

				var readable = (counts.Item1 / totalEntityes) * 100;
				var updatable = (counts.Item3 / totalEntityes) * 100;
				var writable = (counts.Item2 / totalEntityes) * 100;
				var deletable = (counts.Item4 / totalEntityes) * 100;

				var wrapper = new StatViewModelWrapper
				{
					TranViewModels = listTranViewsModels,
					TotalCount = listTranViewsModels.Sum(c => c.LetterCount),
					ReadablePercent = readable,
					UpdatablePercent = updatable,
					WritablePercent = writable,
					DeletablePercent = deletable,

					WritableCount = counts.Item2,
					DeletableCount = counts.Item4,
					ReadableCount = counts.Item1,
					UpdatableCount = counts.Item3
				};
				return Json(wrapper, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("stat/shedule")]
		[Feature(Features.Statistics, "Просмотр статистики отключен")]
		public ActionResult GetSheduleData()
		{
			try
			{
				var helper = new IocHelper();
				var data = helper.ArticleService.GetSheduleData();

				return Json(data, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("updateArticle")]
		[HttpPost]
		[System.Web.Mvc.Authorize(Roles = "Admin")]
		[Feature(Features.Edit, "Редактирование отключено")]
		public ActionResult UpdateArticle(ArticleDto dto)
		{
			try
			{
				var article = dto.Convert();
				GroupHelper.Instance.InitGroups(dto);
				var helper = new IocHelper();
				helper.ArticleService.UpdateArticle(article);
				return Json(true);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("SaveArtilce")]
		[HttpPost]
		[Feature(Features.Add, "Добавление отключено")]
		public ActionResult SaveArticle(ArticleDto dto)
		{
			try
			{
				var article = dto.Convert();
				GroupHelper.Instance.InitGroups(dto);
				var helper = new IocHelper();
				helper.ArticleService.SaveArticle(article);
				return Json(true);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("SaveArtilceByRef")]
		[HttpPost]
		public ActionResult SaveArticleByRef(string link)
		{
			try
			{
				var dto = new ArticleDto();
				var client = new WebClient();
				client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

				var streamData = client.OpenRead(link);
				var reader = new StreamReader(streamData);
				string html = reader.ReadToEnd();
				var res = _regexForTitle.Match(html);
				dto.ArticleName = res.Groups[1].Value;
				var tags = html.StripTagsCharArray();//CommonHelper.Instance.StripTagsCharArray(html);
				var content = tags.Replace(dto.ArticleName, string.Empty);
				dto.Content = content;
				//var article = dto.Convert();
				//IocHelper.ArticleService.SaveArticle(article);
				return Json(dto);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("SaveNewGroup")]
		[HttpPost]
		[System.Web.Mvc.Authorize(Roles = "Admin")]
		public ActionResult SaveNewGroup(string group)
		{
			try
			{
				var helper = new IocHelper();
				var res = IocHelper.CurrentToggle == "xml" 
					? helper.GroupService.GetGroupsByParams(c => c.GroupName == group, c => c.GroupId, 0, 1).FirstOrDefault()
					: helper.GroupService.GetAllGroups(c => c.GroupId).FirstOrDefault(c => c.GroupName == group);

				if (res != null)
				{
					throw new Exception(string.Format("Группа \"{0}\" уже существует", group));
				}
				var groupArticle = new ArticleGroup
				{
					GroupName = group,
					Groups = new List<string>()
				};
				helper.GroupService.SaveGroup(groupArticle);
				return Json(true);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("DeleteArticle")]
		[HttpPost]
		[System.Web.Mvc.Authorize(Roles = "Admin")]
		[Feature(Features.Remove, "Удаление статей отключено")]
		public ActionResult DeleteArticelById(string id)
		{
			try
			{
				int ids;
				int.TryParse(id, out ids);
				var article = new Article
				{
					ArticleId = ids
				};
				var helper = new IocHelper();
				var current = article.Clone();
				helper.ArticleService.DeleteArticle(article);
				return Json(current);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[ActionName("SendEmail")]
		[HttpPost]
		[Feature(Features.SwitchEmail, "Отправка Email оключена")]
		public ActionResult SendEmail()
		{
			try
			{
				if (!FeatureListModel.Instance.CheckFeatureForEnabled(Features.SwitchEmail))
					throw new Exception("Отправка Email оключена");
				var helper = new IocHelper();
				//helper.ToggleToXml(true);
				var listOfLinks =
					helper.ArticleService.GetAllArticles(c => c.ArticleId).OrderBy(c => c.ArticleName)
						.Select((c, index) => string.Format("{0}) Название: <span style='font-weight:900;color:black'>{1}</span>(<a href='{2}'>{2}</a>)", index + 1, c.ArticleName, c.Link));
				var str = string.Join("<br/>", listOfLinks);
				EmailService.Instance.SendMail(CommonHelper.Instance.CurrentUser.Email, "Список ссылок на статьи", str);
				//helper.ToggleToDb(true);
				return Json(CommonHelper.Instance.CurrentUser.Email);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("mail")]
		[Feature(Features.SwitchEmail, "Отправка Email оключена")]
		[HttpPost]
		public ActionResult SendEmail(string id, string mail, string subject)
		{
			try
			{
				mail = string.IsNullOrEmpty(mail) ? CommonHelper.Instance.CurrentUser.Email : mail;
				subject = string.IsNullOrEmpty(subject) ? "Прочитай это, друг" : subject;
				var helper = new IocHelper();
				int currentId;
				int.TryParse(id, out currentId);
				var article = helper.ArticleService.GetArticleById(currentId);
				EmailService.Instance.SendMail(mail, subject, article.HtmlText,"Статья: ");
				return Json(mail);
				//return Json(data);
			}
			catch (Exception ex)
			{
				throw new HttpException(500, ex.Message);
			}
		}
	}
}