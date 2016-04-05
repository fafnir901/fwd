using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using FWD.CommonIterfaces;
using FWD.DAL.Entities;
using FWD.DAL.Entities.Enums;
using FWD.DAL.Helpers;
using FWD.DAL.Model;
using WebRock.Utils.ExpressionTrees;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;
using ArticleGroup = FWD.BusinessObjects.Domain.ArticleGroup;
using User = FWD.BusinessObjects.Domain.User;

namespace FWD.DAL.Domain
{
	using Group = Entities.ArticleGroup;
	public class GroupDbRepository : BaseDB, ICommonRepository<ArticleGroup>, IManagmentEntityWithUser
	{
		private Entities.User _currentUser;
		protected SimpleMapper<ArticleGroup, Group> mapper;
		public GroupDbRepository()
		{
			Container = null;
			ConfigureToBo();
			ConfigureToDb();
			mapper = new SimpleMapper<ArticleGroup, Group>();
			mapper.AddMapping(c => c.GroupId, c => c.GroupId);
			mapper.AddMapping(c => c.GroupName, c => c.GroupName);
		}
		public int Save(ArticleGroup entity, bool now = true)
		{
			return ExecuteWithTry(() =>
			{
				using (Container = new ArticleContext())
				{
					var existings = Container.Groups.FirstOrDefault(c => c.GroupName == entity.GroupName);
					if (existings != null)
					{
						throw new Exception("Группа с именем '{0}' уже существует".Fmt(existings.GroupName));
					}
					var result = Mapper.Map<Group>(entity);
					TransactionHelper.AddTransaction(Container, ActionType.Adding, result, _currentUser);
					Container.Groups.Add(result);
					if (now)
					{
						Container.SaveChanges();
					}
					return result.GroupId;
				}
			});
		}

		public IEnumerable<ArticleGroup> SaveMany(IEnumerable<ArticleGroup> entities)
		{
			return ExecuteWithTry(() =>
			{
				using (Container = new ArticleContext())
				{
					var enumerable = entities as ArticleGroup[] ?? entities.ToArray();
					foreach (var group in enumerable)
					{
						var groupDb = Mapper.Map<ArticleGroup, Group>(group);
						TransactionHelper.AddTransaction(Container, ActionType.Adding, groupDb, _currentUser);
						Container.Groups.Add(groupDb);

					}
					Container.SaveChanges();
					return enumerable;
				}
			});
		}

		public ArticleGroup Read(int id)
		{
			return ExecuteWithTry(() =>
			{
				using (Container = new ArticleContext())
				{
					var groupDb = Container.Groups.FirstOrNothing(c => c.GroupId == id);
					TransactionHelper.AddTransaction(Container, ActionType.Read, groupDb, _currentUser);
					return groupDb.Bind(Mapper.Map<Group, ArticleGroup>).GetOrDefault(null);
				}
			});
		}

		public int Update(ArticleGroup entity)
		{
			if (entity == null || entity.GroupId == 0)
			{
				return -1;
			}
			return ExecuteWithTry(() =>
			{
				using (Container = new ArticleContext())
				{
					var fromDb = Container.Groups.FirstOrDefault(c => c.GroupId == entity.GroupId);
					fromDb.GroupName = entity.GroupName;
					TransactionHelper.AddTransaction(Container, ActionType.Updating, fromDb, _currentUser);
					Container.SaveChanges();
					return fromDb.GroupId;
				}
			});
		}

		public void Delete(ArticleGroup entity, bool now = true)
		{
			if(entity.GroupId == 1)
				throw new Exception("Не возможно удалить группу по умолчанию");
			ExecuteWithTry(() =>
			{
				using (Container = new ArticleContext())
				{
					var existingArticles = Container.Articles.Where(c => c.Group.GroupId == entity.GroupId);
					var defaultGroup = Container.Groups.FirstOrDefault(c => c.GroupId == 1);
					foreach (var existingArticle in existingArticles)
					{
						existingArticle.Group = defaultGroup;
					}
					var fromDb = Container.Groups.First(c => c.GroupId == entity.GroupId);
					TransactionHelper.AddTransaction(Container, ActionType.Deleting, fromDb, _currentUser);
					Container.Groups.Remove(fromDb);
					if (now)
					{
						Container.SaveChanges();
					}
					return 0;
				}
			});
		}

		public void DeleteMany(IEnumerable<ArticleGroup> entities)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ArticleGroup> GetAll(QueryParams<ArticleGroup> param)
		{
			return ExecuteWithTry(() =>
			{
				param = QueryParams<ArticleGroup>.Validate(param, c => c.GroupId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Groups.OrderBy(c => c.GroupId).Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
					var result = Mapper.Map<IEnumerable<Group>, IEnumerable<ArticleGroup>>(list);
					return result.OrderBy(param.Order);
				}
			});
		}

		public IEnumerable<ArticleGroup> GetByPredicate(Expression<Func<ArticleGroup, bool>> predicate, QueryParams<ArticleGroup> param)
		{
			return ExecuteWithTry(() =>
			{
				//var currentPredicate = predicate.Convert<Group, ArticleGroup>(mapper);
				var anotherPredicate = Mapper.Map<Expression<Func<ArticleGroup, bool>>, Expression<Func<Group, bool>>>(predicate);
				param = QueryParams<ArticleGroup>.Validate(param, c => c.GroupId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Groups
						.OrderBy(c => c.GroupId)
						.Where(anotherPredicate)
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Group>, IEnumerable<ArticleGroup>>(list);
					return result.OrderBy(param.Order);
				}
			});
		}

		public IEnumerable<ArticleGroup> GetBySqlPredicate(string sql, params object[] args)
		{
			return ExecuteWithTry(() =>
			{
				using (Container = new ArticleContext())
				{
					return Mapper.Map<IEnumerable<Group>,IEnumerable<ArticleGroup>>(Container.Database.SqlQuery<Group>(sql, args).ToList());
				}
			});
		}

		public IEnumerable<T1> GetBySqlPredicate<T1>(string sql, params object[] args)
		{
			throw new NotImplementedException();
		}

		public DataSet GetDataSetBySqlPredicate(string sql, params object[] args)
		{
			throw new NotImplementedException();
		}

		public int TotalCount { get { return Container.Groups.Count(); } }
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
			return Mapper.Map<Entities.User,BusinessObjects.Domain.User>(_currentUser);
		}
	}
}
