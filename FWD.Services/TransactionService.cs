using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using Newtonsoft.Json;
using WebRock.Utils.UtilsEntities;

namespace FWD.Services
{
	public class TransactionService
	{
		private ICommonRepository<Transaction> _repository;

		public TransactionService(ICommonRepository<Transaction> repository)
		{
			_repository = repository;
		}

		public IEnumerable<Transaction> GetAllTransactions(Func<Transaction, object> order, int? skip = 0, int? take = 999)
		{
			var queryParams = new QueryParams<Transaction>
			{
				Skip = skip,
				Take = take,
				Order = order
			};
			return _repository.GetAll(queryParams);
		}

		public Transaction GetLastTransaction()
		{
			var sql = "select top 1 * from [TransactionLogs] tLog where tLog.EntityType = 1 order by tLog.Id desc";
			return _repository.GetBySqlPredicate(sql).FirstOrDefault();
		}

		public Dictionary<string, string> GetLastParameters()
		{
			Dictionary<string, string> dict = null;
			var lastTransaction = GetLastTransaction();
			if (lastTransaction != null && lastTransaction.Parameters != null)
			{
				dict = JsonSerializer.Create()
				.Deserialize<Dictionary<string, string>>(new JsonTextReader(new StringReader(lastTransaction.Parameters)));
			}
			return dict;
		}

		public Tuple<int,IEnumerable<Transaction>> GetTransactionBySearchParams(int skip, int take, string parameter, string value)
		{
			var queryParams = new QueryParams<Transaction>(skip, take, c => c.Id);

			Expression<Func<Transaction, bool>> expression = null;
			string sql = "";
			int totalCount = 0;
			if (string.IsNullOrEmpty(value))
			{
				expression = c => c.Id > 0;
				totalCount = _repository.TotalCount;
			}
			else
			{
				switch (parameter)
				{
					case "Id":
						var currentId = int.Parse(value);
						expression = x => x.Id.Equals(currentId);
						sql = "select COUNT(tRead.Id)  from [TransactionLogs] tRead where tRead.Id = " + value;
						break;
					case "Description":
						expression = c => c.Description.ToLower().Contains(value.ToLower());
						sql = "select COUNT(tRead.Id)  from [TransactionLogs] tRead where tRead.Description like N'%" + value.ToLower() + "%'";
						break;
					case "ActionDateTime": //не пашет. Ёбанный стыд
						var dt = DateTime.Parse(value);
						var nextDt = dt.AddMinutes(1);
						expression = c => c.ActionDateTime >= dt && c.ActionDateTime < nextDt;
						break;
				}
				totalCount = _repository.GetBySqlPredicate<int>(sql).FirstOrDefault();
			}
			var result = _repository.GetByPredicate(expression, queryParams);
			return new Tuple<int, IEnumerable<Transaction>>(totalCount, result);
		}

		public Tuple<int, int, int, int> GetTrancCount()
		{
			var sql1 = "select COUNT(tRead.Id)  from [TransactionLogs] tRead where tRead.ActionType = 4 and tRead.EntityType = 1";
			var sql2 = "select COUNT(tWrite.Id)  from [TransactionLogs] tWrite where tWrite.ActionType = 1 and tWrite.EntityType = 1";
			var sql3 = "select COUNT(tUpdate.Id) from [TransactionLogs] tUpdate where tUpdate.ActionType = 2 and tUpdate.EntityType = 1";
			var sql4 = "select COUNT(tDelete.Id) from [TransactionLogs] tDelete where tDelete.ActionType = 3 and tDelete.EntityType = 1";
			var read = _repository.GetBySqlPredicate<int>(sql1).FirstOrDefault();
			var write = _repository.GetBySqlPredicate<int>(sql2).FirstOrDefault();
			var update = _repository.GetBySqlPredicate<int>(sql3).FirstOrDefault();
			var delete = _repository.GetBySqlPredicate<int>(sql4).FirstOrDefault();
			return new Tuple<int, int, int, int>(read, write, update, delete);
		}

		public int GetTrancCountOfArticles()
		{
			var sql1 = "select COUNT(tRead.Id)  from [TransactionLogs] tRead where tRead.ActionType IN (1,2,3,4) and tRead.EntityType = 1";
			return _repository.GetBySqlPredicate<int>(sql1).FirstOrDefault();
		}

		public int TotalCount
		{
			get
			{
				return _repository.TotalCount;
			}
		}
	}
}
