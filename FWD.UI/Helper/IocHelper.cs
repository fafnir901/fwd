using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using FWD.Services;
using FWD.UI.Ioc;
using Ninject;
using Ninject.Parameters;

namespace FWD.UI.Helper
{
	public static class IocHelper
	{
		static IocHelper()
		{
			_appKernel = new StandardKernel(new ArticleDbModule());
		}
		private static IKernel _appKernel;

		public static IKernel AppKernel {
			get
			{
				return _appKernel;
			}
		}

		private static ArticleService _service;
		private static PlanService _pService;
		private static GroupService _groupService;
		private static UserService _userService;

		public static UserService UserService
		{
			get
			{
				ToggleToUserDb();
				if (_userService == null)
				{
					var rep = AppKernel.Get<ICommonRepository<User>>();
					_userService = UserService.Instance(rep, "");
				}
				return _userService;
			}
		}

		public  static ArticleService ArticleService
		{
			get
			{
				if (_service == null)
				{
					var rep = AppKernel.Get<ICommonRepository<Article>>();
					_service = new ArticleService(rep,"");
				}
				return _service;
			}
		}

		public static ArticleHelperService ArticleHelperService {
			get
			{
				return ArticleHelperService.Instance;
			}
		}

		public static PlanService PlanService
		{
			get
			{
				ToggleToPlan();
				if (_pService == null)
				{
					var rep = AppKernel.Get<ICommonRepository<IPlan>>();
					_pService = new PlanService(rep);
				}
				return _pService;
			}
		}

		public static GroupService GroupService
		{
			get
			{
				if (_groupService == null)
				{
					ToggleToGroup();
					var rep = AppKernel.Get<ICommonRepository<ArticleGroup>>();
					_groupService = new GroupService(rep, _service);
				}
				return _groupService;
			}
		}

		public static void ToggleToXml()
		{
			_appKernel = new StandardKernel(new ArticleXmlModule());
			_service = null;
		}

		public static void ToggleToDb()
		{
			_appKernel = new StandardKernel(new ArticleDbModule());
			_service = null;
		}

		public static void ToggleToPlan()
		{
			_appKernel = new StandardKernel(new PlanXmlModule());
		}

		public static void ToggleToGroup()
		{
			_appKernel = new StandardKernel(new ArticleGroupXmlModule());
		}

		public static void ToggleToUserDb()
		{
			_appKernel = new StandardKernel(new UserDbModule());
		}
	}
}
