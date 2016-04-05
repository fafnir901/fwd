using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Compatibility;
using Text.Analizer.Implementation;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;
using TextAnalyzerTests.Common;
using WebRock.Utils.Monad;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Text.Analizer.UnitTests
{
	[TestFixture]
	public class CommonAnalizerFixture
	{
		private SimpleAnalizerContainer _analizerContainer;
		private SimpleTextAnalizerManager _analyzerManager;

		[SetUp]
		public void OnSetUp()
		{
			_analizerContainer = new SimpleAnalizerContainer();
			_analyzerManager = new SimpleTextAnalizerManager(_analizerContainer);
		}

		[Test]
		public void ShouldCorrectDefineLanguageTest()
		{
			var engtextString = "Hello, Nikitos";
			var rustextString = "Здарова, Никитос";
			var engtextString1 = "Fuck off";

			var text = new Termins.Text(engtextString, _analyzerManager);
			_analizerContainer.Language.Should(Be.EqualTo("en-En"));

			text = new Termins.Text(rustextString, _analyzerManager);
			_analizerContainer.Language.Should(Be.EqualTo("ru-Ru"));

			text = new Termins.Text(engtextString1, _analyzerManager);
			_analizerContainer.Language.Should(Be.EqualTo("en-En"));
		}

		[Test]
		public void ShouldCorrectCalculateCountOfSentensesTest()
		{
			var txtString = "Hello! How are you?Are going to another place? Ok...Good buy.";
			var text = new Termins.Text(txtString, _analyzerManager);
			text.Sentenses.Count().Should(Be.EqualTo(5));
		}

		[Test]
		public void ShouldCorrectDefineTypeOfSentence()
		{
			var txtString = "Hello! How are you?Are going to another place? Ok...Good buy.No...";
			var text = new Termins.Text(txtString, _analyzerManager);
			var result = text.Sentenses.ToList();

			result[0].SentenseType.Should(Be.EqualTo(SentenseType.Exclamatory));
			result[1].SentenseType.Should(Be.EqualTo(SentenseType.Interrogative));
			result[2].SentenseType.Should(Be.EqualTo(SentenseType.Interrogative));
			result[3].SentenseType.Should(Be.EqualTo(SentenseType.Undefined));
			result[4].SentenseType.Should(Be.EqualTo(SentenseType.Default));
			result[5].SentenseType.Should(Be.EqualTo(SentenseType.Undefined));
		}

		[Test]
		public void ShouldDefiniteNames()
		{
			var txtString = "Hello! How are you?Are going to another place, Nikitos? Ok...Good buy.No...";
			var text = new Termins.Text(txtString, _analyzerManager);
			var result = text.Sentenses.ToList();
			result.SelectMany(c => c.Words.Where(x => x.IsName())).First().Value.Should(Be.EqualTo("Nikitos"));
		}

		[Test]
		public void ShouldGetSignsTest()
		{
			var tstString = "Hey,how are you? Ok,but you should know:i'm batmen.";
			var text = new Termins.Text(tstString, _analyzerManager);
			var result = text.Sentenses.ToList();
			result[0].Signs.Count().Should(Be.EqualTo(2));
			result[1].Signs.Count().Should(Be.EqualTo(3));
		}

		[Test]
		public void ShouldGetBeforeSignSimpleTest()
		{
			var tstString = "Hey,how are you? Ok,but you should know:i'm batmen.";
			var text = new Termins.Text(tstString, _analyzerManager);
			var result = text.Sentenses.ToList();
			var a = result[0].Signs.First().GetBeforeSign().MaybeAs<Word>().GetOrDefault(null);
			a.Value.Should(Be.EqualTo("Hey"));
		}

		[Test]
		public void ShouldGetBeforeSignComplexTest()
		{
			var tstString = "Hey,how are you,man? Ok,but you should know:i'm batmen.";
			var text = new Termins.Text(tstString, _analyzerManager);
			var result = text.Sentenses.ToList();
			var a = result[0].Signs.Skip(1).Take(1).First().GetBeforeSign().MaybeAs<Word>().GetOrDefault(null);
			var b = result[1].Signs.Skip(1).Take(1).First().GetBeforeSign().MaybeAs<Word>().GetOrDefault(null);
			Console.WriteLine(result[0].Signs.First().CurrentSign);
			Console.WriteLine(result[0].Signs.Skip(1).Take(1).First().CurrentSign);
			Console.WriteLine(result[0].ToString());
			Console.WriteLine(result[1].ToString());
			Console.WriteLine(b.Value);
			Console.WriteLine(a.Value);
			Assert.AreEqual("know", b.Value);
			Assert.AreEqual("you", a.Value);

			var d =
				result[0].Signs.FirstOrDefault(c => c.CurrentSign == SignEnum.Question)
					.MaybeAs<Sign>()
					.Bind(c => c.GetBeforeSign())
					.GetOrDefault(null);
			d.Should(Be.Not.Null);
			d.Value.Should(Be.EqualTo("man"));
			Console.WriteLine(d.Value);
		}

		[Test]
		public void ShouldGetAfterSignSimpleTest()
		{
			var tstString = "Hey,how are you? Ok,but you should know:i'm batmen.";
			var text = new Termins.Text(tstString, _analyzerManager);
			var result = text.Sentenses.ToList();
			var a = result[0].Signs.First().GetAfterSign().MaybeAs<Word>().GetOrDefault(null);
			a.Value.Should(Be.EqualTo("how"));
		}

		[Test]
		public void ShouldCorrectProcessHtmlText()
		{
			var testStr = "<div class='fedor' attr='pertia'>Hello, my name is 'Fedor'</div>";
			var text = new Termins.Text(testStr, _analyzerManager, isHtml: true);
			text.Value.Should(Be.EqualTo("Hello, my name is 'Fedor'"));
			text.EntityManager.Container.OriginalString.Should(Be.EqualTo("Hello, my name is 'Fedor'"));
		}

		[Test]
		public void ShouldParseBigText()
		{
			var strBuilder = new StringBuilder();

			for (int i = 0; i < 10000; i++)
			{
				strBuilder.Append("My Name is Nikita. I love music!");
			}

			var stopWatch = new Stopwatch();

			stopWatch.Start();
			var text = new Termins.Text(strBuilder.ToString(), _analyzerManager);
			var count = text.Sentenses.SelectMany(c => c.Words.SelectMany(x => x.Letters)).Count();
			Console.WriteLine("Words count : {0}", count);
			stopWatch.Stop();

			Console.WriteLine("Ellapsed:{0}", stopWatch.ElapsedMilliseconds);
		}
	}
}
