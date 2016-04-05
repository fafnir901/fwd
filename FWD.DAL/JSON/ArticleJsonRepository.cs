using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using Newtonsoft.Json;
using WebRock.Utils.UtilsEntities;

namespace FWD.DAL.JSON
{
	public class ArticleJsonRepository : BaseJson<Article>, ICommonRepository<Article>
	{
		public int Save(Article entity, bool now = true)
		{
			var existing = DeserializeFromFile();
			var maxId = existing == null ? 0 : existing.Max(c => c.ArticleId);
			entity.ArticleId = maxId == 0 ? 1 : maxId + 1;
			if (existing != null)
			{
				var existingLink = existing.FirstOrDefault(c => c.Link == entity.Link);
				if (existingLink != null)
				{
					throw new Exception("Статья с ссылкой \"{0}\" уже существует".Fmt(entity.Link));
				}
			}
			else
			{
				existing = new List<Article>();
			}
			existing.Add(entity);
			System.IO.File.WriteAllText(Path, JsonConvert.SerializeObject(existing));
			return entity.ArticleId;
		}

		public IEnumerable<Article> SaveMany(IEnumerable<Article> entities)
		{
			throw new NotImplementedException();
		}

		public Article Read(int id)
		{
			throw new NotImplementedException();
		}

		public int Update(Article entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(Article entity, bool now = true)
		{
			throw new NotImplementedException();
		}

		public void DeleteMany(IEnumerable<Article> entities)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Article> GetAll(QueryParams<Article> param)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Article> GetByPredicate(Expression<Func<Article, bool>> predicate, QueryParams<Article> param)
		{
			throw new NotImplementedException();
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
			get { throw new NotImplementedException(); }
		}

		public string CurrentXmlPath
		{
			get { throw new NotImplementedException(); }
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
