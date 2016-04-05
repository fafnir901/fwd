using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.FileUtils
{
	public static class IOUtils
	{
		/// <summary>
		/// Получает набор байтов из УРЛа
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static byte[] GetDataFromUrl(string url)
		{
			if (!File.Exists(url))
				throw new FileNotFoundException("File not exists.");
			using (var fs = new FileStream(url, FileMode.Open))
			{
				var reader = new BinaryReader(fs);
				return reader.ReadBytes((int)fs.Length);
			}
		}

		/// <summary>
		/// Получает набор байтов из потока
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] GetDataFromStream(Stream stream)
		{
			if (stream.Length < 1)
				throw new Exception("Nothing to read.");
			using (var reader = new BinaryReader(stream))
			{
				return reader.ReadBytes((int)stream.Length);
			}
		}

		/// <summary>
		/// Создает набор байтов для строки данных в формате .csv.
		/// Так же добавляет вначала BOM
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static byte[] CreateCsvFileData(string fileName, string data)
		{
			var stringBytes = Encoding.UTF8.GetBytes(data).ToList();
			var bufferBytes = new List<byte>() { 0xEF, 0xBB, 0xBF }; // Add BOM

			bufferBytes.AddRange(stringBytes);
			//return File(bufferBytes.ToArray(), "text/csv; charset = utf-8;",
			//	fileName + ".csv");
			return bufferBytes.ToArray();
		}
	}

}
