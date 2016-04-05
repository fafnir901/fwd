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
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.BusinessObjects.Xml;
using FWD.CommonIterfaces;
using FWD.DAL.Helpers;
using WebRock.Utils.Monad;
using WebRock.Utils.UtilsEntities;

namespace FWD.DAL.Xml
{
	public class PlanXmlRepository : BaseXml, ICommonRepository<IPlan>
	{
		public override string FileName
		{
			get { return "plan.xml"; }
		}

		public PlanXmlRepository(string path=null,bool isForce = false)
			: base("Plans", path, isForce)
		{
		}

		public int Save(IPlan entity, bool now = true)
		{
			var current = entity as Plan;
			current.ChangedData = entity.PossibleChangeDate == null ? default(DateTime) : entity.PossibleChangeDate.Value;
			var plan = new XmlPlan()
			{
				Plan = current
			};

			using (var stream = plan.Serialize())
			{
				stream.Position = 0;
				var currentDocument = XDocument.Load(stream);
				if (currentDocument.Root != null)
				{
					var element = currentDocument.Root.Elements().First();

					AppendElement(element, "Id", null);
					if (now)
					{
						Document.Save(Path);
					}
					var id = int.Parse(element.Attribute("Id").Value);
					entity.Id = id;
					return id;
				}
				return -1;
			}
		}

		public IEnumerable<IPlan> SaveMany(IEnumerable<IPlan> entities)
		{
			_builder = new StringBuilder();
			var lst = new List<Plan>();
			foreach (var plan in entities)
			{
				var res = plan.Clone();
				Save(res as Plan, false);
				lst.Add(res as Plan);
			}
			Document.Save(Path);
			if (!string.IsNullOrEmpty(_builder.ToString()))
			{
				throw new Exception(_builder.ToString());
			}
			return lst;
		}

		public IPlan Read(int id)
		{
			if (Document.Root.Elements().Any())
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("Id").Value == id.ToString());
				if (res != null)
				{
					var textReader = new StringReader(res.ToString());
					var plan =  XmlPlan.Deserialize(textReader);
					DateTime? current;
					if (plan.ChangedData == default(DateTime))
					{
						current = null;
					}
					else
					{
						current = plan.ChangedData;
					}
					plan.PossibleChangeDate = current;
					return plan;
				}
			}
			return null;
		}

		public Task<Plan> ReadAsync(int id)
		{
			throw new NotImplementedException();
		}

		public int Update(IPlan entity)
		{
			if (Document.Root.Elements().Any() && entity.Id != 0)
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("Name").Value == entity.Name);
				if (res != null)
				{
					var innerDict = new Dictionary<string, string>();
					innerDict.Add("Description", entity.Description);

					var current = entity as Plan;
					current.ChangedData = entity.PossibleChangeDate == null ? default(DateTime) : current.PossibleChangeDate.Value;

					innerDict.Add("ChangedData", XmlConvert.ToString(current.ChangedData));
					innerDict.Add("IsDone", XmlConvert.ToString(entity.IsDone));
					SetAttributeValue(res, innerDict);

				}
				Document.Save(Path);
				return res == null ? -1 : int.Parse(res.Attribute("Id").Value);
			}
			return -1;
		}

		public void Delete(IPlan entity, bool now = true)
		{
			if (entity != null && entity.Id != 0)
			{
				var res = Document.Root.Elements().FirstOrDefault(c => c.Attribute("Id").Value == entity.Id.ToString());
				res.MaybeAs<XElement>().If(c => c != null).Do(c =>
				{
					res.Remove();
					if (now)
					{ Document.Save(Path); }
				});
			}
		}

		public void DeleteMany(IEnumerable<IPlan> entities)
		{
			foreach (var plan in entities)
			{
				Delete(plan, false);
			}
			Document.Save(Path);
		}

		public IEnumerable<IPlan> GetAll(QueryParams<IPlan> param)
		{
			Document = XDocument.Load(Path);
			if (Document.Root.Elements().Any())
			{
				param = QueryParams<IPlan>.Validate(param, c => c.Id);
				var res = Document.Root.Elements().Skip(param.Skip ?? 0).Take(param.Take ?? 0);
				var list = res.Select(xElement => new StringReader(xElement.ToString())).Select(XmlPlan.Deserialize).ToList();
				return list.OrderBy(c => param.Order);
			}
			return null;
		}

		public IEnumerable<IPlan> GetByPredicate(Expression<Func<IPlan, bool>> predicate, QueryParams<IPlan> param)
		{
			param = QueryParams<IPlan>.Validate(param, c => c.Id);
			var set = predicate.GetSinglePredicateSet();
			var props = set.Property.Split("&&".ToCharArray()).Where(c => !string.IsNullOrEmpty(c)).ToArray();
			Func<XElement, bool> currentPredicate;
			var value = set.Value.Replace("\"", "");
			if (value == "True" || value == "False")
			{
				value = value.ToLower();
			}
			if (props.Count() > 1)
			{
				currentPredicate = c => c.Attribute(props[0]).Value.ApplyToString(set.Method, value) ||
										c.Attribute(props[1]).Value.ApplyToString(set.Method, value);
			}
			else
			{
				currentPredicate = c => c.Attribute(props[0]).Value.ApplyToString(set.Method, value);
			}
			var res = Document.Root.Elements().Where(c => currentPredicate.Invoke(c))
				.Skip(param.Skip ?? 0).Take(param.Take ?? 0).ToList();
			var list = res.Select(xElement => new StringReader(xElement.ToString())).Select(XmlPlan.Deserialize).ToList();
			return list.OrderBy(c => param.Order);
		}

		public IEnumerable<IPlan> GetBySqlPredicate(string sql, params object[] args)
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

		public string CurrentXmlPath {
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
