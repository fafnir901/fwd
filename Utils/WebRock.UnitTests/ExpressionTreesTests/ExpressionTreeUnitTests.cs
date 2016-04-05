using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRock.Utils.ExpressionTrees;
using WebRock.Utils.Mappers;

namespace WebRock.UnitTests.ExpressionTreesTests
{
	[TestClass]
	public class ExpressionTreeUnitTests
	{
		private class User
		{
			public string Name { get; set; }
			public string Login { get; set; }
			public DateTime RegisterDate { get; set; }
			public bool ForTest { get; set; }

			public Tester Tester { get; set; }

			public IEnumerable<EnumFields> EnumFieldses { get; set; }
		}

		private class Tester
		{
			public string S { get; set; }
			public SecondTester SecondTester { get; set; }
		}

		private class SecondTester
		{
			public string S2 { get; set; }
		}

		private class EnumFields
		{
			public string EnumS { get; set; }
		}

		private class UserDto
		{
			public string FirstName { get; set; }
			public string FirstLogin { get; set; }
			public DateTime RegisterDateOne { get; set; }
			public bool ForTest { get; set; }
			public Tester Vester { get; set; }

			public IEnumerable<EnumFields> mYEnumFieldses { get; set; }
		}

		private IEnumerable<User> GetCurrentUser(Expression<Func<UserDto, bool>> predicate, IEnumerable<User> users)
		{
			var mapper = new SimpleMapper<UserDto, User>();
			mapper.AddMapping(c => c.FirstName, c => c.Name);
			mapper.AddMapping(c => c.FirstLogin, c => c.Login);
			mapper.AddMapping(c => c.RegisterDateOne, c => c.RegisterDate);
			mapper.AddMapping(c => c.Vester, c => c.Tester);
			mapper.AddMapping(c => c.Vester.S, c => c.Tester.S);
			mapper.AddMapping(c => c.Vester.SecondTester, c => c.Tester.SecondTester);
			mapper.AddMapping(c => c.Vester.SecondTester.S2, c => c.Tester.SecondTester.S2);
			mapper.AddMapping(c => c.mYEnumFieldses, c => c.EnumFieldses);
			mapper.AddMapping(c => c.mYEnumFieldses.Select(x=>x.EnumS), c => c.EnumFieldses.Select(x=>x.EnumS));
			return users.Where(predicate.Convert<User, UserDto>(mapper).Compile());
		}

		private IEnumerable<User> GetCurrentUserWithoutConvert(Expression<Func<User, bool>> predicate, IEnumerable<User> users)
		{
			return users.Where(predicate.Compile());
		}

		[TestMethod]
		public void SimpleConverterTest()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today
				}
			};

			var result = GetCurrentUser(c => c.FirstName == "123" && c.FirstLogin == "123", lstOfUser);
			Assert.IsNotNull(result);
			Assert.AreEqual("123", result.FirstOrDefault().Name);
			Assert.AreEqual("123", result.FirstOrDefault().Login);
		}

		[TestMethod]
		public void ConverterTest()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "Petia",
					Name = "123",
					RegisterDate = DateTime.Now,
					ForTest = false
					
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today,
					ForTest = true
				}
			};

			var result = GetCurrentUser(c => c.FirstName == "123" || !c.ForTest || c.FirstLogin.Contains("Petia") || c.ForTest, lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual("123", result.FirstOrDefault().Name);
			Assert.AreEqual("Petia", result.FirstOrDefault().Login);

			result = GetCurrentUser(c => !c.ForTest || c.FirstLogin.Contains("Petia"), lstOfUser).ToList();
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void ConverterWithContainsTest()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today
				}
			};

			var result = GetCurrentUser(c => c.FirstName.Contains("1") && c.FirstLogin == "123", lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual("123", result.FirstOrDefault().Name);
			Assert.AreEqual("123", result.FirstOrDefault().Login);
		}

		[TestMethod]
		public void ConverterWithOnlyContainsTest()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today
				}
			};

			var result = GetCurrentUser(c => c.FirstName.Contains("1"), lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual("123", result.FirstOrDefault().Login);
		}

		[TestMethod]
		public void SimpleUnaryExpressionTest()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now,
					ForTest = false
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today,
					ForTest = true
				}
			};

			var result = GetCurrentUser(c => !c.ForTest, lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual("123", result.FirstOrDefault().Login);
		}

		[TestMethod]
		public void TestWithDeepGetterTest()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now,
					ForTest = false,
					Tester = new Tester
					{
						S = "5000",
						SecondTester = new SecondTester
						{
							S2 = "6000"
						}
					}
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today,
					ForTest = true,
					Tester = new Tester
					{
						S="6000",
						SecondTester = new SecondTester
						{
							S2 = "6000"
						}
					}
				}
			};
			var result = GetCurrentUser(c => c.Vester.SecondTester.S2.Contains("6") && c.Vester.S == "6000", lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual("6000", result.First().Tester.SecondTester.S2);
		}


		[TestMethod]
		public void TestWithNativeLinq()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now,
					ForTest = false,
					EnumFieldses = new List<EnumFields>
					{
						new EnumFields{EnumS = "6000"}
					}
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today,
					ForTest = true,
					EnumFieldses = new List<EnumFields>
					{
						new EnumFields{EnumS = "5000"}
					}
				}
			};
			var rs = GetCurrentUserWithoutConvert(c => c.EnumFieldses.FirstOrDefault(x => x.EnumS.Equals("5000")) != null, lstOfUser);
			Assert.IsNotNull(rs);
			Assert.AreEqual("5000", rs.First().EnumFieldses.First().EnumS);
			var result = GetCurrentUser(c => c.mYEnumFieldses.FirstOrDefault() != null, lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual("6000", result.First().EnumFieldses.First().EnumS);
		}

		[TestMethod]
		public void TestWithNativeLinqMoreDipper()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now,
					ForTest = false,
					EnumFieldses = new List<EnumFields>
					{
						new EnumFields{EnumS = "6000"}
					}
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today,
					ForTest = true,
					EnumFieldses = new List<EnumFields>
					{
						new EnumFields{EnumS = "5000"}
					}
				}
			};
			EnumFields empty = null;
			var result = GetCurrentUser(c => c.mYEnumFieldses.FirstOrDefault(x => x.EnumS.Equals("6000")) != empty && c.ForTest, lstOfUser).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual("6000", result.Single().EnumFieldses.Single().EnumS);
		}
	}
}
