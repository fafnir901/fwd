using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using FWD.DAL.Entities;
using FWD.DAL.Entities.Enums;
using FWD.DAL.Helpers;
using FWD.DAL.Model;
using WebRock.Utils.ExpressionTrees;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;
using Article = FWD.BusinessObjects.Domain.Article;
using Comment = FWD.BusinessObjects.Domain.Comment;

namespace FWD.DAL.Domain
{
	public class CommentDbRepository : BaseDB, ICommonRepository<Comment>
	{
		protected SimpleMapper<Comment, Entities.Comment> mapper;
		public CommentDbRepository()
		{
			Container = null;
			ConfigureToBo();
			ConfigureToDb();

			mapper = new SimpleMapper<Comment, Entities.Comment>();
			mapper.AddMapping(c => c.CommentId, c => c.CommentId);
			mapper.AddMapping(c => c.CommentText, c => c.CommentText);
			mapper.AddMapping(c => c.GroupName, c => c.GroupName);
			mapper.AddMapping(c => c.UserName, c => c.UserName);
			mapper.AddMapping(c => c.AddedDate, c => c.AddedDate);
		}
		public int Save(Comment entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var commDb = Mapper.Map<Comment, Entities.Comment>(entity);
					var user = Container.Users.First(c => c.UserId == commDb.UserId);
					TransactionHelper.AddTransaction(Container, ActionType.Adding, commDb, user);
					Container.Comments.Add(commDb);
					if (now)
					{
						Container.SaveChanges();
					}
					return 0;
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

		public IEnumerable<Comment> SaveMany(IEnumerable<Comment> entities)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					var list = new List<Entities.Comment>();
					var enumerable = entities as Comment[] ?? entities.ToArray();
					foreach (var comment in enumerable)
					{
						var commentDb = Mapper.Map<Comment, Entities.Comment>(comment);
						var user = Container.Users.First(c => c.UserId == commentDb.UserId);
						TransactionHelper.AddTransaction(Container, ActionType.Adding, commentDb, user);
						Container.Comments.Add(commentDb);

					}
					Container.SaveChanges();
					return enumerable;
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

		public Comment Read(int id)
		{
			return null;
		}

		public int Update(Comment entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(Comment entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return;
				}
				using (Container = new ArticleContext())
				{
					var commentDb = Container.Comments.FirstOrNothing(c => c.CommentId == entity.CommentId);
					if (commentDb != Maybe.Nothing)
					{
						var user = Container.Users.First(c => c.UserId == commentDb.Value.UserId);
						TransactionHelper.AddTransaction(Container, ActionType.Deleting, commentDb.Value, user);
						Container.Comments.Remove(commentDb.Value);
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
				throw;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void DeleteMany(IEnumerable<Comment> entities)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Comment> GetAll(QueryParams<Comment> param)
		{
			try
			{
				param = QueryParams<Comment>.Validate(param, c => c.GroupName, 999);
				using (Container = new ArticleContext())
				{
					var list = Container.Comments.OrderBy(c => c.GroupName).Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
					var result = Mapper.Map<IEnumerable<Entities.Comment>, IEnumerable<Comment>>(list);
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

		public IEnumerable<Comment> GetByPredicate(Expression<Func<Comment, bool>> predicate, QueryParams<Comment> param)
		{
			try
			{
				//var currentPredicate = predicate.Convert<Entities.Comment, Comment>(mapper);
				var anotherPredicate = Mapper.Map<Expression<Func<Comment, bool>>, Expression<Func<Entities.Comment, bool>>>(predicate);
				param = QueryParams<Comment>.Validate(param, c => c.GroupName, 999);
				using (Container = new ArticleContext())
				{
					var list = Container.Comments
						.OrderBy(c => c.GroupName)
						.Where(anotherPredicate)
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Entities.Comment>, IEnumerable<Comment>>(list);
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
				throw;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public IEnumerable<Comment> GetBySqlPredicate(string sql, params object[] args)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					var res =  Container.Database.SqlQuery<Entities.Comment>(sql, args).ToList();
					return Mapper.Map<List<Entities.Comment>, List<Comment>>(res);
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

		public IEnumerable<T1> GetBySqlPredicate<T1>(string sql, params object[] args)
		{
			throw new NotImplementedException();
		}

		public DataSet GetDataSetBySqlPredicate(string sql, params object[] args)
		{
			throw new NotImplementedException();
		}

		public int TotalCount
		{
			get
			{
				return Container.Comments.Count();
			}
		}
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

		public List<string> Import(bool isDb, byte[] data)
		{
			throw new NotImplementedException();
		}
	}
}
