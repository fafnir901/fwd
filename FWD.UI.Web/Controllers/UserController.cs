using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models;
using FWD.UI.Web.Models.Entities;
using FWD.UI.Web.Models.Entities.Dtos;
using FWD.UI.Web.Models.Helper;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.UI.Web.Controllers
{
	[RouteArea("User")]
	public class UserController : Controller
	{
		[HttpGet]
		public ActionResult Login()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Registration()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Registration(RegistrationModel model)
		{
			var errorList = new List<string>();

			if (model.Password != model.ConfirmPassword)
			{
				errorList.Add("Пароли не совпадают");
			}
			var helper = new IocHelper();
			var existingUser = helper.UserService.GetUserByParams(model.FirstName, model.LastName, model.Email, model.Login);
			if (existingUser.Any())
			{
				errorList.Add("Пользователь с похожими данными уже существует");
			}

			if (errorList.Count > 0)
			{
				CommonHelper.Instance.TempFiles = null;
				foreach (var item in errorList)
				{
					ModelState.AddModelError("", item);
				}
				return View(model);
			}
			var user = new User
			{
				Credential = new Credential
				{
					Login = model.Login,
					Password = model.Password
				},
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				Avatar = CommonHelper.Instance.TempFiles.Count > 0 ? CommonHelper.Instance.TempFiles.First().ImageData : null

			};

			CommonHelper.Instance.TempFiles = null;
			if (!ModelState.IsValid)
			{

				CommonHelper.Instance.TempFiles = null;
				return View(model);
			}
			else
			{
				helper.UserService.AddUser(user);
			}
			return View("Login", user);
		}

		[HttpPost]
		public ActionResult Login(User user)
		{
			var helper = new IocHelper();
			var currentUser = helper.UserService.GetUserByCredential(user.Credential.Password, user.Credential.Login);
			if (currentUser == null)
			{
				ModelState.AddModelError("", "Пользователь не найден");
				return View(user);
			}
			else
			{
				FormsAuthentication.SetAuthCookie(currentUser.Credential.Login + "[!!!]" + currentUser.UserRole.RoleName(), false);
				CommonHelper.Instance.CurrentUser = currentUser;
				var artilceId = new IocHelper().TransactionService.GetLastParameters().MaybeAs<Dictionary<string, string>>().Bind(c => c["ArticleId"]).GetOrDefault("1");
				return new RedirectResult("../entity/type-article/id-" + artilceId);
			}
		}

		[Route("avatar/{id}/{size}")]
		public async Task<FileContentResult> GetAvatar(int? id = null, string size = null)
		{
			var helper = new IocHelper();
			var currentId = id ?? 0;
			var user = helper.UserService.GetUser(currentId);
			var maybeSize = size.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
			var currentAvatar = user == null
				? WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(
					@"D:\examples\Examples\ForWebDevelopmentProject\FWD.UI.Web\Content\Images\default_avatar.png")
				: user.Avatar;
			var result = new ImageResult
			{
				ImageData = currentAvatar
			};
			var reworkedImage = CommonHelper.Instance.DefineSizeOfImage(maybeSize, result);
			return await Task.Factory.StartNew(() => File(reworkedImage.MaybeAs<ImageResult>().Bind(c => c.ImageData).GetOrDefault(null), "image/png"));
		}

		[Route("avatar/{size}")]
		public async Task<FileContentResult> GetAvatar(string size = null)
		{
			var maybeSize = size.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
			var result = new ImageResult
			{
				ImageData = CommonHelper.Instance.CurrentUser.Avatar ?? WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(@"D:\examples\Examples\ForWebDevelopmentProject\FWD.UI.Web\Content\Images\default_avatar.png")
			};
			var reworkedImage = CommonHelper.Instance.DefineSizeOfImage(maybeSize, result);
			return await Task.Factory.StartNew(() => File(reworkedImage.MaybeAs<ImageResult>().Bind(c => c.ImageData).GetOrDefault(null), "image/gif"));
		}

		[HttpGet]
		public ActionResult Logout()
		{
			CommonHelper.Instance.CurrentUser = new User();
			FormsAuthentication.SignOut();

			Session.Clear();
			Session.Abandon();
			return View("Login");
		}

		[HttpGet]
		[Authorize]
		[Route("editor")]
		public ActionResult GetUserEditor()
		{
			try
			{
				var userEditor = new UserEditorDto(CommonHelper.Instance.CurrentUser);

				return Json(userEditor, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[HttpGet]
		[Route("currentUser")]
		public ActionResult GetCurrentUserData()
		{
			try
			{
				var current = CommonHelper.Instance.CurrentUser;

				return Json(new { UserId = current.UserId, UserRole = current.UserRole.RoleName(), UserName = current.FirstName }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[HttpPost]
		[Authorize]
		[Route("saveAvatar")]
		public ActionResult SaveAvatar()
		{
			try
			{
				var file = CommonHelper.Instance.TempFiles.LastOrDefault();
				if (file != null)
				{
					var user = CommonHelper.Instance.CurrentUser;
					user.Avatar = file.ImageData;

					var helper = new IocHelper();
					helper.UserService.UpdateUser(user);
					CommonHelper.Instance.TempFiles = new List<ImageResult>();
					return Json(true, JsonRequestBehavior.AllowGet);

				}
				else
				{
					throw new Exception("Аватар не был изменен");
				}
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[HttpPost]
		[Authorize]
		[Route("updateUserData")]
		public ActionResult UpdateuserData(User user)
		{
			try
			{
				var helper = new IocHelper();

				var current = CommonHelper.Instance.CurrentUser;
				helper.UserService.ValidateUserData(current, user);

				current.Email = user.Email;
				current.FirstName = user.FirstName;
				current.LastName = user.LastName;

				helper.UserService.UpdateUser(current);

				return Json(true, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}
	}
}