using System.Configuration;
using System.Web;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using FWD.Services;
using FWD.UI.Web.Models.Ioc;
using Ninject;

namespace FWD.UI.Web.Models.Helper
{
	public class IocHelper
	{
		private static IKernel _appKernel;
		public IKernel AppKernel
		{
			get
			{
				if (_appKernel == null)
				{
					_appKernel = new StandardKernel();
					_appKernel.Load(new ArticleDbModule());
					_appKernel.Load(new TransactionDbModule());

					_appKernel.Load(new ArticleGroupModule());
					_appKernel.Load(new UserDbModule());
					_appKernel.Load(new CommentDbModule());
					_appKernel.Load(new PlanDbModule());
					_appKernel.Load(new TagDbModule());
				}
				return _appKernel;
			}
		}

		public static string GetCurrentDirectory()
		{
			return ConfigurationManager.AppSettings["path"];
		}

		public static bool IsForcePath()
		{
			var res = ConfigurationManager.AppSettings["forcePath"] ?? string.Empty;
			return res != string.Empty && bool.Parse(res);
		}

		private ArticleService _service;
		private PlanService _pService;
		private GroupService _groupService;
		private CommentService _commentService;
		private UserService _userService;
		private TagService _tagService;

		private FWD.Services.TransactionService _tService;

		private static string DefaultToggler { get; set; }
		public static string CurrentToggle
		{
			get
			{
				if (HttpContext.Current.Session["CurrentToggle"] != null)
				{
					return HttpContext.Current.Session["CurrentToggle"].ToString();
				}
				return DefaultToggler;
			}
			set { HttpContext.Current.Session["CurrentToggle"] = value; }
		}

		static IocHelper()
		{
			DefaultToggler = ConfigurationManager.AppSettings["defaultToggler"];
		}
		public ArticleService ArticleService
		{
			get
			{
				if (_service == null)
				{
					switch (CurrentToggle)
					{
						case "db":
							ToggleToDb();
							break;
						default:
							ToggleToXml();
							break;
					}
					var rep = AppKernel.Get<ICommonRepository<Article>>(); ;
					_service = new ArticleService(rep, GetCurrentDirectory(), currentUser:CommonHelper.Instance.CurrentUser);
				}
				return _service;
			}
		}

		public TagService TagService
		{
			get
			{
				if (_tagService == null)
				{
					var rep = AppKernel.Get<ICommonRepository<ITag>>();
					_tagService = new TagService(rep, GetCurrentDirectory(), currentUser:CommonHelper.Instance.CurrentUser);
				}
				return _tagService;
			}
		}

		public CommentService CommentService
		{
			get
			{
				if (_commentService == null)
				{
					var rep = AppKernel.Get<ICommonRepository<Comment>>(); ;
					_commentService = CommentService.Instance(rep);
				}
				return _commentService;
			}
		}

		public UserService UserService
		{
			get
			{
				if (_userService == null)
				{
					var rep = AppKernel.Get<ICommonRepository<User>>(); ;
					_userService = UserService.Instance(rep, GetCurrentDirectory());
				}
				return _userService;
			}
		}

		public TransactionService TransactionService
		{
			get
			{
				if (_tService == null)
				{
					var rep = AppKernel.Get<ICommonRepository<Transaction>>(); ;
					_tService = new TransactionService(rep);
				}
				return _tService;
			}
		}

		public ArticleHelperService ArticleHelperService
		{
			get
			{
				return ArticleHelperService.Instance;
			}
		}

		public PlanService PlanService
		{
			get
			{
				if (_pService == null)
				{
					switch (CurrentToggle)
					{
						case "db":
							ToggleToDb();
							break;
						default:
							ToggleToXml();
							break;
					}
					var rep = AppKernel.Get<ICommonRepository<IPlan>>(); ;
					_pService = new PlanService(rep, CommonHelper.Instance.CurrentUser);
				}
				return _pService;
			}
		}

		public GroupService GroupService
		{
			get
			{
				if (_groupService == null)
				{
					switch (CurrentToggle)
					{
						case "db":
							ToggleToDb();
							break;
						default:
							ToggleToXml();
							break;
					}
					var rep = AppKernel.Get<ICommonRepository<ArticleGroup>>();
					_groupService = new GroupService(rep, _service, CommonHelper.Instance.CurrentUser);
				}
				return _groupService;
			}
		}

		public void ToggleToXml(bool needOverrideToggle = false)
		{
			if (needOverrideToggle)
			{
				CurrentToggle = "xml";
			}
			if (!AppKernel.HasModule(typeof (ArticleXmlModule).FullName))
			{
				AppKernel.Load(new ArticleXmlModule());
				AppKernel.Unload(typeof(ArticleDbModule).FullName);
			}
			if (!AppKernel.HasModule(typeof (ArticleGroupXmlModule).FullName))
			{
				AppKernel.Load(new ArticleGroupXmlModule());
				AppKernel.Unload(typeof(ArticleGroupModule).FullName);
			}


			if (AppKernel.HasModule(typeof (ArticleGroupXmlModule).FullName) &&
			    AppKernel.HasModule(typeof (ArticleDbModule).FullName))
			{
				//AppKernel.Load(new ArticleXmlModule());
				AppKernel.Unload(typeof(ArticleDbModule).FullName);
			}

			if (!AppKernel.HasModule(typeof(PlanXmlModule).FullName))
			{
				AppKernel.Load(new PlanXmlModule());
				AppKernel.Unload(typeof(PlanDbModule).FullName);
			}
			_service = null;
		}


		public void ToggleToDb(bool needOverrideToggle = false)
		{
			if (needOverrideToggle)
			{
				CurrentToggle = "db";
			}
			if (!AppKernel.HasModule(typeof(ArticleDbModule).FullName))
			{
				AppKernel.Load(new ArticleDbModule());
				AppKernel.Unload(typeof(ArticleXmlModule).FullName);
			}
			if (!AppKernel.HasModule(typeof(ArticleGroupModule).FullName))
			{
				AppKernel.Load(new ArticleGroupModule());
				AppKernel.Unload(typeof(ArticleGroupXmlModule).FullName);
			}
			if (!AppKernel.HasModule(typeof(PlanDbModule).FullName))
			{
				AppKernel.Load(new PlanDbModule());
				AppKernel.Unload(typeof(PlanXmlModule).FullName);
			}
			_service = null;
		}
	}
}