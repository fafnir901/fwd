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
using FWD.DAL.Helpers;
using FWD.DAL.Model;
using WebRock.Utils.ExpressionTrees;
using WebRock.Utils.Mappers;
using WebRock.Utils.UtilsEntities;
using Article = FWD.BusinessObjects.Domain.Article;

namespace FWD.DAL.Domain
{
	public class TransactionDBRepository : BaseDB, ICommonRepository<Transaction>
	{
		protected SimpleMapper<Transaction, Entities.TransactionLog> mapper;
		public TransactionDBRepository()
		{
			ConfigureToBo();
			ConfigureToDb();

			mapper = new SimpleMapper<Transaction, Entities.TransactionLog>();
			mapper.AddMapping(c => c.Id, c => c.Id);
			mapper.AddMapping(c => c.EntityId, c => c.EntityId);
			mapper.AddMapping(c => c.EntityType, c => c.EntityType.GetEntityTypeName());
			mapper.AddMapping(c => c.Description, c => c.Description);
			mapper.AddMapping(c => c.ActionType, c => c.ActionType.GetActionTypeName());
			mapper.AddMapping(c => c.ActionDateTime, c => c.ActionDateTime);
			mapper.AddMapping(c => c.Parameters, c => c.Parameters);
		}

		public int Save(Transaction entity, bool now = true)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Transaction> SaveMany(IEnumerable<Transaction> entities)
		{
			throw new NotImplementedException();
		}

		public Transaction Read(int id)
		{
			throw new NotImplementedException();
		}

		public int Update(Transaction entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(Transaction entity, bool now = true)
		{
			throw new NotImplementedException();
		}

		public void DeleteMany(IEnumerable<Transaction> entities)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Transaction> GetAll(QueryParams<Transaction> param)
		{
			try
			{
				param = QueryParams<Transaction>.Validate(param, c => c.Id, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.TransactionLogs.OrderByDescending(c => c.ActionDateTime).Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
					var result = Mapper.Map<IEnumerable<Entities.TransactionLog>, IEnumerable<Transaction>>(list);
					return result; //.OrderBy(param.Order);
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

		public IEnumerable<Transaction> GetByPredicate(Expression<Func<Transaction, bool>> predicate, QueryParams<Transaction> param)
		{
			try
			{
				//var currentPredicate = predicate.Convert<TransactionLog,Transaction>(mapper);
				var anotherPredicate = Mapper.Map<Expression<Func<Transaction, bool>>, Expression<Func<TransactionLog, bool>>>(predicate);
				param = QueryParams<Transaction>.Validate(param, c => c.Id, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.TransactionLogs
						.OrderByDescending(c => c.Id)
						.Where(anotherPredicate)
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Entities.TransactionLog>, IEnumerable<Transaction>>(list);
					return result;
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

		public IEnumerable<Transaction> GetBySqlPredicate(string sql, params object[] args)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					var logs =  Container.Database.SqlQuery<TransactionLog>(sql, args).ToList();
					var result = Mapper.Map<IEnumerable<Entities.TransactionLog>, IEnumerable<Transaction>>(logs);
					return result;
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
			throw new NotImplementedException();
		}

		public int TotalCount
		{
			get
			{
				using (Container = new ArticleContext())
				{
					return Container.TransactionLogs.Count();
				}
			}
		}

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
	}
}
