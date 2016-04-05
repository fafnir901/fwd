using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FWD.BusinessObjects.Domain;
using FWD.DAL.Domain;
using FWD.DAL.Xml;
using FWD.UI.Web.Controllers;
using FWD.UI.Web.Models.Entities;
using FWD.UI.Web.Models.Entities.Dtos;
using WebRock.Utils.Monad;

namespace FWD.UI.Web.Models.Helper
{
	public class CommonHelper
	{
		private readonly static Lazy<CommonHelper> _instance = new Lazy<CommonHelper>(() => new CommonHelper());
		private CommonHelper() { }

		public static CommonHelper Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		public void Try(Action action, Action exceptionAction)
		{
			try
			{
				action();
			}
			catch (Exception)
			{
				exceptionAction();
				throw;
			}
		}

		public ImageResult DefineSizeOfImage(string size, ImageResult image)
		{
			if (string.IsNullOrEmpty(size) || image == null)
			{
				return image;
			}
			var result = image.Clone();
			var sizeResult = size.Split('x');
			if (sizeResult.Count() <= 1 || sizeResult.Count() > 2)
			{
				return image;
			}
			int width, height;
			int.TryParse(sizeResult[0], out width);
			int.TryParse(sizeResult[1], out height);
			result.ImageData = WebRock.Utils.FileUtils.ImageUtils.CreateIconSimple(result.ImageData, new Size(width, height));
			return result;
		}

		public IDto GetEntity(string type, string id)
		{
			var maybeType = type.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
			var maybeId = id.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
			int currentId;
			int.TryParse(maybeId, out currentId);
			var helper = new IocHelper();
			switch (maybeType)
			{
				default:
				case "article":
					var article = helper.ArticleService.GetArticleById(currentId);
					if (article == null)
					{
						throw new Exception("Статьи с данным идентификатором не сущесвует");
					}
					return new ArticleDto(article);
				//case "image":
				//	return IocHelper.;
				case "plan":
					var plan = helper.PlanService.GetPlanById(currentId);
					if (plan == null)
					{
						throw new Exception("Плана с данным идентификатором не сущесвует");
					}
					return new PlanDto(plan);
				case "group":
					var group = helper.GroupService.GetGroupById(currentId);
					if (group == null)
					{
						throw new Exception("Группы с данным идентификатором не сущесвует");
					}
					return new GroupDtoView(group);
			}
		}

		public string GetInfo()
		{
			double size = 0;
			if (Directory.Exists(IocHelper.GetCurrentDirectory() + BaseXml.FOLDER))
			{
				var infos = new DirectoryInfo(IocHelper.GetCurrentDirectory() + BaseXml.FOLDER);
				size = infos.EnumerateFiles()
					.Where(info => info.Extension == ".xml")
					.Aggregate<FileInfo, double>(0, (current, info) => (current + info.Length/(1024.0*1024.0)));
			}
			var articleDb = new ArticleDBRepository();
			var dbSize = articleDb.GetSize();

			var result = string.Format("Размеры данных:<br>XML: {0:0.00} MB, DB:{1}", size, dbSize);
			return result;
		}

		public List<ImageResult> TempFiles
		{
			get
			{
				if (HttpContext.Current.Session["tempFiles"] == null)
				{
					var lst = new List<ImageResult>();
					HttpContext.Current.Session["tempFiles"] = lst;
				}
				return HttpContext.Current.Session["tempFiles"] as List<ImageResult>;
			}
			set { HttpContext.Current.Session["tempFiles"] = value; }
		}

		public Article CurrentArticle
		{
			get
			{
				var current = HttpContext.Current.Session["CurrentArticleInImageController"] as Article;
				if (current == null)
				{
					current = new Article();
					HttpContext.Current.Session["CurrentArticleInImageController"] = current;
				}
				return current;
			}
			set { HttpContext.Current.Session["CurrentArticleInImageController"] = value; }
		}

		public User CurrentUser
		{
			get
			{
				var current = HttpContext.Current.Session["CurrentUser"] as User;
				if (current == null)
				{
					current = new User();
					HttpContext.Current.Session["CurrentUser"] = current;
				}
				return current;
			}
			set { HttpContext.Current.Session["CurrentUser"] = value; }
		}

		public static string GetBaseUrl(HttpRequestBase request)
		{
			var appUrl = HttpRuntime.AppDomainAppVirtualPath;

			if (!string.IsNullOrWhiteSpace(appUrl)) appUrl += "/";

			var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

			return baseUrl;
		}
	}

	public static class HelperExtensions
	{
		public static byte[] GetBytes(this string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
	}
}