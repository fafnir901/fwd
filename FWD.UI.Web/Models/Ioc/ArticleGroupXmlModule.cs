using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using FWD.DAL.Xml;
using FWD.UI.Web.Models.Helper;
using Ninject.Modules;

namespace FWD.UI.Web.Models.Ioc
{
	public class ArticleGroupXmlModule : NinjectModule
	{
		public override void Load()
		{
			this.Bind<ICommonRepository<ArticleGroup>>().To<AtricleGroupsXmlRepository>().WithConstructorArgument("path", IocHelper.GetCurrentDirectory()).WithConstructorArgument("isForce", IocHelper.IsForcePath());
		}
	}
}