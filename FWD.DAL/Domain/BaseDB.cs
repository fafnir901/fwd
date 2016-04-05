//using FWD.DAL.Model;

using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using FWD.DAL.Model;

namespace FWD.DAL.Domain
{
	public abstract class BaseDB
	{
		protected ArticleContext Container;

		protected  void ConfigureToDb()
		{
			Mapping.Mapper.ConfigureMapToDb();
		}

		protected void ConfigureToBo()
		{
			Mapping.Mapper.ConfigureMapToBo();
		}

		public abstract string Export();
		public string GetSize()
		{
			try
			{
				using (Container = new ArticleContext())
				{
					var res = Container.Database.SqlQuery<HelpResult>("EXEC sp_helpdb;");
					var firstOrDefault = res.FirstOrDefault(c => c.Name == "Article");
					if (firstOrDefault != null)
						return firstOrDefault.Db_Size;
					return string.Empty;
				}
			}
			catch (Exception)
			{
				return "База данных отсутсвует";
			}
			
		}

		protected TIn ExecuteWithTry<TIn>(Func<TIn> func)
		{
			try
			{
				return func();

			}
			catch (DbEntityValidationException ex)
			{
				foreach (var dbEntityValidationResult in ex.EntityValidationErrors)
				{
					foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
					{
						Console.WriteLine(dbValidationError.ErrorMessage);
					}
				}
				throw;
			}
			catch (Exception e)
			{
				throw;
			}
		}

		private class HelpResult
		{
			public string Name { get; set; }
			public string Db_Size { get; set; }
		}
	}
}
