using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FWD.BusinessObjects.Absrtact;
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
using Plan = FWD.DAL.Entities.Plan;
using User = FWD.BusinessObjects.Domain.User;

namespace FWD.DAL.Domain
{
	public class PlanDbRepository : BaseDB, ICommonRepository<IPlan>, IManagmentEntityWithUser
	{
		private Entities.User _currentUser;
		protected SimpleMapper<CurrentPlan, Entities.Plan> mapper;
		public PlanDbRepository()
		{
			Container = new ArticleContext();
			Container = null;
			ConfigureToBo();
			ConfigureToDb();
			mapper = new SimpleMapper<CurrentPlan, Entities.Plan>();
			mapper.AddMapping(c => c.Id, c => c.Id);
			mapper.AddMapping(c => c.Name, c => c.Name);
			mapper.AddMapping(c => c.Description, c => c.Description);
			mapper.AddMapping(c => c.IsDone, c => c.IsDone);
			mapper.AddMapping(c => c.AddedDate, c => c.AddedDate);
			mapper.AddMapping(c => c.PossibleChangeDate, c => c.PossibleChangeDate);
		}
		public int Save(IPlan entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var current = entity as CurrentPlan;
					var planDb = Mapper.Map<CurrentPlan, Plan>(current);
					if (Container.Plans.FirstOrDefault(c => c.Name == entity.Name) != null)
					{
						throw new Exception("Найден план с таким же названием");
					}


					Container.Plans.Add(planDb);
					if (now)
					{
						Container.SaveChanges();
					}
					entity.Id = planDb.Id;

					TransactionHelper.AddTransaction(Container, ActionType.Adding, planDb, _currentUser);
					if (now)
					{
						Container.SaveChanges();
					}
					return planDb.Id;
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

		public IEnumerable<IPlan> SaveMany(IEnumerable<IPlan> entities)
		{
			throw new NotImplementedException();
		}

		public IPlan Read(int id)
		{
			try
			{
				if (id <= 0)
				{
					return null;
				}
				using (Container = new ArticleContext())
				{
					var artDb = Container.Plans.FirstOrNothing(c => c.Id == id);
					if (artDb.HasValue)
					{
						TransactionHelper.AddTransaction(Container, ActionType.Read, artDb.Value, _currentUser);
						Container.SaveChanges();
					}
					return artDb.Bind(Mapper.Map<Entities.Plan, CurrentPlan>).GetOrDefault(null);
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

		public int Update(IPlan entity)
		{
			try
			{
				if (entity == null || entity.Id == 0)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var fromDb = Container.Plans.FirstOrDefault(c => c.Id == entity.Id);

					fromDb.Name = entity.Name;
					fromDb.Description = entity.Description;
					fromDb.PossibleChangeDate = DateTime.Now;
					fromDb.IsDone = entity.IsDone;

					TransactionHelper.AddTransaction(Container, ActionType.Updating, fromDb, _currentUser);
					Container.SaveChanges();
					return fromDb.Id;
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

		public void Delete(IPlan entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return;
				}
				using (Container = new ArticleContext())
				{
					var artDb = Container.Plans.FirstOrNothing(c => c.Id == entity.Id);
					if (artDb != Maybe.Nothing)
					{
						TransactionHelper.AddTransaction(Container, ActionType.Deleting, artDb.Value, _currentUser);
						Container.Plans.Remove(artDb.Value);
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

		public void DeleteMany(IEnumerable<IPlan> entities)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPlan> GetAll(QueryParams<IPlan> param)
		{
			try
			{
				param = QueryParams<IPlan>.Validate(param, c => c.Id, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Plans.OrderBy(c => c.Id)
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Entities.Plan>, IEnumerable<CurrentPlan>>(list);
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

		public IEnumerable<IPlan> GetByPredicate(Expression<Func<IPlan, bool>> predicate, QueryParams<IPlan> param)
		{
			try
			{
				param = QueryParams<IPlan>.Validate(param, c => c.Id, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Plans.OrderBy(c => c.Id)
						.Where(predicate)
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Entities.Plan>, IEnumerable<CurrentPlan>>(list.Select(c => c as Plan));
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

		public IEnumerable<IPlan> GetBySqlPredicate(string sql, params object[] args)
		{
			throw new NotImplementedException();
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
				using (Container = new ArticleContext())
				{
					return Container.Plans.Count();
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
