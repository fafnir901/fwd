using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using FWD.DAL;
using FWD.DAL.Xml;
using Ninject.Modules;

namespace FWD.UI.Ioc
{
	public class ArticleXmlModule : NinjectModule
	{
		public override void Load()
		{
			this.Bind<ICommonRepository<Article>>().To<ArticleXmlRepository>().WithConstructorArgument("path","");
		}
	}
}
