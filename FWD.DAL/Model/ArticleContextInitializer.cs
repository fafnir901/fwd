using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Xml;

namespace FWD.DAL.Model
{
	public class ArticleContextInitializer : MigrateDatabaseToLatestVersion<ArticleContext, ArticleMigrationConfigurator>//DropCreateDatabaseIfModelChanges<ArticleContext>//CreateDatabaseIfNotExists<ArticleContext>
	{
		//protected override void Seed(ArticleContext context)
		//{
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "Без группы" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = ".NET" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "ASP.NET" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "SQL" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "JavaScript" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "HTML" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "Администрирование" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "Наука" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "Паттерны" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "Графика" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "Web" });
		//	context.Groups.Add(new Entities.ArticleGroup { GroupName = "Программирование" });

		//	base.Seed(context);
		//}
	}
}
