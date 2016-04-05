using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
using FWD.BusinessObjects;
using FWD.BusinessObjects.Domain;
using FWD.UI.Helper;
using Microsoft.Win32;
using Image = System.Windows.Controls.Image;

namespace FWD.UI.Views
{
	/// <summary>
	/// Interaction logic for Create.xaml
	/// </summary>
	public partial class Create : Window
	{
		private MainWindow _mainWindow;
		private Article _editableAtricle;
		private Article _creatableAtricle;
		private bool _isXml;

		public const string IMAGE_NAME_SUFFIX = "Image_";

		public Create(MainWindow mainWindow, bool isXml, Article editableAtricle = null)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			_editableAtricle = editableAtricle;
			if (_editableAtricle != null)
			{
				Name.Text = _editableAtricle.ArticleName;
				Text.Text = _editableAtricle.InitialText;
				Link.Text = _editableAtricle.Link;
				AuthorNameTextBox.Text = _editableAtricle.AuthorName;
				foreach (var image in _editableAtricle.Images)
				{
					var img = new Image();
					img.Stretch = Stretch.Fill;
					img.Name = IMAGE_NAME_SUFFIX + image.ImageId;
					CreateBitmapImageFromData(image.Data, img);
					ImageListBox.Items.Add(img);
				}
			}
			else
			{
				_creatableAtricle = new Article();
			}
			_isXml = isXml;
			var groups = IocHelper.GroupService.GetAllGroups(c => c.GroupId);
			var names = groups.Select(c => c.GroupName);
			foreach (var name in names)
			{
				GroupsComboBox.Items.Add(name);
			}
			GroupsComboBox.SelectedIndex = 0;
			if (_editableAtricle != null && groups.Select(c => c.Groups).Select(c => c.Contains(_editableAtricle.ArticleName)).Any())
			{
				foreach (var item in GroupsComboBox.Items)
				{
					var res = groups.FirstOrDefault(c => c.GroupName == item.ToString());
					if (res != null)
					{
						bool val = res.Groups.Contains(_editableAtricle.ArticleName);
						if (val)
						{
							var index = GroupsComboBox.Items.IndexOf(res.GroupName);
							GroupsComboBox.SelectedIndex = index;
						}
					}
				}
			}
		}

		private void CreateBitmapImageFromData(byte[] data, Image image)
		{
			if (data == null) return;
			try
			{
				var strmImg = new MemoryStream(data);

				var myBitmapImage = new BitmapImage();
				myBitmapImage.BeginInit();
				myBitmapImage.StreamSource = strmImg;
				myBitmapImage.DecodePixelHeight = 200;
				myBitmapImage.DecodePixelWidth = 200;
				myBitmapImage.EndInit();
				image.Source = myBitmapImage;

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Validate();
				if (_editableAtricle == null)
				{
					CreateNewArticle();
				}
				else
				{
					UpdateAtricle();
				}
				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				Name.Text = string.Empty;
				Text.Text = string.Empty;
				Link.Text = string.Empty;
			}
		}

