using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.ModelBinding;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models.Helper;
using Microsoft.AspNet.SignalR;

namespace FWD.UI.Web.Models.Entities
{
	public class ChatHub : Hub
	{
		public void Send(dynamic message, string groupName)
		{
			// Call the broadcastMessage method to update clients.

			message.date = DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));
			message.id = Guid.NewGuid().ToString();

			Clients.Group(groupName).sendMessage(message);
			if (HttpContext.Current.Cache[groupName] == null)
			{
				var now = DateTime.Now;
				var lst = new List<dynamic>();
				lst.Add(message);

				var currentComment = new Comment
				{
					CommentId = Guid.Parse((string)message.id),
					GroupName = groupName,
					CommentText = message.message,
					UserName = message.name,
					AddedDate = DateTime.ParseExact((string)message.date, "F", CultureInfo.CreateSpecificCulture("en-US")),
					UserId = message.userId
				};
				var helper = new IocHelper();
				helper.CommentService.Save(currentComment);

				HttpContext.Current.Cache.Add(groupName, lst, null, DateTime.MaxValue, new TimeSpan(0, 0, 0, 30),
					CacheItemPriority.Default, (key, value, reason) => CacheCallBack(key));
			}
			else
			{
				var currentComment = new Comment
				{
					CommentId = Guid.Parse((string)message.id),
					GroupName = groupName,
					CommentText = message.message,
					UserName = message.name,
					AddedDate = DateTime.ParseExact((string)message.date, "F", CultureInfo.CreateSpecificCulture("en-US")),
					UserId = message.userId
				};
				var helper = new IocHelper();
				helper.CommentService.Save(currentComment);

				var current = HttpContext.Current.Cache[groupName] as List<dynamic>;
				if (current == null)
				{
					var next = HttpContext.Current.Cache[groupName] as List<Comment>;
					next.Add(currentComment);
				}
				else
				{
					current.Add(message);
				}
			}
		}

		public void Remove(string id, string name, string groupName)
		{
			object answer = new { type = "warning", message = "В данной группе нет комментариев" };
			var currentMessages = HttpContext.Current.Cache[groupName] as List<dynamic>;
			if (currentMessages != null)
			{
				var result = currentMessages.FirstOrDefault(c => c.name == name && c.id == id);
				if (result != null)
				{
					answer = new { type = "info", message = "Успешно удален" };
				}
				else
				{
					answer = new { type = "warning", message = "Комментария с данным ID не существует" };
				}

			}
			Clients.Group(groupName).removeMessage(answer);
		}

		public void Delete(string id, string groupName, string userName)
		{
			var helper = new IocHelper();
			helper.CommentService.DeleteComment(new Comment() { CommentId = Guid.Parse(id) });
			HttpContext.Current.Cache.Remove(groupName);
			var answer = new { type = "info", message = userName +" удалил комментарий", id = id };
			Clients.Group(groupName).removeMessage(answer);
		}

		public void ConnectToGroup(string groupName)
		{
			if (HttpContext.Current.Cache[groupName] != null)
			{
				var res = HttpContext.Current.Cache[groupName] as List<dynamic>;
				if (res == null)
				{
					var comments = HttpContext.Current.Cache[groupName] as List<Comment>;
					if (comments != null)
					{
						res = comments
									.Select(comment => new { message = comment.CommentText, id = comment.CommentId.ToString(), name = comment.UserName, date = comment.AddedDate.ToString("F", CultureInfo.CreateSpecificCulture("en-US")), userId = comment.UserId })
									.Cast<dynamic>().ToList();
					}
				}
				Clients.Caller.connectToGroup(res);
			}
			else
			{
				var helper = new IocHelper();
				var comments = helper.CommentService.GetByGroup(groupName);
				var lst = comments
					.Select(comment => new { message = comment.CommentText, id = comment.CommentId.ToString(), name = comment.UserName, date = comment.AddedDate.ToString("F", CultureInfo.CreateSpecificCulture("en-US")), userId = comment.UserId })
					.Cast<dynamic>().ToList();
				HttpContext.Current.Cache.Add(groupName, lst, null, DateTime.MaxValue, new TimeSpan(0, 0, 0, 30),
					CacheItemPriority.Default, (key, value, reason) => CacheCallBack(key));
				Clients.Caller.connectToGroup(lst);
			}
		}

		private void CacheCallBack(string key)
		{
			//var currentLst = value as List<dynamic>;
			//var lstForSave = currentLst
			//	.Select(c => new Comment { CommentId = Guid.Parse((string)c.id), GroupName = key, CommentText = c.message, UserName = c.name, AddedDate = DateTime.ParseExact((string)c.date, "F", CultureInfo.CreateSpecificCulture("en-US")) });
			//IocHelper.CommentService.SaveAll(lstForSave);
			if (HttpContext.Current != null)
			{
				var helper = new IocHelper();
				var res = helper.CommentService.GetByGroup(key);

				HttpContext.Current.Cache.Add(key, res, null, DateTime.MaxValue, new TimeSpan(0, 0, 5, 30),
					CacheItemPriority.Default, (currentKey, value, reason) => CacheCallBack(currentKey));
			}

		}

		public Task JoinGroup(string groupName)
		{
			return Groups.Add(Context.ConnectionId, groupName);
		}

		public Task LeaveGroup(string groupName)
		{
			return Groups.Remove(Context.ConnectionId, groupName);
		}
	}
}