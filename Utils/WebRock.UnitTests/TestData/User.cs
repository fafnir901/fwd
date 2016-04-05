using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebRock.UnitTests.ExpressionTreesTests;
using WebRock.Utils.ExpressionTrees;
using WebRock.Utils.Mappers;

namespace WebRock.UnitTests.TestData
{
	public class User
	{
		public string Name { get; set; }
		public string Login { get; set; }
		public DateTime RegisterDate { get; set; }
		public bool IsActive { get; set; }
		public Credentials Credentials { get; set; }
		public IEnumerable<CutomActivity> Activities { get; set; }
		public static IEnumerable<User> GetCurrentUser(Expression<Func<UserDto, bool>> predicate, IEnumerable<User> users)
		{
			var mapper = new SimpleMapper<UserDto, User>();
			mapper.AddMapping(c => c.FirstName, c => c.Name);
			mapper.AddMapping(c => c.FirstLogin, c => c.Login);
			mapper.AddMapping(c => c.RegisterDateOne, c => c.RegisterDate);
			mapper.AddMapping(c => c.CredentialsDto, c => c.Credentials);
			mapper.AddMapping(c => c.CredentialsDto.Password, c => c.Credentials.Password);
			mapper.AddMapping(c => c.CredentialsDto.Token, c => c.Credentials.Token);
			mapper.AddMapping(c => c.CredentialsDto.Token.CurrentTokenNumber, c => c.Credentials.Token.CurrentTokenNumber);
			mapper.AddMapping(c => c.ActivitiesDto, c => c.Activities);
			return users.Where(predicate.Convert<User, UserDto>(mapper).Compile());
		}

		public static IEnumerable<User> CreateSimleData()
		{
			var lstOfUser = new List<User>
			{
				new User
				{
					Login = "123",
					Name = "123",
					RegisterDate = DateTime.Now,
					IsActive = false,
					Credentials = new Credentials
					{
						Password = "qwerty",
						Token = new Token
						{
							CurrentCuid = Guid.NewGuid(),
							CurrentTokenNumber = "5"
						}
					},
					Activities = new List<CutomActivity>
					{
						new CutomActivity
						{
							CurrentActivity = "6000"
						}
					}
				},
				new User
				{
					Login = "123",
					Name = "Name",
					RegisterDate = DateTime.Today,
					IsActive = true,
					Credentials = new Credentials
					{
						Password = "1q2w3e4r",
						Token = new Token
						{
							CurrentCuid = Guid.NewGuid(),
							CurrentTokenNumber = "6"
						}
					},
					Activities = new List<CutomActivity>
					{
						new CutomActivity
						{
							CurrentActivity = "5000"
						}
					}
				}
			};

			return lstOfUser;
		}
	}

	public class Credentials
	{
		public string Password { get; set; }
		public Token Token { get; set; }
	}

	public class Token
	{
		public string CurrentTokenNumber { get; set; }
		public Guid CurrentCuid { get; set; }
	}

	public class CutomActivity
	{
		public string CurrentActivity { get; set; }
	}

	public class UserDto
	{
		public string FirstName { get; set; }
		public string FirstLogin { get; set; }
		public DateTime RegisterDateOne { get; set; }
		public bool IsActive { get; set; }
		public Credentials CredentialsDto { get; set; }

		public IEnumerable<CutomActivity> ActivitiesDto { get; set; }
	}
}
