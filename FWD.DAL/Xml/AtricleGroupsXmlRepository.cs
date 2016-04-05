using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Xml;
using FWD.CommonIterfaces;
using FWD.DAL.Helpers;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.DAL.Xml
{
	public class AtricleGroupsXmlRepository : BaseXml, ICommonRepository<ArticleGroup>
	{
		private readonly string _fileName = "Groups.xml";

		public override string FileName
		{
			get { return "Groups.xml"; }
		}

		public AtricleGroupsXmlRepository(string path = null,bool isForce = false)
			: base("ArticleGroup", path, isForce)
		{
			if (!Document.Root.Elements().Any())
			{
				var group = new ArticleGroup
				{
					GroupName = "Без группы"
				};
				Save(group);
			}
		}

		public int Save(ArticleGroup entity, bool now = true)
		{
			var xmlGroups = new XmlArtcileGroup()
			{
				ArticleGroup = entity
			};
			var duplicated = Document.Root.Elements().FirstOrDefault(c => c.Attribute("GroupName").Value == entity.GroupName);
			if (duplicated != null)
			{
				if (now)
				{
					throw new Exception("Группа с именем \"{0}\" уже существует".Fmt(entity.GroupName));
				}
				else
				{
					_builder.Append("Группа с именем \"{0}\" уже существует".Fmt(entity.GroupName));
				}
			}

			using (var stream = xmlGroups.Serialize())
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
						AppendElement(element, "GroupId", null);
						if (entity.Groups != null)
						{
							foreach (var @group in entity.Groups)
							{
								var innerElement = new XElement("Groups", @group);
								element.Add(innerElement);
							}
						}
						if (now)
						{
							Document.Save(Path);
						}
						var id = int.Parse(element.Attribute("GroupId").Value);
						entity.GroupId = id;
						return id;
					}
				}
				return -1;
			}
		}

		public IEnumerable<ArticleGroup> SaveMany(IEnumerable<ArticleGroup> entities)
		{
			_builder = new StringBuilder();
			foreach (var group in entities)
			{
				Save(group, false);
			}
			if (!string.IsNullOrEmpty(_builder.ToString()))
			{
				throw new Exception(_builder.ToString());
			}
			Document.Save(Path);
			return entities;
		}

		public ArticleGroup Read(int id)
		{
			if (Document.Root.Elements().Any())
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("GroupId").Value == id.ToString());
				if (res != null)
				{
					var textReader = new StringReader(res.ToString());
					return XmlArtcileGroup.Deserialize(textReader);
				}
			}
			return null;
		}

		public int Update(ArticleGroup entity)
		{
			if (Document.Root.Elements().Any() && entity.GroupId != 0)
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("GroupName").Value == entity.GroupName);
				if (res != null)
				{
					var innerDict = new Dictionary<string, string>();
					innerDict.Add("GroupName", entity.GroupName);
					SetAttributeValue(res, innerDict);
					if (res.Elements().Any())
					{
						if (!entity.Groups.Any())
						{
							res.Elements().Remove();
						}
						else
						{
							var notMatched = res.Elements().Where(c => !entity.Groups.Contains(c.Value));
							notMatched.Remove();
						}

						foreach (var group in entity.Groups)
						{
							var xElement = new XElement("Groups", group);
							if (res.Elements().FirstOrDefault(c => c.Value.Equals(xElement.Value)) == null)
							{
								var prevs = Document.Root.Elements().Where(c => c.Value.Equals(xElement.Value));
								prevs.Elements().Remove();
								res.Add(xElement);
							}
						}
					}
					else
					{
						foreach (var group in entity.Groups)
						{
							var xElement = new XElement("Groups", group);
							var prevs = Document.Root.Elements().Where(c => c.Value.Equals(xElement.Value));
							prevs.Elements().Remove();
							res.Add(xElement);
						}
					}
				}
				Document.Save(Path);
			}
			return -1;
		}

		public void Delete(ArticleGroup entity, bool now = true)
		{
			if (entity != null && entity.GroupId != 0)
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("GroupId").Value == entity.GroupId.ToString());
				res.MaybeAs<XElement>().If(c => c != null).Do(c =>
				{
					res.Remove();
					if (now)
					{ Document.Save(Path); }
				});
			}
		}

		public void DeleteMany(IEnumerable<ArticleGroup> entities)
		{
			foreach (var article in entities)
			{
				Delete(article, false);
			}
			Document.Save(Path);
		}

		public IEnumerable<ArticleGroup> GetAll(QueryParams<ArticleGroup> param)
		{
			if (Document.Root.Elements().Any())
			{
				param = QueryParams<ArticleGroup>.Validate(param, c => c.GroupId);
				var res = Document.Root.Elements().Skip(param.Skip ?? 0).Take(param.Take ?? 0);
				var list = res.Select(xElement => new StringReader(xElement.ToString())).Select(XmlArtcileGroup.Deserialize).ToList();
				return list.OrderBy(c => param.Order);
			}
			return null;
		}

		public IEnumerable<ArticleGroup> GetByPredicate(Expression<Func<ArticleGroup, bool>> predicate, QueryParams<ArticleGroup> param)
		{
			param = QueryParams<ArticleGroup>.Validate(param, c => c.GroupId);
			var set = predicate.GetSinglePredicateSet();
			var props = set.Property.Split("&&".ToCharArray()).Where(c => !string.IsNullOrEmpty(c)).ToArray();
			Func<XElement, bool> currentPredicate;
			if (props.Count() > 1)
			{
				currentPredicate = c => c.Attribute(props[0]).Value.ApplyToString(set.Method, set.Value.Replace("\"", "")) ||
										c.Attribute(props[1]).Value.ApplyToString(set.Method, set.Value.Replace("\"", ""));
			}
			else
			{
				currentPredicate = c => c.Attribute(props[0]).Value.ApplyToString(set.Method, set.Value.Replace("\"", ""));
				if (set.Method == "Contains" && set.Property == "Groups")
				{
					currentPredicate = c => c.Elements().Select(x=>x.Value).Contains(set.Value.Replace("\"", ""));
				}
			}
			var res = Document.Root.Elements().Where(c => currentPredicate.Invoke(c))
				.Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
			var list = res.Select(xElement => new StringReader(xElement.ToString())).Select(XmlArtcileGroup.Deserialize).ToList();
			return list.OrderBy(c => param.Order);
		}

		public IEnumerable<ArticleGroup> GetBySqlPredicate(string sql, params object[] args)
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
			throw new NotImplementedException();
		}
	}
}
