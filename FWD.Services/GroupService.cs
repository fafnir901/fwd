using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.Services
{
	public class GroupService : IManagmentEntityWithUser
	{
		private ICommonRepository<ArticleGroup> _repository;
		private ArticleService _service;
		private User _currentUser;

		public GroupService(ICommonRepository<ArticleGroup> repository, ArticleService service, User currentUser = null)
		{
			_repository = repository;
			_service = service;
			_currentUser = currentUser;
		}

		public void SaveGroup(ArticleGroup @group)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			group.GroupId = _repository.Save(group);
		}

		public void SaveManyGroups(IEnumerable<ArticleGroup> groups)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			_repository.SaveMany(groups);
		}

		public void UpdateGroup(ArticleGroup @group)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			group.GroupId = _repository.Update(group);
		}

		public void DeleteGroup(ArticleGroup @group)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			_repository.Delete(group);
		}

		public ArticleGroup GetGroupById(int id)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			return _repository.Read(id);
		}

		public IEnumerable<ArticleGroup> GetAllGroups(Func<ArticleGroup, object> order, int? skip = 0, int? take = 999)
		{
			var queryParams = new QueryParams<ArticleGroup>
			{
				Skip = skip,
				Take = take,
				Order = order
			};
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			return _repository.GetAll(queryParams);
		}

		public IEnumerable<ArticleGroup> GetGroupsByParams(Expression<Func<ArticleGroup, bool>> predicate, Func<ArticleGroup, object> order,
			int? skip = 0, int? take = 999)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			var queryParams = new QueryParams<ArticleGroup>
			{
				Skip = skip,
				Take = take,
				Order = order
			};
			return _repository.GetByPredicate(predicate, queryParams);
		}

		public void DeleteManyGroups(IEnumerable<ArticleGroup> groups)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			if (groups != null)
				_repository.DeleteMany(groups);
		}

		public int TotalCount
		{
			get
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				return _repository.TotalCount;
			}
		}

		public List<Article> GetArticlesInGroup(ArticleGroup @group)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			var listOfArtcile = @group.Groups.Select(name => _service.GetArticlesByParams(c => c.ArticleName == name, c => c.ArticleId, 0, 1).FirstOrDefault()).ToList();
			var res = _service.GetArticlesByParams(c => group.Groups.Contains(c.ArticleName),c=>c.ArticleId);
			group.Articles = listOfArtcile;
			return listOfArtcile;
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
}
