using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using FWD.UI.Web.Models.Helper;
using Newtonsoft.Json;
using WebRock.Utils.Monad;

namespace FWD.UI.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapMvcAttributeRoutes();

			#region TopMenu
			routes.MapRoute(
			"TopMenu",
			"topMenu/{source}",
			new { controller = "TopMenu", action = "ToggleSource", source = UrlParameter.Optional });

			routes.MapRoute(
			"SaveXml",
			"saveToXml",
			new { controller = "TopMenu", action = "SaveToXml", source = UrlParameter.Optional });

			routes.MapRoute(
			"SaveDb",
			"saveToDb",
			new { controller = "TopMenu", action = "SaveToDb", source = UrlParameter.Optional });
			#endregion

			#region LeftPanel
			routes.MapRoute(
			"LeftPanel",
			"panel",
			new { controller = "LeftPanel", action = "Panel" });

			routes.MapRoute(
			"Groups",
			"groups",
			new { controller = "LeftPanel", action = "Groups" });
			#endregion

			#region Article
			//routes.MapRoute(
			//"Articles",
			//"articles/{articleId}",
			//new { controller = "Articles", action = "Articles", articleId = UrlParameter.Optional });

			routes.MapRoute(
			"ArticleSave",
			"saveArticle",
			new { controller = "Articles", action = "SaveArtilce" });

			routes.MapRoute(
			"SaveArtilceByRef",
			"saveArticleByRef",
			new { controller = "Articles", action = "SaveArtilceByRef" });

			routes.MapRoute(
			"DeleteArticle",
			"deleteArticle/{id}",
			new { controller = "Articles", action = "DeleteArticle", id = UrlParameter.Optional });

			routes.MapRoute(
			"SendEmail",
			"sendEmail",
			new { controller = "Articles", action = "SendEmail" });

			routes.MapRoute(
				"SaveNewGroup",
				"save_group",
				new { controller = "Articles", action = "SaveNewGroup" });
			#endregion

			#region Image
			routes.MapRoute(
			"Images",
			"images/{articleId}/{imageId}/{size}",
			new { controller = "Image", action = "GetImages", articleId = UrlParameter.Optional, imageId = UrlParameter.Optional, size = UrlParameter.Optional });
			#endregion

			#region Plan
			routes.MapRoute(
			"AllPlans",
			"plans",
			new { controller = "Plan", action = "GetAllPlans", skip = UrlParameter.Optional, take = UrlParameter.Optional, order = UrlParameter.Optional });

			routes.MapRoute(
			"UpdatePlan",
			"updatePlan",
			new { controller = "Plan", action = "UpdatePlan" });
			#endregion

			routes.MapRoute(
			"Entity",
			"entity/{type}/{id}",
			new { controller = "Entity", action = "GetEntity", type = UrlParameter.Optional, id = UrlParameter.Optional });

			var artilceId = new IocHelper().TransactionService.GetLastParameters().MaybeAs<Dictionary<string,string>>().Bind(c=>c["ArticleId"]).GetOrDefault("1");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Entity", action = "GetEntity", type = "type-article", id = "id-" + artilceId }
			);

			routes.MapRoute(
				"404-PageNotFound",
				"{*url}",
				new { controller = "Static", action = "PageNotFound" }
			);


		}
	}
}
