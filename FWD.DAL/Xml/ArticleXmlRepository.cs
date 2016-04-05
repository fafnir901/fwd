using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Xml;
using FWD.CommonIterfaces;
using FWD.DAL.Helpers;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.DAL.Xml
{
	public class ArticleXmlRepository : BaseXml, ICommonRepository<Article>
	{

		public ArticleXmlRepository(string path = null, bool isForce = false)
			: base("Articles", path, isForce)
		{
		}
		public int Save(Article entity, bool now = true)
		{
			var xmlArticle = new XmlArticle
			{
				Article = entity
			};
			var duplicated = Document.Root.Elements().FirstOrDefault(c => c.Attribute("Link").Value == entity.Link);
			if (duplicated != null)
			{
				if (now)
				{
					throw new Exception("Найдена статья с таким же источником. Статья не сохранена.");
				}
				else
				{
					_builder.Append("Найдена повторяющиеся статья для \"{0}\". Статья не сохранена.".Fmt(duplicated.Attribute("ArticleName").Value));
				}
			}

			xmlArticle.Article.GroupId = xmlArticle.Article.ArticleGroup.GroupId;
			xmlArticle.Article.CreationDate = DateTime.Now;

			using (var stream = xmlArticle.Serialize())
			{
				stream.Position = 0;
				var currentDocument = XDocument.Load(stream);
				if (currentDocument.Root != null)
				{
					var element = currentDocument.Root.Elements().First();
					if (duplicated != null)
					{
						return -1;
					}
					else
					{
						AppendElement(element, "ArticleId", "ImageId");
						if (now)
						{
							Document.Save(Path);
						}
						var id = int.Parse(element.Attribute("ArticleId").Value);
						entity.ArticleId = id;
						return id;
					}
				}
				return -1;
			}
		}
		public IEnumerable<Article> SaveMany(IEnumerable<Article> entities)
		{
			_builder = new StringBuilder();
			foreach (var article in entities)
			{
				Save(article, false);
			}
			if (!string.IsNullOrEmpty(_builder.ToString()))
			{
				throw new Exception(_builder.ToString());
			}
			Document.Save(Path);
			return entities;
		}
		public Article Read(int id)
		{
			if (Document.Root.Elements().Any())
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("ArticleId").Value == id.ToString());
				if (res != null)
				{
					var attr = res.Attribute("CreationDate");
					if (string.IsNullOrEmpty(attr.Value))
					{
						attr.Value = default(DateTime).ToString();
					}
					var textReader = new StringReader(res.ToString());
					return XmlArticle.Deserialize(textReader);
				}
			}
			return null;
		}
		public async Task<Article> ReadAsync(int id)
		{
			return await Task.Factory.StartNew(() => Read(id));
		}

		public int Update(Article entity)
		{
			if (Document.Root.Elements().Any() && entity.ArticleId != 0)
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("Email").Value == entity.Link);
				if (res != null)
				{
					var innerDict = new Dictionary<string, string>();
					innerDict.Add("ArticleName", entity.ArticleName);
					innerDict.Add("InitialText", entity.InitialText);
					innerDict.Add("AuthorName", entity.AuthorName);
					innerDict.Add("Link", entity.Link);
					innerDict.Add("GroupId", entity.ArticleGroup.MaybeAs<ArticleGroup>().Bind(c=>c.GroupId).GetOrDefault(-1).ToString());
					SetAttributeValue(res, innerDict);

					if (res.Elements().Any())
					{
						foreach (var xElement in res.Elements())
						{
							foreach (var img in entity.Images)
							{
								var depperDict = new Dictionary<string, string>();
								depperDict.Add("Name", img.Name);
								depperDict.Add("Data", System.Text.Encoding.Default.GetString(img.Data));
								SetAttributeValue(xElement, depperDict);
							}
						}
					}
					else
					{
						foreach (var img in entity.Images)
						{
							var xElement = new XElement("Image");
							var depperDict = new Dictionary<string, string>();
							depperDict.Add("Name", img.Name);
							depperDict.Add("Data", System.Text.Encoding.Default.GetString(img.Data));
							SetAttributeValue(xElement, depperDict);
							res.Add(xElement);
						}
					}
				}
				Document.Save(Path);
			}
			return -1;
		}

		public void Delete(Article entity, bool now = true)
		{
			if (entity != null && entity.ArticleId != 0)
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("ArticleId").Value == entity.ArticleId.ToString());
				res.MaybeAs<XElement>().If(c => c != null).Do(c =>
				{
					res.Remove();
					if (now)
					{ Document.Save(Path); }
				});
			}
		}

		public void DeleteMany(IEnumerable<Article> entities)
		{
			foreach (var article in entities)
			{
				Delete(article, false);
			}
			Document.Save(Path);
		}

		public IEnumerable<Article> GetAll(QueryParams<Article> param)
		{
			if (Document.Root.Elements().Any())
			{
				param = QueryParams<Article>.Validate(param, c => c.ArticleId);
				var res = Document.Root.Elements().Skip(param.Skip ?? 0).Take(param.Take ?? 0);
				var list = res.Select(xElement => new StringReader(xElement.ToString())).Select(XmlArticle.Deserialize).ToList();
				return list.OrderBy(c => param.Order);
				//var context = new XmlParserContext(null, null, null, XmlSpace.None);
				//XmlReader reader = new XmlTextReader(res.ToString(), XmlNodeType.Element, context);
				//return XmlArticle.DeserializeEnumerable(reader);
			}
			return null;
		}

		public IEnumerable<Article> GetByPredicate(Expression<Func<Article, bool>> predicate, QueryParams<Article> param)
		{
			param = QueryParams<Article>.Validate(param, c => c.ArticleId);
			var set = predicate.GetSinglePredicateSet();
			var props = set.Property.Split("&&".ToCharArray()).Where(c => !string.IsNullOrEmpty(c)).ToArray();
			Func<XElement, bool> currentPredicate;
			Expression<Func<XElement, bool>> first;
			if (props.Count() > 1)
			{
				currentPredicate = c => c.Attribute(props[0]).Value.ApplyToString(set.Method, set.Value.Replace("\"", "")) ||
										c.Attribute(props[1]).Value.ApplyToString(set.Method, set.Value.Replace("\"", ""));

				//var collector = new ExpressionCollector<XElement>();

				//foreach (var prop in props)
				//{
				//	string prop1 = prop;
				//	var replaces = set.Value.Replace("\"", "");
				//	collector.AddExpression(c => c.Attribute(prop1).Value.ApplyToString(set.Method, replaces), ExpressionType.OrElse);
				//}
				//currentPredicate = collector.Collect().Compile();
			}
			else
			{
				currentPredicate = c => c.Attribute(props[0]).Value.ApplyToString(set.Method, set.Value.Replace("\"", ""));
			}
			var res = Document.Root.Elements().Where(c => currentPredicate.Invoke(c))
				.Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
			var list = res.Select(xElement => new StringReader(xElement.ToString())).Select(XmlArticle.Deserialize).ToList();
			return list.OrderBy(c => param.Order);
		}

		public IEnumerable<Article> GetBySqlPredicate(string sql, params object[] args)
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
				return Document.Root.Elements().Count();
			}
		}

		public string CurrentXmlPath
		{
			get
			{
				return Path;
			}
		}

		public string Export()
		{
			throw new NotImplementedException();
		}

		public string Export(int articleId)
		{
			throw new NotImplementedException();
		}

		public List<string> Import(bool isDb, byte[] data)
		{
			if (isDb)
			{
				throw new Exception("Выбран другой источник данных");
			}
			using (var stream = new MemoryStream(data))
			{
				var document = XDocument.Load(stream);
				MergeAndImport(document);
			}
			return null;
		}

		private void MergeAndImport(XDocument anotherDocument)
		{
			var anotherList = anotherDocument.Root.Elements().Select(xElement => new StringReader(xElement.ToString())).Select(XmlArticle.Deserialize).ToList();
			var currentList = Document.Root.Elements().Select(xElement => new StringReader(xElement.ToString())).Select(XmlArticle.Deserialize).ToList();
			foreach (Article article in anotherList)
			{
				Article match = currentList.FirstOrDefault(c => c.ArticleName == article.ArticleName || c.Link == article.Link);
				if (match == null)
				{
					Save(article, false);
				}
			}
			Document.Save(Path);
		}
	}
}
