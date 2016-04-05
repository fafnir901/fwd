using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FWD.BusinessObjects.Domain;
using FWD.DAL.Model;
using FWD.Services;
using FWD.UI.Web.Controllers;
using FWD.UI.Web.Models;
using FWD.UI.Web.Models.Entities;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			Database.SetInitializer<ArticleContext>(new ArticleContextInitializer());

		}

		protected void Session_Start(Object sender, EventArgs e)
		{
			FormsAuthentication.SignOut();
		}

		protected void Session_End(Object sender, EventArgs e)
		{
			FeatureListModel.Instance.ReloadFeatureList();
		}
		protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
		{
			if (FormsAuthentication.CookiesSupported == true)
			{
				if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
				{
					try
					{
						//let us take out the username now                
						var res = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
						var userNameAndRole = res.Split("[!!!]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						var username = userNameAndRole[0];
						string roles = userNameAndRole[1];

						//string userName = CommonHelper.Instance.CurrentUser.Credential.Login;
						//roles = CommonHelper.Instance.CurrentUser.UserRole.RoleName();

						//let us extract the roles from our own custom cookie


						//Let us set the Pricipal with our user specific details
						HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
						  new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
					}
					catch (Exception)
					{
						Session.Clear();
						Session.Abandon();
						Response.Redirect("~/User/Login");
					}
				}
			}
		}

		protected void Application_EndRequest()
		{
			if (Context.Response.StatusCode == 404)
			{
				Response.Clear();

				var rd = new RouteData();
				rd.Values["controller"] = "Static";
				rd.Values["action"] = "PageNotFound";

				IController c = new StaticController();
				c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
			}

			var context = new HttpContextWrapper(Context);
			if (Context.Response.StatusCode == 401 || (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest()) || Context.Cache["ResponceStatus"] != null)
			{
				Response.Clear();
				Context.Response.StatusCode = 308;
				Context.Cache.Add("ResponceStatus", 308, null,
					DateTime.MaxValue, new TimeSpan(0, 0, 1),CacheItemPriority.Default, null);
				var rd = new RouteData();
				rd.Values["controller"] = "User";
				rd.Values["action"] = "Login";

				IController c = new UserController();
				Context.Request.RequestContext.RouteData = rd; 
				c.Execute(Context.Request.RequestContext);
			}
		}
	}
}
