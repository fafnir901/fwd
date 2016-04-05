using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FWD.CommonIterfaces;
using FWD.DAL.Mapping;
using FWD.DAL.Model;
using WebRock.Utils.ExpressionTrees;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;
using AutoMapper;
using FWD.BusinessObjects.Domain;
using Article = FWD.BusinessObjects.Domain.Article;
using Mapper = AutoMapper.Mapper;
using User = FWD.BusinessObjects.Domain.User;

namespace FWD.DAL.Domain
{
	public class UserDbRepository : BaseDB, ICommonRepository<User>
	{
		protected SimpleMapper<User, Entities.User> mapper;
		public UserDbRepository()
		{
			Container = new ArticleContext();
			if (!Container.Database.Exists())
			{
				throw new EntitySqlException("База данных осутствует.");
			}
			//if (!Container.)
			//{
			//	throw new EntitySqlException("База данных осутствует.");
			//}
			Container = null;
			ConfigureToBo();
			ConfigureToDb();
			mapper = new SimpleMapper<User, Entities.User>();
			mapper.AddMapping(c => c.UserId, c => c.UserId);
			mapper.AddMapping(c => c.LastName, c => c.LastName);
			mapper.AddMapping(c => c.FirstName, c => c.FirstName);
			mapper.AddMapping(c => c.Email, c => c.Email);
			mapper.AddMapping(c => c.Avatar, c => c.Avatar);
			//mapper.AddMapping(c => c.Credential.CredentialId, c => c.Credential.CredentialId);
			mapper.AddMapping(c => c.Credential.Login, c => c.Login);
			mapper.AddMapping(c => c.Credential.Password, c => c.Password);
		}
		public int Save(User entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var artDb = Mapper.Map<User, Entities.User>(entity);

					Container.Users.Add(artDb);
					if (now)
					{
						Container.SaveChanges();
					}
					entity.UserId = artDb.UserId;

					return artDb.UserId;
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

		public IEnumerable<User> SaveMany(IEnumerable<User> entities)
		{
			throw new NotImplementedException();
		}

		public User Read(int id)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					
					var user = Container.Users.FirstOrDefault(c=>c.UserId == id);
					if(user != null)
						return Mapper.Map<Entities.User, User>(user);
					return null;
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

		public int Update(User entity)
		{
			try
			{
				if (entity == null || entity.UserId == 0)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var fromDb = Container.Users.FirstOrDefault(c => c.UserId == entity.UserId);

					//fromDb = Mapper.Map<User, Entities.User>(entity);
					fromDb.UserRole = (int)entity.UserRole;
					fromDb.LastName = entity.LastName;
					fromDb.FirstName = entity.FirstName;
					fromDb.Avatar = entity.Avatar;
					fromDb.Email = entity.Email;
					fromDb.Login = entity.Credential.Login;
					fromDb.Password = entity.Credential.Password;
					
					//TransactionHelper.AddTransaction(Container, ActionType.Updating, fromDb);
					Container.SaveChanges();
					return fromDb.UserId;
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

		public void Delete(User entity, bool now = true)
		{
			throw new NotImplementedException();
		}

		public void DeleteMany(IEnumerable<User> entities)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<User> GetAll(QueryParams<User> param)
		{
			try
			{
				param = QueryParams<User>.Validate(param, c => c.UserId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Users.OrderBy(c => c.UserId).Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
					var result = Mapper.Map<IEnumerable<Entities.User>, IEnumerable<User>>(list);
					return result.OrderBy(param.Order);
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

		public IEnumerable<User> GetByPredicate(Expression<Func<User, bool>> predicate, QueryParams<User> param)
		{
			try
			{
				//var currentPredicate = predicate.Convert<Entities.User, User>(mapper);
				var anotherPredicate = Mapper.Map<Expression<Func<User, bool>>, Expression<Func<Entities.User, bool>>>(predicate);
				param = QueryParams<User>.Validate(param, c => c.UserId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Users
						.OrderBy(c => c.UserId)
						.Where(anotherPredicate)
						.Skip(param.Skip ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Entities.User>, IEnumerable<User>>(list);
					return result.OrderBy(param.Order);
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

		public IEnumerable<User> GetBySqlPredicate(string sql, params object[] args)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					var users = Container.Database.SqlQuery<Entities.User>(sql, args).ToList().MaybeAs<List<Entities.User>>().Bind(Mapper.Map<List<Entities.User>, List<User>>).GetOrDefault(new List<User>());
					return users;
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
			catch (Exception)
			{
				throw;
			}
		}

		public IEnumerable<T1> GetBySqlPredicate<T1>(string sql, params object[] args)
		{
			throw new NotImplementedException();
		}

		public DataSet GetDataSetBySqlPredicate(string sql, params object[] args)
		{
			throw new NotImplementedException();
		}

		public int TotalCount { get; private set; }

		public string CurrentXmlPath
		{
			get
			{
				return null;
			}
		}

		public override string Export()
		{
			throw new NotImplementedException();
		}

		public string Export(int articleId)
		{
			throw new NotImplementedException();
		}

		public List<string> Import(bool isDb,byte[] data)
		{
			throw new NotImplementedException();
		}
	}
}
