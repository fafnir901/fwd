using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects;
using Microsoft.SqlServer.Server;
using WebRock.Utils.UtilsEntities;

namespace FWD.CommonIterfaces
{
	public interface ICommonRepository<T> where T:class 
	{
		int Save(T entity, bool now = true);
		IEnumerable<T> SaveMany(IEnumerable<T> entities);
		T Read(int id);
		int Update(T entity);
		void Delete(T entity, bool now = true);
		void DeleteMany(IEnumerable<T> entities);
		IEnumerable<T> GetAll(QueryParams<T> param);
		IEnumerable<T> GetByPredicate(Expression<Func<T, bool>> predicate, QueryParams<T> param);

		IEnumerable<T> GetBySqlPredicate(string sql, params object[] args);

		IEnumerable<T1> GetBySqlPredicate<T1>(string sql, params object[] args);

		DataSet GetDataSetBySqlPredicate(string sql, params object[] args);

		int TotalCount { get; }

		string CurrentXmlPath { get; }

		string Export();

		string Export(int articleId);
		List<string> Import(bool isDb, byte[] data);
	}
}
