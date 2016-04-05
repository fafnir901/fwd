using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.Services
{
	public class PlanService : IManagmentEntityWithUser
	{
		private ICommonRepository<IPlan> _repository;
		private User _currentUser;

		public PlanService(ICommonRepository<IPlan> repository, User currentUser = null)
		{
			_repository = repository;
			_currentUser = currentUser;
		}

		public void SavePlan(IPlan article)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			article.Id = _repository.Save(article);
		}

		public void SaveManyPlans(IEnumerable<IPlan> plans)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			_repository.SaveMany(plans);
		}

		public void UpdatePlan(IPlan plan)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			plan.Id = _repository.Update(plan);
		}

		public void DeletePlan(IPlan plan)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			_repository.Delete(plan);
		}

		public IPlan GetPlanById(int id)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			return _repository.Read(id);
		}

		public IEnumerable<IPlan> GetAllPlans(Func<IPlan, object> order, int? skip = 0, int? take = 999)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			var queryParams = new QueryParams<IPlan>
			{
				Skip = skip,
				Take = take,
				Order = order
			};

			var result = _repository.GetAll(queryParams).ToList();
			DeleteOldPlans(result);
			return result;
		}

		public IEnumerable<IPlan> GetPlansByParams(Expression<Func<IPlan, bool>> predicate, Func<IPlan, object> order,
			int? skip = 0, int? take = 999)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			var queryParams = new QueryParams<IPlan>
			{
				Skip = skip,
				Take = take,
				Order = order
			};
			var result = _repository.GetByPredicate(predicate, queryParams).ToList();
			DeleteOldPlans(result);
			return result;
		}

		public void DeleteManyPlans(IEnumerable<IPlan> plans)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			if (plans != null)
				_repository.DeleteMany(plans);
		}

		public int TotalCount
		{
			get
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				return _repository.TotalCount;
			}
		}

		private void DeleteOldPlans(List<IPlan> plans)
		{
			_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
			var deletingPlans = plans.Where(c => c.PossibleChangeDate != null);
			var lst = new List<IPlan>();
			lst.AddRange(deletingPlans);
			foreach (var deletingPlan in lst)
			{
				var span = DateTime.Now - deletingPlan.PossibleChangeDate.Value;
				if (span.Days > 3 && deletingPlan.IsDone)
				{
					DeletePlan(deletingPlan);
					plans.Remove(deletingPlan);
				}
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
}