		private void UpdateAtricle()
		{
			_editableAtricle.ArticleName = Name.Text;
			_editableAtricle.InitialText = Text.Text;
			_editableAtricle.Link = Link.Text;
			_editableAtricle.AuthorName = AuthorNameTextBox.Text;

			InitGroups(_editableAtricle);


			JustHelper.ToggleToRightSource(_isXml);
			IocHelper.ArticleService.UpdateArticle(_editableAtricle);
			_mainWindow.CreatedArticle = _editableAtricle;
			MessageBox.Show("Статья успешно обновлена", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void InitGroups(Article article)
		{
			var group = GetGroup();
			if (@group.Groups == null)
			{
				@group.Groups = new List<string>();
			}
			@group.Groups.Add(article.ArticleName);
			IocHelper.GroupService.UpdateGroup(@group);
			_mainWindow.Groups = IocHelper.GroupService.GetAllGroups(c => c.GroupId).ToList();
		}

		private void CreateNewArticle()
		{

			_creatableAtricle.ArticleName = Name.Text;
			_creatableAtricle.InitialText = Text.Text;
			_creatableAtricle.Link = Link.Text;
			_creatableAtricle.AuthorName = AuthorNameTextBox.Text;

			InitGroups(_creatableAtricle);

			JustHelper.ToggleToRightSource(_isXml);
			IocHelper.ArticleService.SaveArticle(_creatableAtricle);
			_mainWindow.CreatedArticle = _creatableAtricle;
			MessageBox.Show("Статья успешно сохранена", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private ArticleGroup GetGroup()
		{
			if (GroupsComboBox.SelectedItem != null)
			{
				var name = GroupsComboBox.SelectedItem.ToString();
				return IocHelper.GroupService.GetGroupsByParams(c => c.GroupName == name, c => c.GroupId, 0, 1).FirstOrDefault();
			}
			else
			{
				var newGroup = new ArticleGroup
				{
					GroupName = GroupsComboBox.Text
				};
				IocHelper.GroupService.SaveGroup(newGroup);
				return newGroup;
			}
		}

		private void Validate()
		{
			var builder = new StringBuilder();
			if (string.IsNullOrEmpty(Name.Text))
			{
				builder.Append("У статьи остутствует название.");
			}
			if (string.IsNullOrEmpty(Link.Text))
			{
				builder.Append("У статьи отсутствует ссылка на источник.");
			}
			if (string.IsNullOrEmpty(Text.Text) || Text.Text.Length < 3)
			{
				builder.Append("Отсутствует тескт статьи или текст меньше 3-ех символов.");
			}
			if (!string.IsNullOrEmpty(builder.ToString()))
			{
				throw new Exception(builder.ToString());
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var myDialog = new OpenFileDialog();
			myDialog.Filter = "Картинки(*.JPG;*.GIF;*.PNG)|*.JPG;*.GIF;*.PNG" + "|Все файлы (*.*)|*.* ";
			myDialog.CheckFileExists = true;
			myDialog.Multiselect = true;
			if (myDialog.ShowDialog() == true)
			{
				var data = WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(myDialog.FileName);
				var image = new BusinessObjects.Domain.Image()
				{
					Data = data,
					Name = myDialog.SafeFileName
				};
				InitImage(image, myDialog.SafeFileName);
			}
		}

		private void InitImage(BusinessObjects.Domain.Image image, string name)
		{
			var img = new Image();
			img.Stretch = Stretch.Fill;

			if (_editableAtricle != null)
			{
				var res = _editableAtricle.Images.FirstOrDefault(c => c.Name == name);
				if (res == null)
				{
					_editableAtricle.Images.Add(image);
					CreateBitmapImageFromData(image.Data, img);
					ImageListBox.Items.Add(img);
				}
				else
				{
					MessageBox.Show("В данной статье уже имеется подобная картинка", "Сообщение", MessageBoxButton.OK,
						MessageBoxImage.Information);
				}
			}
			else
			{
				var images = _creatableAtricle.Images ?? new List<BusinessObjects.Domain.Image>();
				var res = images.FirstOrDefault(c => c.Name == name);
				if (res == null)
				{
					images.Add(image);
					CreateBitmapImageFromData(image.Data, img);
					ImageListBox.Items.Add(img);
				}
				else
				{
					MessageBox.Show("В данной статье уже имеется подобная картинка", "Сообщение", MessageBoxButton.OK,
						MessageBoxImage.Information);
				}
				_creatableAtricle.Images = images;
			}
		}

		private void ImageListBox_OnDrop(object sender, DragEventArgs e)
		{
			try
			{
				var data = e.Data as DataObject;
				BusinessObjects.Domain.Image image = new BusinessObjects.Domain.Image();
				if (data.ContainsFileDropList())
				{
					var buffer = WebRock.Utils.FileUtils.IOUtils.GetDataFromUrl(data.GetFileDropList()[0]);
					image.Data = buffer;
					image.Name = data.GetFileDropList()[0].Split(@"\".ToCharArray()).Last();
				}
				else if (data.ContainsText())
				{
					image.Data = IocHelper.ArticleHelperService.GetBytesFromUrl(data.GetText());
					image.Name = data.GetText().Split(@"\".ToCharArray()).Last();
				}
				InitImage(image, image.Name);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			ToggleToRightSource();
		}

		private void ToggleToRightSource()
		{
			JustHelper.ToggleToRightSource(_isXml);
		}

		private void ImageListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				Image item;
				var lstItem = sender as ListBox;
				var elem = lstItem.InputHitTest(e.GetPosition(lstItem)) as UIElement;

				while (elem != lstItem)
				{
					if (elem is Image)
					{
						item = (Image)elem;

						var image = new BusinessObjects.Domain.Image()
						{
							Name = item.Name,
							Data = JustHelper.GetJpgFromImageControl(item.Source as BitmapImage)
						};
						var view = new ImageView(image);
						view.ShowDialog();
						return;
					}
					elem = (UIElement)VisualTreeHelper.GetParent(elem);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void ImageListBox_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				if (MessageBox.Show("Удалить?", "Сообщение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					foreach (var selectedItem in ImageListBox.SelectedItems)
					{
						var res = selectedItem as Image;
						if (res != null)
						{
							var strId = res.Name.Replace(IMAGE_NAME_SUFFIX, "");
							if (!string.IsNullOrEmpty(strId))
							{
								_editableAtricle.Images.Remove(_editableAtricle.Images.First(c => c.ImageId.ToString() == strId));
							}
							var index = ImageListBox.Items.IndexOf(res);
							ImageListBox.Items.RemoveAt(index);
							ImageListBox.Items.Refresh();
							break;
						}
					}

				}
			}
		}

		private void ImageListBox_OnDragEnter(object sender, DragEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
