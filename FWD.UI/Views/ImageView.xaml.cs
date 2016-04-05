using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = FWD.BusinessObjects.Domain.Image;

namespace FWD.UI.Views
{
	/// <summary>
	/// Interaction logic for ImageView.xaml
	/// </summary>
	public partial class ImageView : Window
	{
		public ImageView(Image image)
		{
			InitializeComponent();

			var strmImg = new MemoryStream(image.Data);

			var myBitmapImage = new BitmapImage();
			myBitmapImage.BeginInit();
			myBitmapImage.StreamSource = strmImg;
			myBitmapImage.EndInit();
			Image.Source = myBitmapImage;
			this.Height = myBitmapImage.Height;
			this.Width = myBitmapImage.Width;
			this.Title += " " + image.Name;

		}
	}
}
