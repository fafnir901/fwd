using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Domain.Dto;
using FWD.CommonIterfaces;
using FWD.CommonIterfaces.Utils;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.Services
{
	public class ArticleService : IManagmentEntityWithUser
	{
		private ICommonRepository<Article> _repository;
		private ILogger _logger;
		private User _currentUser;
		public ArticleService(ICommonRepository<Article> repository, string loggerPath, Logger logger = null, User currentUser = null)
		{
			_repository = repository;
			_logger = logger ?? new Logger(loggerPath);
			_currentUser = currentUser;
		}

		public void SaveArticle(Article article)
		{
			try
			{
				article.Images = article.Images != null && article.Images.Any() 
					? article.Images.Distinct(ImageComparer.Instance).ToList() 
					: null;

				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));

				article.ArticleId = _repository.Save(article);
				_logger.Trace(string.Format("Статья с идентификатором {0} успешно сохранена", article.ArticleId));
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public void SaveManyArticles(IEnumerable<Article> articles)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				var res = _repository.SaveMany(articles);
				_logger.Trace(string.Format("Метод {0}.Производиться добавления новых статей. Идентификаторы статьей: {1}", MethodBase.GetCurrentMethod().Name, string.Join(",", res.Select(c => c.ArticleId))));
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public void UpdateArticle(Article article)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				article.Images = article.Images.Distinct(ImageComparer.Instance).ToList();
				article.ArticleId = _repository.Update(article);
				_logger.Trace(string.Format("Метод {0}.Производиться обновление статьи. Идентификатор статьи: {1}", MethodBase.GetCurrentMethod().Name, article.ArticleId));
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public void DeleteArticle(Article article, GroupService service = null)
		{
			try
			{
				if (service != null)
				{
					var group = service.GetGroupsByParams(c => c.Groups.Contains(article.ArticleName), c => c.GroupId);
					foreach (var articleGroup in group)
					{
						articleGroup.Groups.Remove(article.ArticleName);
						service.UpdateGroup(articleGroup);
					}
				}
				_logger.Trace(string.Format("Метод {0}.Производиться удаление статьи. Идентификатор статьи: {1}", MethodBase.GetCurrentMethod().Name, article.ArticleId));
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_repository.Delete(article);
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public Article GetArticleById(int id)
		{
			try
			{
				_logger.Trace(string.Format("Метод {0}.Производиться выбор статьи. Идентификатор статьи: {1}", MethodBase.GetCurrentMethod().Name, id));
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				return _repository.Read(id);
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public async Task<Article> GetArticleByIdAsync(int id)
		{
			return await Task.Factory.StartNew(() =>
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				return _repository.Read(id);
			});
		}

		public string ExportFromDb()
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_logger.Trace(string.Format("Метод {0}. Производится экспорт", MethodBase.GetCurrentMethod().Name));
				return _repository.Export();
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public string ExportFromDb(int articleId)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_logger.Trace(string.Format("Метод {0}. Производится экспорт", MethodBase.GetCurrentMethod().Name));
				return _repository.Export(articleId);
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public List<string> Import(bool isDb, byte[] data)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_logger.Trace(string.Format("Метод {0}. Производится импорт", MethodBase.GetCurrentMethod().Name));
				return _repository.Import(isDb, data);
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public IEnumerable<Article> GetAllArticles(Func<Article, object> order, int? skip = 0, int? take = 999)
		{
			var queryParams = new QueryParams<Article>
			{
				Skip = skip,
				Take = take,
				Order = order
			};
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			_logger.Trace(string.Format("Метод {0}.Производиться выборка всех статей.Параметры выборки: skip {1},take {2}", MethodBase.GetCurrentMethod().Name, queryParams.Skip, queryParams.Take));
			return _repository.GetAll(queryParams);
		}

		public ArticleSheduleData GetSheduleData()
		{
			_logger.Trace(string.Format("Метод {0}.Производиться выборка данных для графика зависимости.", MethodBase.GetCurrentMethod().Name));
			var sql = "select distinct art.ArticleId as ArticleId, art.ArticleName as ArticleName, logs.ActionDateTime as ActionDateTime from [Articles] art " +
					"INNER JOIN [TransactionLogs] logs " +
					"ON logs.EntityType = 1 " +
					"AND logs.ActionType = 1 " +
					"AND art.ArticleId = logs.EntityId";
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			var result = _repository.GetDataSetBySqlPredicate(sql);
			var shedule = new ArticleSheduleData(result.Tables[0].Rows.Cast<DataRow>().Select(c => new ArticleDto
			{
				AddedDate = c["ActionDateTime"].MaybeAs<DateTime>().GetOrDefault(default(DateTime)),
				ArticleId = c["ArticleId"].MaybeAs<int>().GetOrDefault(-1),
				ArticleName = c["ArticleName"].MaybeAs<string>().GetOrDefault(string.Empty)
			}));
			return shedule;
		}

		public IEnumerable<Article> GetArticlesByParams(Expression<Func<Article, bool>> predicate, Func<Article, object> order,
			int? skip = 0, int? take = 999)
		{
			try
			{
				var queryParams = new QueryParams<Article>
				{
					Skip = skip,
					Take = take,
					Order = order
				};
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_logger.Trace(string.Format("Метод {0}.Производиться выборка стотей по предикату статьи.", MethodBase.GetCurrentMethod().Name));
				return _repository.GetByPredicate(predicate, queryParams);
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public void DeleteManyArticles(IEnumerable<Article> articles, GroupService service = null)
		{
			try
			{
				if (service != null)
				{
					foreach (var article in articles)
					{
						var group = service.GetGroupsByParams(c => c.Groups.Contains(article.ArticleName), c => c.GroupId);
						foreach (var articleGroup in group)
						{
							articleGroup.Groups.Remove(article.ArticleName);
							service.UpdateGroup(articleGroup);
						}
					}

				}
				if (articles != null)
				{
					_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
					_repository.DeleteMany(articles);
				}
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public int TotalCount
		{
			get
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				return _repository.TotalCount;
			}
		}

		public string Path
		{
			get
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				return _repository.CurrentXmlPath;
			}
		}

		public IEnumerable<ArticleSearchResult> GetBySearchStringWithTags(string search, Expression<Func<Article, object>> order,
			int? skip = 0, int? take = 999, string fmt = "")
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));

				_logger.Trace(string.Format("Метод {0}.Производиться поиск статей. Строка поиска: {1}", MethodBase.GetCurrentMethod().Name, search));

				return
					_repository.MaybeAs<ISpecifiedArticleRepository>()
					.Bind((c) => c.GetWithTags(search, new QueryParams<Article>(0, 999, x => x.ArticleName))
					.Select(x => new ArticleSearchResult(x, search, fmt))
					.ToList())
					.GetOrDefault(new List<ArticleSearchResult>());
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public IEnumerable<ArticleSearchResult> GetBySearchString(string search, Expression<Func<Article, object>> order,
			int? skip = 0, int? take = 999, string fmt = "")
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				var sql =
					("DECLARE @SearchString nvarchar(max)= upper(N'{0}') "
					 + "SELECT TOP {1} * FROM (select *,row_number() OVER (ORDER BY {2}) as RowNumber from [Articles] a" +
					 " where UPPER(a.ArticleName) like '%'+@SearchString+'%'"
					 + " OR UPPER(a.AuthorName) like '%'+@SearchString+'%' OR UPPER(a.InitialText) like '%'+@SearchString+'%') as Extent WHERE Extent.RowNumber > {3} ORDER BY Extent.{2}")
						.Fmt(search, take, order.GetProperty().Name, skip);
				_logger.Trace(string.Format("Метод {0}.Производиться поиск статей. Строка поиска: {1}", MethodBase.GetCurrentMethod().Name, search));
				return _repository.GetBySqlPredicate(sql).Select(c => new ArticleSearchResult(c, search, fmt));
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public IEnumerable<ArticleSearchResult> GetByTagIDsWithTags(IEnumerable<int> ids, Func<Article, object> func, int? skip = 0, int? take = 999, string fmt = "")
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				var searcgString = string.Join(",", ids);
				_logger.Trace(string.Format("Метод {0}.Производиться поиск статей по тэгам. Строка поиска: {1}", MethodBase.GetCurrentMethod().Name, searcgString));
				return
					_repository.MaybeAs<ISpecifiedArticleRepository>()
					.Bind((c) => c.GetByTagWithTags(ids, new QueryParams<Article>(0, 999, x => x.ArticleName))
					.Select(x => new ArticleSearchResult(x, searcgString, fmt))
					.ToList())
					.GetOrDefault(new List<ArticleSearchResult>());
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public void SetUser(User user)
		{
			_currentUser = user;
		}

		public User GetUser()
		{
			return _currentUser;
		}
	}

	public class ImageComparer : IEqualityComparer<Image>
	{
		private ImageComparer()
		{
		}

		private static ImageComparer _instance;

		public static ImageComparer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ImageComparer();
				}
				return _instance;
			}
		}

		public bool Equals(Image x, Image y)
		{
			if (x.Name == y.Name)
			{
				return true;
			}
			return false;
		}

		public int GetHashCode(Image obj)
		{
			return obj.Name.GetHashCode() * obj.GetHashCode();
		}
	}
}
