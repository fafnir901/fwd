using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Domain.Dto;
using FWD.CommonIterfaces;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.Services
{
	public class TagService : IManagmentEntityWithUser
	{
		private ICommonRepository<ITag> _repository;
		private ILogger _logger;
		private User _currentUser;

		public TagService(ICommonRepository<ITag> repository, string loggerPath, Logger logger = null, User currentUser = null)
		{
			_repository = repository;
			_logger = logger ?? new Logger(loggerPath);
			_currentUser = currentUser;
		}

		public void SaveTag(ITag tag)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				tag.Id = _repository.Save(tag);
				_logger.Trace(string.Format("Тэг с идентификатором {0} успешно сохранена", tag.Id));
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public void UpdateTag(ITag tag)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_repository.Update(tag);
				_logger.Trace(string.Format("Тэг с идентификатором {0} успешно обновлен", tag.Id));
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public void DeleteTag(int id)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_repository.Delete(new Tag
				{
					Id = id
				});
				_logger.Trace(string.Format("Тэг с идентификатором {0} успешно удален", id));
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public Tag GetTag(int id)
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_logger.Trace(string.Format("Тэг с идентификатором {0} успешно прочитан", id));
				return (Tag) _repository.Read(id);
				
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public TagsCommonInfo GetInfo()
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				var tags = _repository.GetAll(new QueryParams<ITag>(0, 999, c => c.Name));
				_logger.Trace(string.Format("Получение информации о тэгах"));
				return new TagsCommonInfo(tags);
			}
			catch (Exception ex)
			{
				_logger.TraceError(MethodBase.GetCurrentMethod().Name, ex);
				throw;
			}
		}

		public List<TagRadialData> GetRadialSheduleData()
		{
			try
			{
				_repository.MaybeAs<IManagmentEntityWithUser>().Do((c) => c.SetUser(_currentUser));
				_logger.Trace(string.Format("Получение радиального графика по тегам"));
				var sql = "select tag.Name, count(tag.Id) as Count from [Tags] tag " +
							"INNER JOIN [TagArticles] tg " +
							"	ON tag.Id = tg.Tag_Id " +
							"GROUP BY tag.Name";

				var result = _repository.GetBySqlPredicate<TagRadialData>(sql).ToList();
				return result;
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

	public class TagRadialData
	{
		public string Name { get; set; }
		public int Count { get; set; }
	}
}
