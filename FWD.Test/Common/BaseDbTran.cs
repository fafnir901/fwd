using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;

namespace FWD.Test.Common
{
	[TestFixture]
	public abstract class BaseDbRollbackFixture
	{
		protected TransactionScope _scope;

		[SetUp]
		protected void SetUp()
		{
			_scope = new TransactionScope();
		}

		[TearDown]
		protected void TearDown()
		{
			_scope.Dispose();
		}
	}
}
