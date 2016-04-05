using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.FileUtils
{
	public static class ImageUtils
	{
		/// <summary>
		/// Создает из набора байт полного изображения его иконку
		/// </summary>
		/// <param name="fullImageData">набора байт полного изображения</param>
		/// <param name="iconSize">размер иконки</param>
		/// <param name="format">Формат получившегося изображения.По умолчанию равен ImageFormat.Png</param>
		/// <returns></returns>
		public static byte[] CreateIconSimple(byte[] fullImageData, Size iconSize, ImageFormat format = null)
		{
			using (var memoryStream = new MemoryStream(fullImageData))
			{
				var bitmap = new Bitmap(memoryStream);
				var iconData = new Bitmap(bitmap, iconSize);
				var mIconStream = new MemoryStream();
				if (format == null)
				{
					format = ImageFormat.Png;
				}
				iconData.Save(mIconStream, format);
				return mIconStream.ToArray();
			}
		}
	}
}
