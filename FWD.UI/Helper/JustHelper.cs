using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FWD.UI.Helper
{
	public static class JustHelper
	{
		public static void MakeFontSizeStretchable(int initialFontSize, int initialWindowHeight, int actualHeight, TextBlock block, params Control[] controls)
		{
			var inititialState = initialWindowHeight / initialFontSize;
			var delta = actualHeight / initialFontSize;
			var changingFontSize = delta - inititialState;
			var changedFontSize = initialFontSize + changingFontSize;
			if (changedFontSize > 32)
			{
				changedFontSize = 32;
			}
			if (changedFontSize < initialFontSize)
			{
				changedFontSize = initialFontSize;
			}
			foreach (var control in controls)
			{
				control.FontSize = changedFontSize;
			}
			if (block != null)
			{
				block.FontSize = changedFontSize;
			}
		}

		public static void ToggleToRightSource(bool isXml)
		{
			if (isXml)
			{
				IocHelper.ToggleToXml();
			}
			else
			{
				IocHelper.ToggleToDb();
			}
		}

		public static byte[] GetJpgFromImageControl(BitmapImage imageC)
		{
			var memStream = new MemoryStream();
			var encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(imageC));
			encoder.Save(memStream);
			return memStream.GetBuffer();
		}
	}
}
