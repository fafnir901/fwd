using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FWD.DAL.JSON
{
	public abstract class BaseJson<T>
	{
		protected const string Folder = "JSON";
		protected const string FileName = "data.json";
		protected string Path { get; set; }
		protected BaseJson(string path = null, bool overridePath = false)
		{
			if (!Directory.Exists(Folder))
			{
				Directory.CreateDirectory(Folder);
			}
			Path = @"{0}\{1}".Fmt(Folder, FileName);
			if (!File.Exists(Path))
			{
				File.Create(Path);
			}
		}

		protected List<T> DeserializeFromFile()
		{
			return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(Path));
		}

	}
}
