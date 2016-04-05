using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using FWD.DAL.Domain;
using Ninject.Modules;

namespace FWD.UI.Web.Models.Ioc
{
	public class TagDbModule : NinjectModule
	{
		public override void Load()
		{
			this.Bind<ICommonRepository<ITag>>().To<TagDbRepository>();
		}
	}

}