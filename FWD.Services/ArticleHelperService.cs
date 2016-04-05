using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using Text.Analizer;
using Text.Analizer.Implementation;
using Text.Analizer.Termins;

namespace FWD.Services
{
	public class ArticleHelperService
	{
		private readonly static Lazy<ArticleHelperService> _instance = new Lazy<ArticleHelperService>(() => new ArticleHelperService());

		public static ArticleHelperService Instance
		{
			get { return _instance.Value; }
		}

		private SimpleTextAnalizerManager _manager;
		private ArticleHelperService()
		{
			_manager = new SimpleTextAnalizerManager(new SimpleAnalizerContainer());
		}

		public int GetCountOfWords(Article article)
		{
			_manager.Container.SetOriginalString(article.InitialText,true);//OriginalString = StripTagsCharArray(article.InitialText);
			return _manager.GetSentenses().SelectMany(c => c.Words).Count();
		}

		public int GetCountOfLetters(Article article)
		{
			_manager.Container.SetOriginalString(article.InitialText,true);//OriginalString = StripTagsCharArray(article.InitialText);
			return _manager.GetSentenses().SelectMany(c => c.Words.SelectMany(x => x.Letters)).Count();
		}

		public int GetCountOfSentences(Article article)
		{
			_manager.Container.SetOriginalString(article.InitialText, true);//OriginalString =  StripTagsCharArray(article.InitialText);
			return _manager.GetSentenses().Count();
		}

		public byte[] GetBytesFromUrl(string url)
		{
			byte[] b;
			HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
			WebResponse myResp = myReq.GetResponse();

			Stream stream = myResp.GetResponseStream();
			using (BinaryReader br = new BinaryReader(stream))
			{
				b = br.ReadBytes((int)myResp.ContentLength);
				br.Close();
			}
			myResp.Close();
			return b;
		}
	}
}
