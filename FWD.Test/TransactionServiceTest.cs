using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.DAL.Domain;
using FWD.Services;
using FWD.Test.Common;
using NUnit.Framework;

namespace FWD.Test
{
	[TestFixture]
	public class TransactionServiceTest : BaseDbRollbackFixture
	{
		[Test]
		public void GetCounts()
		{
			var rep = new TransactionDBRepository();
			var service = new TransactionService(rep);
			var t = service.GetTrancCount();
			t.Item1.Should(Be.GreaterThan(0));
			t.Item2.Should(Be.GreaterThan(0));
			t.Item3.Should(Be.GreaterThan(0));
		}

		[Test]
		public void GetBySearchParameters()
		{
			var rep = new TransactionDBRepository();
			var service = new TransactionService(rep);

			var result = service.GetTransactionBySearchParams(0, 10, "Description", "Статья");
			result.Item2.Count().Should(Be.GreaterThan(0));

			result = service.GetTransactionBySearchParams(0, 10, "Id", "1");
			result.Item2.Count().Should(Be.EqualTo(1));

			//result = service.GetTransactionBySearchParams(0, 10, "ActionDateTime", "26.03.2015, 20:22:47");
			//result.Count().Should(Be.EqualTo(1));
		}
	}
}
