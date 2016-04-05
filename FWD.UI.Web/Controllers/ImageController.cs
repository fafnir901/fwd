using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FWD.BusinessObjects.Domain;
using FWD.UI.Web.Models.Entities;
using FWD.UI.Web.Models.Helper;
using WebRock.Utils;
using WebRock.Utils.Monad;

namespace FWD.UI.Web.Controllers
{
	public class ImageController : Controller
	{

		[ActionName("GetImages")]
		public async Task<FileContentResult> GetImageByArticleIdAndImageId(string articleID, string imageID, string size = null)
		{
			int articleIdCurrent, imageIdCurrent;
			var maybeArticleId = articleID.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
			var maybeImageId = imageID.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
			var maybeSize = size.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");

			int.TryParse(maybeArticleId, out articleIdCurrent);
			int.TryParse(maybeImageId, out imageIdCurrent);

			var helper = new IocHelper();

			if (CommonHelper.Instance.CurrentArticle.ArticleId != articleIdCurrent)
			{
				CommonHelper.Instance.CurrentArticle = helper.ArticleService.GetArticleById(articleIdCurrent);
			}

			var images = CommonHelper.Instance.CurrentArticle.Images.Select(x => new ImageResult { ImageData = x.Data, Id = x.ImageId });
			var result = images.FirstOrDefault(c => c.Id == imageIdCurrent);
			var reworkedImage = CommonHelper.Instance.DefineSizeOfImage(maybeSize, result);
			return await Task.Factory.StartNew(() => File(reworkedImage.MaybeAs<ImageResult>().Bind(c => c.ImageData).GetOrDefault(null), "image/gif"));
		}

		[HttpPost]
		[Route("uploadImage")]
		public ActionResult UploadImageInTempSession(HttpPostedFileBase[] files)
		{
			try
			{
				if (files == null)
					throw new Exception("Файлы не могут быть загружены");
				var fls = CommonHelper.Instance.TempFiles;
				var currentLst = new List<ImageResult>();
				foreach (var httpPostedFileBase in files)
				{
					if (!fls.Select(c => c.Name).Contains(httpPostedFileBase.FileName))
					{
						var imageRes = new ImageResult
						{
							GuidId = Guid.NewGuid(),
							Name = httpPostedFileBase.FileName,
							ImageData = new byte[httpPostedFileBase.ContentLength]
						};
						httpPostedFileBase.InputStream.Read(imageRes.ImageData, 0, httpPostedFileBase.ContentLength);
						fls.Add(imageRes);
						currentLst.Add(imageRes);
					}
					else
					{
						currentLst.Add(fls.FirstOrDefault(c => c.Name == httpPostedFileBase.FileName));
					}
				}
				return Json(currentLst[0].GuidId.ToString(), JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}

		}

		[HttpPost]
		[Route("uploadImageByLink")]
		public ActionResult UploadImageByLink(string url)
		{
			try
			{
				if (string.IsNullOrEmpty(url))
				{
					throw new Exception("Ссылка не может быть пустой");
				}
				//using (var client = new WebClient())
				//{
				//var random = new Random();
				//string fileName = "File Name " + random.Next(0, 999999)+".jpeg";
				//client.DownloadFile(url, url.Substring(url.LastIndexOf('/')));

				var request = WebRequest.Create(url);

				using (var response = request.GetResponse())
				{
					using (var stream = response.GetResponseStream())
					{
						var img = Bitmap.FromStream(stream);
						var imageRes = new ImageResult
						{
							GuidId = Guid.NewGuid(),
							Name = url.Substring(url.LastIndexOf('/')),
							ImageData = GetByteArrayFromImage(img)
						};
						CommonHelper.Instance.TempFiles.Add(imageRes);
						return Json(imageRes.GuidId.ToString(), JsonRequestBehavior.AllowGet);
					}
				}
				//}
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		private byte[] GetByteArrayFromImage(System.Drawing.Image imageIn)
		{
			var ms = new MemoryStream();
			imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
			return ms.ToArray();
		}

		[HttpPost]
		[Route("clearImage")]
		public void ClearImagesInTempSession()
		{
			CommonHelper.Instance.TempFiles = new List<ImageResult>();
		}

		[Route("getTempImages/{imageGuid}/{size}")]
		public async Task<FileContentResult> GetTempImages(string imageGuid, string size)
		{
			Guid currentGuid;
			Guid.TryParse(imageGuid, out currentGuid);
			var res = CommonHelper.Instance.TempFiles.FirstOrDefault(c => c.GuidId == currentGuid).NothingIfNull();
			var maybeSize = size.MaybeAs<string>().Bind(c => c.Split('-').Skip(1).Take(1).Last()).GetOrDefault("");
			var reworkedImage = CommonHelper.Instance.DefineSizeOfImage(maybeSize, res.GetOrDefault(null));

			return await Task.Factory.StartNew(() => File(reworkedImage.MaybeAs<ImageResult>().Bind(c => c.ImageData).GetOrDefault(null), "image/gif"));
		}
	}
}