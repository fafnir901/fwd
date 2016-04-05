using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FWD.Services;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;
using Microsoft.AspNet.SignalR;

namespace FWD.UI.Web.Models.Entities
{
	public class ReminderHub : Hub
	{
		private static ConcurrentDictionary<int, string> _conn;
		private static ConcurrentDictionary<int, Dictionary<int, string>> _messages;
		private static ConcurrentDictionary<int, ReminderService> _services;

		static ReminderHub()
		{
			_conn = new ConcurrentDictionary<int, string>();
			_messages = new ConcurrentDictionary<int, Dictionary<int, string>>();
			_services = new ConcurrentDictionary<int, ReminderService>();
		}

		private void ValidateForFeature()
		{
			if (!FeatureListModel.Instance.CheckFeatureForEnabled(Features.Reminder))
				throw new Exception("Напоминания отключены");
		}
		public int Store(int userId, string message)
		{
			ValidateForFeature();
			int taskId = 1;
			if (!_messages.ContainsKey(userId))
			{
				_messages.AddOrUpdate(userId,
					(c) => new Dictionary<int, string>() { { 1, message } },
					(c, d) => new Dictionary<int, string> { { 1, message } });
			}
			else
			{
				var kvp = _messages.FirstOrDefault(c => c.Key == userId);
				var max = kvp.Value.Count > 0 ? kvp.Value.Max(c => c.Key) : 0;
				taskId = max + 1;
				kvp.Value.Add(taskId, message);
			}
			return taskId;
		}

		public void Run(int userId, int taskId, int interval = 60)
		{
			ValidateForFeature();
			var reminderService = new ReminderService(interval);
			Task.Run(() =>
				reminderService.SubsribeAndRun(() => Notify(userId, taskId)),
				reminderService.Reminder.CancellationTokenSource.Token);
			_services.AddOrUpdate(taskId, (c) => reminderService, (c, d) => reminderService);
		}

		public void Exit(int taskId)
		{
			ValidateForFeature();
			_services[taskId].Exit();
			ReminderService ser;
			_services.TryRemove(taskId, out ser);
			var res = _messages.Where(c => c.Value.ContainsKey(taskId));
			foreach (var keyValuePair in res)
			{
				keyValuePair.Value.Remove(taskId);
			}
		}

		public IEnumerable<Data> GetAll(int userId)
		{
			ValidateForFeature();
			var result = _messages.Where(c => c.Key == userId).SelectMany(c => c.Value).Select(c => new Data { UserId = userId, TaskId = c.Key, Message = c.Value, IsRunning = false }).ToList();
			foreach (var item in result)
			{
				if (_services.ContainsKey(item.TaskId))
				{
					item.IsRunning = true;
				}
			}
			return result;
		}

		public class Data
		{
			public int UserId { get; set; }
			public int TaskId { get; set; }
			public string Message { get; set; }
			public bool IsRunning { get; set; }
		}
		public void Join(string connectionId, int userId)
		{
			ValidateForFeature();
			if (!_conn.ContainsKey(userId))
			{
				_conn.AddOrUpdate(userId, (c) => Context.ConnectionId, (c, d) => Context.ConnectionId);
			}
			else
			{
				_conn[userId] = Context.ConnectionId;
			}
			//if (!_messages.ContainsKey(userId))
			//{
			//	_messages.AddOrUpdate(userId, (c) => new List<string>() { message }, (c, d) => new List<string>() { message });
			//}

			//var reminderService = new ReminderService(10);
			//Task.Run(() => reminderService.SubsribeAndRun(() => Notify(userId)));
		}

		private void Notify(int userId, int taskId)
		{
			ValidateForFeature();
			Clients.Client(_conn[userId]).notify(_messages[userId][taskId]);
			//Clients.Caller.notify(_messages[userId][taskId]);
		}
	}
}