using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Absrtact;


namespace FWD.DAL.Model
{
	public class ArticleMigrationConfigurator : DbMigrationsConfiguration<ArticleContext>
	{
		public ArticleMigrationConfigurator()
		{
			AutomaticMigrationsEnabled = true;
			base.AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(ArticleContext context)
		{
			if (context.Groups.FirstOrDefault(c => c.GroupName == "Без группы") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "Без группы" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == ".NET") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = ".NET" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "ASP.NET") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "ASP.NET" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "SQL") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "SQL" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "JavaScript") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "JavaScript" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "HTML") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "HTML" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "Администрирование") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "Администрирование" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "Наука") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "Наука" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "Паттерны") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "Паттерны" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "Графика") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "Графика" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "Web") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "Web" });
			if (context.Groups.FirstOrDefault(c => c.GroupName == "Программирование") == null)
				context.Groups.Add(new Entities.ArticleGroup { GroupName = "Программирование" });


			if (context.Tags.FirstOrDefault(c => c.Name == "tutorial") == null)
				context.Tags.Add(new Entities.Tag
				{
					Name = "tutorial",
					Priority = (int)TagPriority.Low,
					TagType = (int)TagType.Default,
					TagColor = "#FFFF00"
				});

			if (context.Tags.FirstOrDefault(c => c.Name == "годно") == null)
				context.Tags.Add(new Entities.Tag
				{
					Name = "годно",
					Priority = (int)TagPriority.Low,
					TagType = (int)TagType.Default,
					TagColor = "#FFFF00"
				});

			base.Seed(context);
		}
	}
}
