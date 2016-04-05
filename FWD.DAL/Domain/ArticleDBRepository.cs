using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Xml;
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
using ArticleGroup = FWD.BusinessObjects.Domain.ArticleGroup;
using Tag = FWD.BusinessObjects.Domain.Tag;
using User = FWD.BusinessObjects.Domain.User;

namespace FWD.DAL.Domain
{
	public class ArticleDBRepository : BaseDB, ICommonRepository<Article>, ISpecifiedArticleRepository, IManagmentEntityWithUser
	{
		protected SimpleMapper<Article, Entities.Article> mapper;
		private Entities.User _currentUser;

		public ArticleDBRepository()
		{
			//Container = new ArticleContext();
			Container = null;
			ConfigureToBo();
			ConfigureToDb();
			mapper = new SimpleMapper<Article, Entities.Article>();
			mapper.AddMapping(c => c.ArticleId, c => c.ArticleId);
			mapper.AddMapping(c => c.ArticleName, c => c.ArticleName);
			mapper.AddMapping(c => c.InitialText, c => c.InitialText);
			mapper.AddMapping(c => c.Link, c => c.Link);
			mapper.AddMapping(c => c.AuthorName, c => c.AuthorName);
			mapper.AddMapping(c => c.Images, c => c.EmbdedImages);
			mapper.AddMapping(c => c.Tags, c => c.Tags);
		}
		public int Save(Article entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					entity.Tags = entity.Tags
						.GroupBy(c => c.Name)
						.Select(c => c.First())
						.ToList();

					var artDb = Mapper.Map<Article, Entities.Article>(entity);

					if (Container.Articles.FirstOrDefault(c => c.Link == artDb.Link) != null)
					{
						throw new Exception("Найдена статья с таким же источником. Статья не сохранена.");
					}

					if (artDb.Group == null && entity.GroupId == -1)
					{
						artDb.Group = Container.Groups.First(c => c.GroupId == 1);
					}
					else
					{
						var groupName =
							entity.ArticleGroup.MaybeAs<BusinessObjects.Domain.ArticleGroup>()
							.Bind(c => c.GroupName)
							.GetOrDefault(string.Empty);

						var existingGroup = Container.Groups
							.FirstOrDefault(c => c.GroupName == groupName || c.GroupId == entity.GroupId);
						artDb.Group = existingGroup ?? artDb.Group;
					}

					if (entity.Tags != null)
					{
						var currentTagsIDs = entity.Tags
							.Select(x => x.Id)
							.ToList();

						artDb.Tags = Container.Tags
							.Where(c => currentTagsIDs.Contains(c.Id))
							.ToList();
					}

					foreach (var embdedImage in artDb.EmbdedImages)
					{
						TransactionHelper.AddTransaction(Container, ActionType.Adding, embdedImage, _currentUser);
					}

					artDb.CreationDate = DateTime.Now;
					Container.Articles.Add(artDb);
					if (now)
					{
						Container.SaveChanges();
					}
					entity.ArticleId = artDb.ArticleId;
					if (entity.Images != null && entity.Images.Any())
					{
						var ids = artDb.EmbdedImages
							.Select(c => new { Name = c.Name, Id = c.ImageId });

						entity.Images.Zip(ids, (a, b) =>
						{
							a.ImageId = b.Id;
							return a;
						});
					}
					TransactionHelper.AddTransaction(Container, ActionType.Adding, artDb, _currentUser);
					if (now)
					{
						Container.SaveChanges();
					}
					return artDb.ArticleId;
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

		public IEnumerable<Article> SaveMany(IEnumerable<Article> entities)
		{
			try
			{
				var builder = new StringBuilder();
				using (Container = new ArticleContext())
				{
					var list = new List<Entities.Article>();
					foreach (var article in entities)
					{
						var duplicated = Container.Articles.FirstOrDefault(c => c.Link == article.Link);
						if (duplicated == null)
						{
							var artDb = Mapper.Map<Article, Entities.Article>(article);

							var group = artDb.Group ?? Container.Groups.
								FirstOrDefault(c => c.GroupId == article.GroupId);
							artDb.Group = group ?? Container.Groups.First();

							if (article.Tags != null && article.Tags.Count > 0)
							{
								var currentTagsIDs = article.Tags
									.Select(x => x.Name)
									.ToList();

								artDb.Tags = Container.Tags
									.Where(c => currentTagsIDs.Contains(c.Name))
									.ToList();
							}

							artDb.CreationDate = DateTime.Now;
							list.Add(artDb);
							TransactionHelper.AddTransaction(Container, ActionType.Adding, artDb, _currentUser);
							foreach (var embdedImage in artDb.EmbdedImages)
							{
								TransactionHelper.AddTransaction(Container, ActionType.Adding, embdedImage, _currentUser);
							}
							Container.Articles.Add(artDb);
						}
						else
						{
							builder.Append("Найдена повторяющиеся статья для \"{0}\". Статья не сохранена.".Fmt(duplicated.ArticleName));
						}

					}
					Container.SaveChanges();
					if (!string.IsNullOrEmpty(builder.ToString()))
					{
						throw new Exception(builder.ToString());
					}
					return list.Zip(entities, (db, bo) => new Article
					{
						ArticleId = db.ArticleId,
						InitialText = db.InitialText,
						Link = db.Link,
						ArticleName = db.ArticleName,
						AuthorName = db.AuthorName,
						Images = Mapper.Map<IEnumerable<EmbdedImage>, List<Image>>(db.EmbdedImages),
						Rate = db.Rate,
						CreationDate = db.CreationDate,
						GroupId = db.Group.GroupId,
						//ArticleGroup = Mapper.Map<Entities.ArticleGroup, ArticleGroup>(db.Group),
						//Tags = Mapper.Map<IEnumerable<Entities.Tag>, List<Tag>>(db.Tags),
					});
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

		public Article Read(int id)
		{
			try
			{
				if (id <= 0)
				{
					return null;
				}
				using (Container = new ArticleContext())
				{
					var artDb = Container.Articles.FirstOrNothing(c => c.ArticleId == id);
					if (artDb.HasValue)
					{
						TransactionHelper.AddTransaction(Container, ActionType.Read, artDb.Value, _currentUser);
						Container.SaveChanges();
					}
					return artDb.Bind(Mapper.Map<Entities.Article, Article>).GetOrDefault(null);
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

		public int Update(Article entity)
		{
			try
			{
				if (entity == null || entity.ArticleId == 0)
				{
					return -1;
				}
				using (Container = new ArticleContext())
				{
					var fromDb = Container.Articles.FirstOrDefault(c => c.ArticleId == entity.ArticleId);
					var groupName =
							entity.ArticleGroup.MaybeAs<BusinessObjects.Domain.ArticleGroup>().Bind(c => c.GroupName).GetOrDefault(string.Empty);

					var existingGroup = Container.Groups.FirstOrDefault(c => c.GroupName == groupName);
					fromDb.ArticleName = entity.ArticleName;
					fromDb.InitialText = entity.InitialText;
					fromDb.Link = entity.Link;
					fromDb.AuthorName = entity.AuthorName;
					fromDb.Group = existingGroup ?? Mapper.Map<Entities.ArticleGroup>(entity.ArticleGroup);
					fromDb.Rate = entity.Rate;


					if (entity.Tags != null)
					{
						var currentTags = Mapper.Map<IEnumerable<FWD.BusinessObjects.Domain.Tag>, IEnumerable<Entities.Tag>>(entity.Tags).ToList();
						var matched = (currentTags.Select(
						tag => new { tag, matched = fromDb.Tags.FirstOrDefault(c => c.Name == tag.Name) })
						.Where(@t => @t.matched != null)
						.Select(@t => @t.tag)).ToList();

						if (fromDb.Tags.Count > currentTags.Count)
						{
							var removeList = fromDb.Tags
								.Select(tag => new { tag, res = currentTags.FirstOrDefault(c => c.Id == tag.Id) })
								.Where(@t => @t.res == null)
								.Select(@t => @t.tag)
								.ToList();

							foreach (var tag in removeList)
							{
								fromDb.Tags.Remove(tag);
							}
						}
						foreach (var tag in matched)
						{
							currentTags.Remove(tag);
						}
						foreach (var tag in currentTags)
						{
							var fromDbTag = Container.Tags.FirstOrDefault(c => c.Id == tag.Id);
							fromDb.Tags.Add(fromDbTag);
						}
					}


					TransactionHelper.AddTransaction(Container, ActionType.Updating, fromDb, _currentUser);

					var embdedImage = Mapper.Map<IEnumerable<Image>, IEnumerable<EmbdedImage>>(entity.Images).ToList();
					var lst = (embdedImage.Select(
						image => new { image, matched = fromDb.EmbdedImages.FirstOrDefault(c => c.Name == image.Name) })
						.Where(@t => @t.matched != null)
						.Select(@t => @t.image)).ToList();
					if (embdedImage.Count < fromDb.EmbdedImages.Count)
					{
						var removeList = fromDb.EmbdedImages
							.Select(image => new { image, res = embdedImage.FirstOrDefault(c => c.ImageId == image.ImageId) })
							.Where(@t => @t.res == null)
							.Select(@t => @t.image)
							.ToList();

						foreach (var image in removeList)
						{
							TransactionHelper.AddTransaction(Container, ActionType.Deleting, image, _currentUser);
							fromDb.EmbdedImages.Remove(image);
							Container.EmbdedImages.Remove(image);
						}
					}
					foreach (var image in lst)
					{
						embdedImage.Remove(image);
					}
					foreach (var image in embdedImage)
					{
						TransactionHelper.AddTransaction(Container, ActionType.Adding, image, _currentUser);
						fromDb.EmbdedImages.Add(image);
					}
					Container.SaveChanges();
					return fromDb.ArticleId;
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

		public void Delete(Article entity, bool now = true)
		{
			try
			{
				if (entity == null)
				{
					return;
				}
				using (Container = new ArticleContext())
				{
					var artDb = Container.Articles.FirstOrNothing(c => c.ArticleId == entity.ArticleId);
					if (artDb != Maybe.Nothing)
					{
						var deletingList = artDb.Value.EmbdedImages.Where(embdedImage => embdedImage != null).ToList();
						foreach (var embdedImage in deletingList)
						{
							TransactionHelper.AddTransaction(Container, ActionType.Deleting, embdedImage, _currentUser);
							Container.EmbdedImages.Remove(embdedImage);
						}
						TransactionHelper.AddTransaction(Container, ActionType.Deleting, artDb.Value, _currentUser);
						Container.Articles.Remove(artDb.Value);
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

		public void DeleteMany(IEnumerable<Article> entities)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					foreach (var article in entities)
					{
						var artDb = Container.Articles.FirstOrNothing(c => c.ArticleId == article.ArticleId);
						if (artDb != Maybe.Nothing)
						{
							var deletingList = artDb.Value.EmbdedImages.Where(embdedImage => embdedImage != null).ToList();
							foreach (var embdedImage in deletingList)
							{
								TransactionHelper.AddTransaction(Container, ActionType.Deleting, embdedImage, _currentUser);
								Container.EmbdedImages.Remove(embdedImage);
							}
							TransactionHelper.AddTransaction(Container, ActionType.Deleting, artDb.Value, _currentUser);
							Container.Articles.Remove(artDb.Value);
						}
					}
					Container.SaveChanges();
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

		public IEnumerable<Article> GetAll(QueryParams<Article> param)
		{
			try
			{
				param = QueryParams<Article>.Validate(param, c => c.ArticleId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Articles.OrderBy(c => c.ArticleId).Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
					var result = Mapper.Map<IEnumerable<Entities.Article>, IEnumerable<Article>>(list);
					var enumerable = result as IList<Article> ?? result.ToList();
					foreach (var article in enumerable)
					{
						article.GroupId = article.ArticleGroup.GroupId;
					}
					return enumerable.OrderBy(param.Order);
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

		public IEnumerable<Article> GetByPredicate(Expression<Func<Article, bool>> predicate, QueryParams<Article> param)
		{
			try
			{
				//var currentPredicate = predicate.Convert<Entities.Article, Article>(mapper);
				var anotherPredicate = Mapper.Map<Expression<Func<Article, bool>>, Expression<Func<Entities.Article, bool>>>(predicate);
				param = QueryParams<Article>.Validate(param, c => c.ArticleId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Articles
						.OrderBy(c => c.ArticleId)
						.Where(anotherPredicate)
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<object>, IEnumerable<Article>>(list);
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

		public IEnumerable<Article> GetWithTags(string searchString, QueryParams<Article> param)
		{
			try
			{
				param = QueryParams<Article>.Validate(param, c => c.ArticleId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Articles.OrderBy(c => c.ArticleId)
						.Where(c => c.ArticleName.ToUpper().Contains(searchString.ToUpper())
							|| c.AuthorName.ToUpper().Contains(searchString.ToUpper())
							|| c.InitialText.ToUpper().Contains(searchString.ToUpper())
							|| c.Tags.FirstOrDefault(x => x.Name.ToUpper().Contains(searchString.ToUpper())) != null)
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Entities.Article>, IEnumerable<Article>>(list);
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

		public IEnumerable<Article> GetByTagWithTags(IEnumerable<int> ids, QueryParams<Article> param)
		{
			try
			{
				param = QueryParams<Article>.Validate(param, c => c.ArticleId, 10);
				using (Container = new ArticleContext())
				{
					var list = Container.Articles
						.OrderBy(c => c.ArticleId)
						.Where(c => c.Tags.Count > 0 && !ids.Except(c.Tags.Select(x => x.Id)).Any())
						.Skip(param.Skip ?? 0)
						.Take(param.Take ?? 0)
						.ToList();
					var result = Mapper.Map<IEnumerable<Entities.Article>, IEnumerable<Article>>(list);
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

		public IEnumerable<Article> GetBySqlPredicate(string sql, params object[] args)
		{
			try
			{
				using (Container = new ArticleContext())
				{
					return Container.Database.SqlQuery<Entities.Article>(sql, args)
						.ToList()
						.MaybeAs<List<Entities.Article>>()
						.Bind(Mapper.Map<List<Entities.Article>, List<Article>>)
						.GetOrDefault(new List<Article>());
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
			using (Container = new ArticleContext())
			{
				var retVal = new DataSet();
				var sqlConn = Container.Database.Connection as SqlConnection;
				var sqlQuery = new SqlCommand(sql, sqlConn) { CommandType = CommandType.Text };
				var adapter = new SqlDataAdapter(sqlQuery);
				using (sqlQuery)
				{
					if (args.Any())
					{
						foreach (var o in args)
						{
							var param = new SqlParameter(o.ToString(), o);
							sqlQuery.Parameters.Add(param);
						}
					}
					adapter.Fill(retVal);
				}
				return retVal;
			}

		}

		public int TotalCount
		{
			get
			{
				return Container.Articles.Count();
			}
		}

		public void UsingTransaction(Action action)
		{
			using (var scope = new TransactionScope())
			{
				action();
				scope.Complete();
			}
		}

		public string CurrentXmlPath
		{
			get
			{
				return null;
			}
		}

		public override string Export()
		{
			using (Container = new ArticleContext())
			{
				TransactionHelper.AddTransaction(Container, ActionType.Export, null, _currentUser);
				Container.SaveChanges();
			}

			var all = GetAll(new QueryParams<Article>(0, 999, c => c.ArticleId));
			var allXml = all.Select(c => new XmlArticle { Article = c }).OrderBy(c => c.Article.ArticleId).ToList();
			using (var stream = XmlArticle.Serialize(allXml.GetType(), allXml))
			{
				stream.Position = 0;
				var document = new XDocument(new XElement("Articles"));
				var anotherDocument = XDocument.Load(stream);
				foreach (var xNode in anotherDocument.Root.Elements().Select(c => c.Elements().First()))
				{
					document.Root.Add(xNode);
				}
				return document.ToString();
			}
		}

		public string Export(int articleId)
		{
			Article article = null;
			using (Container = new ArticleContext())
			{
				article = Container.Articles.FirstOrNothing(c => c.ArticleId == articleId).Bind(Mapper.Map<Entities.Article, Article>).GetOrDefault(null);
				TransactionHelper.AddTransaction(Container, ActionType.Export, null, _currentUser);
				Container.SaveChanges();
			}

			var xmlAricle = new XmlArticle
			{
				Article = article
			};
			using (var stream = XmlArticle.Serialize(xmlAricle.GetType(), xmlAricle))
			{
				stream.Position = 0;
				var document = new XDocument(new XElement("Articles"));
				var anotherDocument = XDocument.Load(stream);
				document.Root.Add(anotherDocument.Root.Elements().First());
				return document.ToString();
			}
		}

		public List<string> Import(bool isDb, byte[] data)
		{
			if (!isDb)
			{
				throw new Exception("Выбран другой источник данных");
			}

			using (Container = new ArticleContext())
			{
				TransactionHelper.AddTransaction(Container, ActionType.Import, null, _currentUser);
				Container.SaveChanges();
			}


			using (var stream = new MemoryStream(data))
			{
				var document = XDocument.Load(stream);
				return MergeAndImport(document);
			}
		}

		private List<string> MergeAndImport(XDocument anotherDocument)
		{
			var unMatchedlist = new List<string>();
			var matchedList = new List<Article>();
			var currentList = GetAll(new QueryParams<Article>(0, 999, c => c.ArticleName)).ToList();
			using (Container = new ArticleContext())
			{
				var anotherList = new List<Article>();
				//var anotherList = anotherDocument.Root.Elements().Select(xElement => new StringReader(xElement.ToString())).Select(XmlArticle.Deserialize).ToList();
				XElement element = new XElement("Article");
				try
				{
					foreach (var xElement in anotherDocument.Root.Elements())
					{
						element = xElement;
						var stringReader = new StringReader(xElement.ToString());
						var article = XmlArticle.Deserialize(stringReader);
						anotherList.Add(article);
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("Статья {0} - не читается", element.Attribute("ArticleName")), ex);
				}


				foreach (Article article in anotherList)
				{
					Article match = currentList.FirstOrDefault(c => c.ArticleName == article.ArticleName || c.Link == article.Link);
					if (match == null)
					{
						matchedList.Add(article);
					}
					else
					{
						unMatchedlist.Add(string.Format("Статья '{0}' обнаружена в базе и не будет сохранена", match.ArticleName));
					}
				}

				SaveMany(matchedList);
				return unMatchedlist;
			}
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
