using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FWD.BusinessObjects.Absrtact;
using FWD.CommonIterfaces;
using FWD.DAL.Entities;
using FWD.DAL.Entities.Enums;
using FWD.DAL.Helpers;
using FWD.DAL.Model;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;
using User = FWD.BusinessObjects.Domain.User;

namespace FWD.DAL.Domain
{
	using Tag = FWD.BusinessObjects.Domain.Tag;
	public class TagDbRepository : BaseDB, ICommonRepository<ITag>, IManagmentEntityWithUser
	{
		private Entities.User _currentUser;
		public TagDbRepository()
		{
			Container = new ArticleContext();
			ConfigureToBo();
			ConfigureToDb();
		}
		public int Save(ITag entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var tagDb = Mapper.Map<Tag, Entities.Tag>(entity as Tag);
					if (Container.Tags.FirstOrDefault(c => c.Name == tagDb.Name) != null)
					{
						throw new Exception("Найден тэг с таким же именем.Тэг не будет сохранен.");
					}


					Container.Tags.Add(tagDb);
					if (now)
					{
						Container.SaveChanges();
					}
					entity.Id = tagDb.Id;
					TransactionHelper.AddTransaction(Container, ActionType.Adding, tagDb, _currentUser);
					if (now)
					{
						Container.SaveChanges();
					}
					return tagDb.Id;
				}
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

		public IEnumerable<ITag> SaveMany(IEnumerable<ITag> entities)
		{
			throw new NotImplementedException();
		}

		public ITag Read(int id)
		{
			try
			{
				if (id <= 0)
				{
					return null;
				}
				using (Container = new ArticleContext())
				{
					var artDb = Container.Tags.FirstOrNothing(c => c.Id == id);
					if (artDb.HasValue)
					{
						TransactionHelper.AddTransaction(Container, ActionType.Read, artDb.Value, _currentUser);
						//foreach (var embdedImage in artDb.Value.EmbdedImages)
						//{
						//	TransactionHelper.AddTransaction(Container, ActionType.Read, embdedImage);
						//}
						Container.SaveChanges();
					}
					return artDb.Bind(Mapper.Map<Entities.Tag, FWD.BusinessObjects.Domain.Tag>).GetOrDefault(null);
				}
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

		public int Update(ITag entity)
		{
			try
			{
				if (entity == null)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var tagDb = Container.Tags.FirstOrDefault(c => c.Id == entity.Id);

					tagDb.Name = entity.Name;
					tagDb.TagColor = entity.TagColor;
					tagDb.Priority = entity.Priority;
					tagDb.TagType = entity.TagType;

					Container.SaveChanges();

					TransactionHelper.AddTransaction(Container, ActionType.Updating, tagDb, _currentUser);

					Container.SaveChanges();

					return tagDb.Id;
				}
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

		public void Delete(ITag entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return;
				}
				using (Container = new ArticleContext())
				{
					var artDb = Container.Tags.FirstOrNothing(c => c.Id == entity.Id);
					if (artDb != Maybe.Nothing)
					{
						TransactionHelper.AddTransaction(Container, ActionType.Deleting, artDb.Value, _currentUser);
						Container.Tags.Remove(artDb.Value);
						if (now)
						{
							Container.SaveChanges();
						}
					}
				}
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
				throw ex;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public void DeleteMany(IEnumerable<ITag> entities)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ITag> GetAll(QueryParams<ITag> param)
		{
			try
			{
				param = QueryParams<ITag>.Validate(param, c => c.Id, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Tags.OrderBy(c => c.Id).Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
					var result = Mapper.Map<IEnumerable<Entities.Tag>, IEnumerable<Tag>>(list);
					//var enumerable = result as IList<Article> ?? result.ToList();
					//foreach (var article in enumerable)
					//{
					//	article.GroupId = article.ArticleGroup.GroupId;
					//}
					return result.OrderBy(param.Order).ToList();
				}
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
				throw ex;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public IEnumerable<ITag> GetByPredicate(Expression<Func<ITag, bool>> predicate, QueryParams<ITag> param)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ITag> GetBySqlPredicate(string sql, params object[] args)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T1> GetBySqlPredicate<T1>(string sql, params object[] args)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					return Container.Database.SqlQuery<T1>(sql, args).ToList();
				}

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

		public DataSet GetDataSetBySqlPredicate(string sql, params object[] args)
		{
			using (Container = new ArticleContext())
			{
				var retVal = new DataSet();
				var sqlConn = Container.Database.Connection as SqlConnection;
				var sqlQuery = new SqlCommand(sql, sqlConn) { CommandType = CommandType.Text };
				var adapter = new SqlDataAdapter(sqlQuery);
				using (sqlQuery)
				{
					if (args.Any())
					{
						foreach (var o in args)
						{
							var param = new SqlParameter(o.ToString(), o);
							sqlQuery.Parameters.Add(param);
						}
					}
					adapter.Fill(retVal);
				}
				return retVal;
			}
		}

		public int TotalCount { get; private set; }
		public string CurrentXmlPath { get; private set; }

		public override string Export()
		{
			throw new NotImplementedException();
		}

		public string Export(int articleId)
		{
			throw new NotImplementedException();
		}

		public List<string> Import(bool isDb, byte[] data)
		{
			throw new NotImplementedException();
		}

		public void SetUser(BusinessObjects.Domain.User user)
		{
			_currentUser = Mapper.Map<BusinessObjects.Domain.User, Entities.User>(user);
		}

		public User GetUser()
		{
			return Mapper.Map<Entities.User, BusinessObjects.Domain.User>(_currentUser);
		}
	}
}
