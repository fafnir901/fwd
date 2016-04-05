using System;
using System.Linq;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;
using WebRock.Utils.Monad;

namespace Text.Analizer.Implementation
{
	public class SimpleTextAnalizerManager :BaseAnalizerManager, ITextAnalizerManager
	{
		public SimpleTextAnalizerManager(IAnalizerContainer container, string language = null):base(container,language)
		{
		}

		public Sentense[] GetSentenses()
		{
			var buffer = Container.OriginalString
				.Replace(".", "~")
				.Replace("!", "~")
				.Replace("?", "~")
				.Replace("...", "~");
			return buffer
				.Split('~')
				.Where(c => c != null && !string.IsNullOrEmpty(c))
				.Select(c => new Sentense(ConcatSignToEndOfSentense(c), new SimpleSentenceAnalizerManager(new SimpleAnalizerContainer(), this.Container.Language), this.Container.Current))
				.ToArray();
		}

		public Letter[] GetLetters()
		{
			return Container.OriginalString.ToCharArray()
				.Select(c => new Letter(c.ToString(), new SimpleTextAnalizerManager(new SimpleAnalizerContainer(), this.Container.Language), this.Container.Current))
				.ToArray();
		}


		private string ConcatSignToEndOfSentense(string sentenceString)
		{
			_bufferedString = _bufferedString.Replace(sentenceString, "");

			var sign = _bufferedString.ToMaybe()
				.If(c => c.Count() >= 3)
				.Bind(c => c.Take(3))
				.Bind(c => c.CreateStringFromChars())
				.If(c => c.Equals("..."))
				.GetOrDefault(string.Empty);

			sign.IsNullOrEmpty().ToMaybe().If(c => c).Do((res) =>
			{
				sign = _bufferedString.FirstOrDefault() == default(char) ? "." : _bufferedString.FirstOrDefault().ToString();
			});

			var indexOfSign = _bufferedString.IndexOf(sign, System.StringComparison.Ordinal);
			if (indexOfSign != -1)
			{
				var removingCount = 1;
				if (sign == "...")
				{
					removingCount = 3;
				}
				_bufferedString = _bufferedString.Remove(indexOfSign, removingCount);
			}
			return sentenceString + sign;
		}


	}

}
