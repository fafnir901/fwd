using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FWD.Services
{
	public class ReminderService
	{
		public Reminder Reminder { get; set; }

		public ReminderService(int seconds)
		{
			Reminder = new Reminder(seconds);
		}

		public void SubsribeAndRun(Action act)
		{
			Reminder.Subscribe(act);
		}

		public void Exit()
		{
			Reminder.CancellationTokenSource.Cancel();
		}
	}

	public class Reminder
	{
		public CancellationTokenSource CancellationTokenSource { get; set; }
		public IObservable<long> Interval { get; set; }

		public Reminder(int seconds)
		{
			Interval = Observable.Interval(TimeSpan.FromSeconds(seconds));
			CancellationTokenSource = new CancellationTokenSource();	
		}

		public void Subscribe(Action act)
		{
			Interval.Subscribe(c => act(), CancellationTokenSource.Token);
		}
	}

	public class ReminderExitEventArgs : EventArgs
	{
		public CancellationTokenSource CancellationTokenSource { get; private set; }

		public ReminderExitEventArgs(CancellationTokenSource source)
		{
			CancellationTokenSource = source;
		}
	}
}
