using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace FWD.UI.Web.Models.Entities
{
	public class ArticlesHub : Hub
	{
		public void NotifyThatArticleWasAddedOrUpdated(dynamic message)
		{
			message.date = DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));
			message.id = Guid.NewGuid().ToString();
			Clients.Others.sendMessage(message);
		}
	}
}