using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FWD.CommonIterfaces;

namespace FWD.Services
{
	public class Logger : ILogger
	{
		public string Path { get; set; }
		private string _fileName = "log.txt";

		public Logger(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			Path = path + @"\" + _fileName;


			if (File.Exists(Path))
			{
				var f = new FileInfo(Path);
				if (f.Length > 7000000)
				{
					File.Move(Path, path + @"\log_old.txt");
				}
			}
		}

		public void Trace(string action)
		{
			Task.Factory.StartNew(async () =>
			{
				//action += "\n";
				var date = DateTime.Now;
				//byte[] result = new UnicodeEncoding().GetBytes(action);
				var formatted = "{0}=>{1}".Fmt(date, action);
				using (var file = new FileStream(Path, FileMode.Append))
				{
					using (var writer = new StreamWriter(file))
					{
						await writer.WriteLineAsync(formatted);
						await writer.FlushAsync();
					}
				}
			});
		}

		public void TraceError(string action, Exception ex)
		{
			Task.Factory.StartNew(async () =>
			{
				var date = DateTime.Now;
				var formatted = "-----------------------------------------\nОшибка!!!---------------------------------\n{0}=>Метод:{1}\nСообщение:{2}".Fmt(date, action, ex.Message);
				using (var file = new FileStream(Path, FileMode.Append))
				{
					using (var writer = new StreamWriter(file))
					{
						await writer.WriteLineAsync(formatted);
						await writer.FlushAsync();
					}
				}
			});
		}

		public void TraceWarning(string action, string warning)
		{
			throw new NotImplementedException();
		}
	}
}
