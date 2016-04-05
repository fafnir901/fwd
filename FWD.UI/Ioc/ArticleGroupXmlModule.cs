using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using FWD.DAL.Domain;
using FWD.DAL.Xml;
using Ninject.Modules;

namespace FWD.UI.Ioc
{
	public class ArticleGroupXmlModule : NinjectModule
	{
		public override void Load()
		{
			this.Bind<ICommonRepository<ArticleGroup>>().To<AtricleGroupsXmlRepository>().WithConstructorArgument("path","");
		}
	}
}
