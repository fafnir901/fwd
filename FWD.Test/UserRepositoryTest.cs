using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.DAL.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FWD.Test
{
	[TestClass]
	public class UserRepositoryTest
	{
		[TestMethod]
		public void Test()
		{
			var rep = new UserDbRepository();
			//var res = rep.GetBuPasswordAndLogin("DotNetZip901", "fafnir901");
			//res.Avatar = WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(@"D:\WP_20150325_001.jpg");
			//rep.Update(res);
		}
	}
}
