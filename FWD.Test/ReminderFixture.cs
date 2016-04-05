using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FWD.Services;
using FWD.Test.Common;
using NUnit.Framework;

namespace FWD.Test
{
	[TestFixture]
	public class ReminderFixture
	{
		[Test]
		public void ShouldEnable()
		{
			var reminderService = new ReminderService(1);
			Task.Run(() => 
				reminderService.SubsribeAndRun(() => Console.WriteLine("Напоминание, будь челом бля")), 
				reminderService.Reminder.CancellationTokenSource.Token);

			Thread.Sleep(3000);
			reminderService.Exit();
			reminderService.Reminder.CancellationTokenSource.IsCancellationRequested.Should(Be.True);
		}

	}
}
