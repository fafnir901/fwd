using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using WebRock.Utils.UtilsEntities;

namespace FWD.Services
{
	public class UserService
	{

		private ILogger _logger;
		private readonly static Lazy<UserService> _instance = new Lazy<UserService>(() => new UserService());

		private UserService()
		{
		}

		private ICommonRepository<User> _repository;

		public static UserService Instance(ICommonRepository<User> repository, string loggerPath, Logger logger = null)
		{
			if (repository == null)
			{
				throw new Exception("Can not create UserService");
			}

			_instance.Value._repository = repository;
			_instance.Value._logger = logger ?? new Logger(loggerPath);
			return _instance.Value;
		}

		public User GetUserByCredential(string password, string login)
		{
			_logger.Trace(string.Format("Метод {0}. Система получает пользователя с паролем {1} и логином {2}", MethodBase.GetCurrentMethod().Name, password, login));
			return
				_repository.GetBySqlPredicate("SELECT * FROM Users as us WHERE us.Login = @p0 AND us.Password = @p1", login, password).FirstOrDefault();
		}

		public IEnumerable<User> GetUserByParams(string firstName, string lastName, string email, string login)
		{
			_logger.Trace(string.Format("Метод {0}. Система получает пользователей по следующим параметрам: Имя:{1};Фамилия:{2};Email:{3};Логин:{4};", MethodBase.GetCurrentMethod().Name, firstName, lastName, email, login));
			return
				_repository.GetBySqlPredicate("SELECT * FROM Users as us WHERE us.Login = @p0 OR us.Email = @p1 OR (us.FirstName = @p2 AND us.LastName = @p3)", login, email, firstName, lastName);
		}

		public void AddUser(User user)
		{
			_logger.Trace(string.Format("Метод {0}. Система добавила пользователя с email {1} и логином {2}",MethodBase.GetCurrentMethod().Name, user.Email, user.Credential.Login));
			_repository.Save(user);
		}

		public IEnumerable<User> GetAllUsers(int skip, int take, Func<User, object> func)
		{
			return _repository.GetAll(new QueryParams<User>(skip, take, func));
		}

		public IEnumerable<User> GetUserByPredicate(Expression<Func<User, bool>> predicate, QueryParams<User> pars)
		{
			return _repository.GetByPredicate(predicate, pars);
		}

		public User CurrentUser { get; set; }

		public void UpdateUser(User user)
		{
			_repository.Update(user);
		}

		public User GetUser(int id)
		{
			_logger.Trace(string.Format("Метод {0}. Система считывает данные о пользователе с ID = {1}", MethodBase.GetCurrentMethod().Name,id));
			return _repository.Read(id);
		}
		public void ValidateUserData(User old, User current)
		{
			if (old.FirstName == current.FirstName && old.LastName == current.LastName && old.Email == current.Email)
			{
				throw new Exception("Данные пользователя не изменились");
			}

			var any = _repository.GetBySqlPredicate("SELECT * FROM Users as us WHERE us.Email = @p0 AND us.UserId <> @p1", current.Email, old.UserId).Any();
			if (any)
			{
				throw new Exception("Такой email уже занят");
			}

			var any1 = _repository.GetBySqlPredicate("SELECT * FROM Users as us WHERE us.FirstName = @p0 AND  us.LastName = @p1 AND us.UserId <> @p2", current.FirstName, current.LastName, old.UserId).Any();
			if (any1)
			{
				throw new Exception("Пользователь с именем {0} и фамилией {1} уже существует".Fmt(current.FirstName, current.LastName));
			}
		}
	}
}
