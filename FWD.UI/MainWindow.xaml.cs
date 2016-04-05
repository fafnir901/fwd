using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FWD.BusinessObjects.Domain;
using FWD.Services;
using FWD.UI.Helper;
using FWD.UI.Views;
using WebRock.Utils.Monad;
using Image = System.Windows.Controls.Image;

namespace FWD.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Remove.IsEnabled = false;
			Edit.IsEnabled = false;
			Statisticks.IsEnabled = false;
			ArticlesCountChanged += MainWindow_ArticlesCountChanged;
			FontSizeLabel.Content = Description.FontSize;
			_windowHeight = (int)this.Height;
			try
			{
				if (!UserConfigService.IsMissingAuth)
				{
					IocHelper.ToggleToDb();
					InitListOfArticles();
				}
				else
				{
					IocHelper.ToggleToXml();
					isXml = true;
					InitListOfArticles();
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("Отсутствует подключение к БД.") || ex is InvalidOperationException)
				{
					MessageBox.Show(string.Format("{0}. Отсутствует подключение к БД. Переключение на контекст XML.", ex.Message), "Ошибка", MessageBoxButton.OK,
						MessageBoxImage.Error);
					IocHelper.ToggleToXml();
					isXml = true;

					InitListOfArticles();

					XMLChecker.IsEnabled = false;
					SaveToDB.IsEnabled = false;
				}
				else
				{
					ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
				}
			}
			Find.Background = _buttonBackgroundInit;
			Find.Foreground = _initialForegroundColorBrush;
			Find.MouseEnter += Button_MouseEnter;
			Find.MouseLeave += Button_MouseLeave;
			SortButton.Background = _buttonBackgroundInit;
			SortButton.Foreground = _initialForegroundColorBrush;
			SortButton.MouseEnter += Button_MouseEnter;
			SortButton.MouseLeave += Button_MouseLeave;
			if (UserConfigService.IsMissingAuth)
			{
				SaveToDB.IsEnabled = false;
				SaveToXML.IsEnabled = false;
				SaveToDB.Visibility = Visibility.Hidden;
				SaveToXML.Visibility = Visibility.Hidden;

				XMLChecker.IsEnabled = false;
				XMLChecker.Visibility = Visibility.Hidden;
				UpdateToXml.IsEnabled = false;
				UpdateToXml.Visibility = Visibility.Hidden;
				Xml.IsEnabled = false;
				Xml.Visibility = Visibility.Collapsed;
				Save.IsEnabled = false;
				Save.Visibility = Visibility.Collapsed;
				LoginItem.Visibility = Visibility.Hidden;
			}
			else
			{
				LoginItem.Header = string.Format("Добро пожаловать,{0} {1}", UserConfigService.LoggedUser.FirstName,
			UserConfigService.LoggedUser.LastName);
			}
			//TextBoxStyle.ApplyOldDisplayStyle(Description);
		}

		#region Fields and Properties

		private bool _isGroupsVisible;
		private bool isXml;
		private bool isOrdered;
		public const string IMAGE_NAME_SUFFIX = "Image_";
		public Article CreatedArticle { get; set; }

		private Article _viewingArticle;
		private List<Article> _articles;
		private List<ArticleGroup> _groups;
		public List<Article> Articles
		{
			get { return _articles; }
			set
			{
				_articles = value;
				if (ArticlesCountChanged != null)
				{
					ArticlesCountChanged.Invoke(this, new EventArgs());
				}
			}
		}

		public List<ArticleGroup> Groups
		{
			get
			{
				return _groups;
			}
			set
			{
				_groups = value;
			}
		}
		public event EventHandler<EventArgs> ArticlesCountChanged;

		private static int _fontSize = 12;
		private static int _windowHeight;

		private static LinearGradientBrush _buttonBackgroundInit = new LinearGradientBrush
		{
			EndPoint = new Point(0.5, 1),
			StartPoint = new Point(0.5, 5),
			MappingMode = BrushMappingMode.RelativeToBoundingBox,
			GradientStops = new GradientStopCollection()
			{
				new GradientStop(Color.FromArgb(0xFF,0x0A,0x0A,0x0A),0),
				new GradientStop(Color.FromArgb(0xFF,0x4F,0x4A,0x4A),0.992)
			}
		};

		private static LinearGradientBrush _buttonBackgroundSelection = new LinearGradientBrush
		{
			EndPoint = new Point(0.5, 1),
			StartPoint = new Point(0.5, 5),
			MappingMode = BrushMappingMode.RelativeToBoundingBox,
			GradientStops = new GradientStopCollection()
			{
				new GradientStop(Color.FromArgb(255,77,149,84),0),
				new GradientStop(Color.FromArgb(255,95,201,59),0.992)
			}
		};

		private static SolidColorBrush _initialForegroundColorBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x06, 0xFF, 0x28));

		#endregion

		#region Control Events
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			var process = Process.GetCurrentProcess();
			process.Kill();
		}
		private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			JustHelper.MakeFontSizeStretchable(_fontSize, _windowHeight, (int)e.NewSize.Height, null, Finder, Count);
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			StretchImages();
		}
		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				InitListOfArticles();
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
			}

		}
		private void XMLChecker_Click(object sender, RoutedEventArgs e)
		{
			isXml = !isXml;
			if (isXml)
			{
				IocHelper.ToggleToXml();
				var result = IocHelper.ArticleService.GetAllArticles(c => c.ArticleName);
				Articles = result == null ? new List<Article>() : result.ToList();
				if (_isGroupsVisible)
				{
					TreeViewHelper.Instance.FillTreeView(Groups, Articles, ArticleTreeView);
					TreeViewHelper.Instance.SelectAndExpand(ArticleTreeView, Articles.FirstOrDefault());
				}
				else
				{
					CreateListOfArticles(Articles);
				}

			}
			else
			{
				IocHelper.ToggleToDb();
				var result = IocHelper.ArticleService.GetAllArticles(c => c.ArticleName);
				Articles = result == null ? new List<Article>() : result.ToList();
				if (_isGroupsVisible)
				{
					TreeViewHelper.Instance.FillTreeView(Groups, Articles, ArticleTreeView);
					TreeViewHelper.Instance.SelectAndExpand(ArticleTreeView, Articles.FirstOrDefault());
				}
				else
				{
					CreateListOfArticles(Articles);
				}
			}
			var message = isXml ? "Контекст переключен на работу с XML" : "Контекст переключен на работу с БД";
			ShowMessage(message);
			InvokeCounter();
			if (!isXml && !_isGroupsVisible)
			{
				SortButton.Visibility = Visibility.Visible;
			}
			else
			{
				SortButton.Visibility = Visibility.Hidden;
			}
		}
		private void SaveToXML_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				IocHelper.ToggleToXml();
				SaveArticles();
				JustHelper.ToggleToRightSource(isXml);
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
				JustHelper.ToggleToRightSource(isXml);
			}

		}
		private void SaveToDB_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				IocHelper.ToggleToDb();
				SaveArticles();
				JustHelper.ToggleToRightSource(isXml);
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
				JustHelper.ToggleToRightSource(isXml);
			}
		}
		private void UpdateToXml_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				Article article;
				if (!_isGroupsVisible)
				{
					article = GetArticlesFromListBox().FirstOrDefault() ?? _viewingArticle;

				}
				else
				{
					article = TreeViewHelper.Instance.GetArticleFromTreeView(ArticleTreeView, Articles) ?? _viewingArticle;
				}
				if (!isXml)
				{
					IocHelper.ToggleToXml();
				}
				if (article == null)
				{
					throw new Exception("Не выбран элемент для обновления.");
				}
				IocHelper.ArticleService.UpdateArticle(article);
				JustHelper.ToggleToRightSource(isXml);
				ShowMessage(string.Format("Статья \"{0}\" успешно обновлена в XML.", article.ArticleName));
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
				JustHelper.ToggleToRightSource(isXml);
			}

		}
		private void ListOfArticle_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var enabled = ListOfArticle.SelectedItems.Count > 0;
			Remove.IsEnabled = enabled || _viewingArticle != null;
			Edit.IsEnabled = enabled || _viewingArticle != null;
			Statisticks.IsEnabled = enabled || _viewingArticle != null;
		}
		private void Find_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string value = new string(Finder.Text.ToCharArray());
				IEnumerable<Article> result = new List<Article>();
				if (!string.IsNullOrEmpty(value))
				{
					Expression<Func<Article, bool>> predicate;
					if (isXml)
					{
						predicate = c => c.ArticleName.Contains(value) || c.InitialText.Contains(value);
					}
					else
					{
						predicate = c => c.ArticleName.Contains(value) || c.InitialText.Contains(value) || c.AuthorName.Contains(value);
					}
					result =
						IocHelper.ArticleService.GetArticlesByParams(predicate,
							c => c.ArticleId);
					if (_isGroupsVisible)
					{
						TreeViewHelper.Instance.FillTreeView(Groups, result, ArticleTreeView);
					}
					else
					{
						CreateListOfArticles(result);
					}
					Articles = result.ToList();
				}
				else
				{
					result =
						IocHelper.ArticleService.GetAllArticles(c => c.ArticleId);
					if (_isGroupsVisible)
					{
						TreeViewHelper.Instance.FillTreeView(Groups, result, ArticleTreeView);
					}
					else
					{
						CreateListOfArticles(result);
					}
					Articles = result.ToList();
				}

				FillContent(result.FirstOrDefault());
				if (_isGroupsVisible)
				{
					TreeViewHelper.Instance.SelectAndExpand(ArticleTreeView, result.FirstOrDefault());
				}
				else
				{
					ListOfArticle.SelectedItem = ListOfArticle.Items.Count > 0 ? ListOfArticle.Items[0] : null;
				}
				CreateMatchingSelection(value);
				InvokeCounter();
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
			}

		}
		private void Create_Click(object sender, RoutedEventArgs e)
		{
			var view = new Create(this, isXml);
			view.ShowDialog();
			if (CreatedArticle != null)
			{
				Articles.Add(CreatedArticle);
			}
			if (_isGroupsVisible)
			{
				TreeViewHelper.Instance.FillTreeView(Groups, Articles, ArticleTreeView);
				TreeViewHelper.Instance.SelectAndExpand(ArticleTreeView, CreatedArticle);
			}
			else
			{
				CreateListOfArticles(Articles);
			}

			CreatedArticle = null;
			InvokeCounter();
		}
		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			var res = GetArticlesFromListBox().FirstOrDefault();
			if (res != null && !_isGroupsVisible)
			{
				var view = new Create(this, isXml, res);
				view.Title = "Редактировать";
				view.ShowDialog();
			}
			else if (_viewingArticle != null)
			{
				var view = new Create(this, isXml, _viewingArticle);
				view.Title = "Редактировать";
				view.ShowDialog();
			}

			if (CreatedArticle != null)
			{
				var art = Articles.First(c => c.ArticleId == CreatedArticle.ArticleId);
				art.ArticleName = CreatedArticle.ArticleName;
				art.InitialText = CreatedArticle.InitialText;
				art.Link = CreatedArticle.Link;
				art.Images = CreatedArticle.Images;
			}
			CreatedArticle = null;
			if (_isGroupsVisible)
			{
				TreeViewHelper.Instance.FillTreeView(Groups, Articles, ArticleTreeView);
				TreeViewHelper.Instance.SelectAndExpand(ArticleTreeView, _viewingArticle);
			}
			else
			{
				CreateListOfArticles(Articles);
			}
			InvokeCounter();
		}
		private void Remove_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (
					MessageBox.Show("Вы действительно хотите удалить статьи?", "Сообщение", MessageBoxButton.YesNo,
						MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					IEnumerable<Article> articles;
					if (_isGroupsVisible)
					{
						articles = new List<Article>();
						var res = TreeViewHelper.Instance.GetArticleFromTreeView(ArticleTreeView, Articles);
						if (res == null)
						{
							var group = TreeViewHelper.Instance.GetGroupFromTreeView(ArticleTreeView, Articles);
							var arts = group.Articles;
							var art = articles.ToList();
							art.AddRange(arts);
							articles = art;

							IocHelper.GroupService.DeleteGroup(group);
							Groups.Remove(Groups.First(c => c.GroupName == group.GroupName));
							JustHelper.ToggleToRightSource(isXml);
						}
						else
						{
							var art = articles.ToList();
							art.Add(res);
							articles = art;
						}

					}
					else
					{
						articles = GetArticlesFromListBox();
					}
					if (articles != null && articles.Any())
					{
						IocHelper.ArticleService.DeleteManyArticles(articles, IocHelper.GroupService);
					}
					else if (_viewingArticle != null)
					{
						var lst = new List<Article>
						{
							_viewingArticle
						};

						IocHelper.ArticleService.DeleteManyArticles(lst, IocHelper.GroupService);
						ShowMessage(string.Format("Статья \"{0}\" успешно удалена.", _viewingArticle.ArticleName));

						Articles.Remove(_viewingArticle);
						CreateListOfArticles(Articles);
						ListOfArticle.Items.Refresh();
						InvokeCounter();
					}

					if (articles != null && articles.Any())
					{
						string joinedNames = string.Join(",", articles.Select(c => c.ArticleName));
						ShowMessage(string.Format("Статьи \"{0}\" успешно удалены.", joinedNames));
						var forRemovinf = Article.Clone(articles);
						foreach (var article in forRemovinf)
						{
							var index = Articles.IndexOf(Articles.Find(c => c.ArticleId == article.ArticleId));
							Articles.RemoveAt(index);
						}
						if (_isGroupsVisible)
						{
							TreeViewHelper.Instance.FillTreeView(Groups, Articles, ArticleTreeView);
							TreeViewHelper.Instance.SelectAndExpand(ArticleTreeView, Articles.FirstOrDefault());
						}
						else
						{
							CreateListOfArticles(Articles);
						}
						InvokeCounter();
					}

					JustHelper.ToggleToRightSource(isXml);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void ListOfArticle_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				ListBoxItem item;
				var lstItem = sender as ListBox;
				var elem = lstItem.InputHitTest(e.GetPosition(lstItem)) as UIElement;

				while (elem != lstItem)
				{
					if (elem is ListBoxItem)
					{
						item = (ListBoxItem)elem;
						lstItem.SelectedItems.Clear();
						int id = int.Parse(item.Name.Replace("article_", ""));
						if (IocHelper.ArticleService == null)
						{
							JustHelper.ToggleToRightSource(isXml);
						}
						var article = IocHelper.ArticleService.GetArticleById(id);
						_viewingArticle = article;
						FillContent(article);
						CreateMatchingSelection(Finder.Text);
						item.IsSelected = true;
						return;
					}
					elem = (UIElement)VisualTreeHelper.GetParent(elem);
				}
			}
			catch (Exception ex)
			{

				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
			}

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

						int id = int.Parse(item.Name.Replace(IMAGE_NAME_SUFFIX, ""));
						var image = Articles.First(c => c.Images.FirstOrDefault(x => x.ImageId == id) != null).Images.First(c => c.ImageId == id);
						ImageView view = new ImageView(image);
						view.ShowDialog();
						return;
					}
					elem = (UIElement)VisualTreeHelper.GetParent(elem);
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
			}
		}
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
		private void Application_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
		{
			if (e.Exception is System.Net.WebException)
			{
				MessageBox.Show("Сайт " + e.Uri.ToString() + " не доступен :(");
				// Нейтрализовать ошибку, чтобы приложение продолжило свою работу
				e.Handled = true;
			}
		}
		private void SortButton_Click(object sender, RoutedEventArgs e)
		{
			if (!isOrdered)
			{
				Articles = IocHelper.ArticleService.GetAllArticles(c => c.ArticleName).ToList();
				isOrdered = !isOrdered;
			}
			else
			{
				Articles = IocHelper.ArticleService.GetAllArticles(c => c.Link).ToList();
				isOrdered = !isOrdered;
			}
			CreateListOfArticles(Articles);
		}
		private void IncreaseFontButton_Click(object sender, RoutedEventArgs e)
		{
			if (Description.FontSize < 126)
			{
				Description.FontSize++;
			}
			FontSizeLabel.Content = Description.FontSize;
		}
		private void DecreaseFontButton_Click(object sender, RoutedEventArgs e)
		{
			if (Description.FontSize > 1)
			{
				Description.FontSize--;
			}
			FontSizeLabel.Content = Description.FontSize;
		}
		private void Statisticks_OnClick(object sender, RoutedEventArgs e)
		{
			
			if (_viewingArticle != null)
			{
				var stat = new Views.Statistics(_viewingArticle);
				stat.ShowDialog();
			}
			else
			{
				var article = GetArticlesFromListBox().FirstOrDefault();
				var stat = new Views.Statistics(article);
				stat.ShowDialog();
			}
		}
		private void Thumb_OnDragCompleted(object sender, DragCompletedEventArgs e)
		{
			StretchImages();
		}
		private void Plan_OnClick(object sender, RoutedEventArgs e)
		{
			var plan = new Views.Plan(isXml);
			plan.ShowDialog();
		}
		private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
		{
			var result = sender as MenuItem;
			result.Foreground = Brushes.Black;
		}
		private void MenuItem_MouseLEave(object sender, MouseEventArgs e)
		{
			var result = sender as MenuItem;
			result.Foreground = Brushes.White;
		}
		private void Button_MouseEnter(object sender, MouseEventArgs e)
		{
			var res = sender as Button;
			res.Background = _buttonBackgroundSelection;
			res.Foreground = Brushes.Black;
		}
		private void Button_MouseLeave(object sender, MouseEventArgs e)
		{
			var res = sender as Button;
			res.Background = _buttonBackgroundInit;
			res.Foreground = _initialForegroundColorBrush;
			res.Focusable = false;
		}
		private void Menu_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var res = sender as MenuItem;
			if (!res.IsEnabled)
			{
				res.Background = Brushes.DarkGray;
				res.Foreground = Brushes.Gray;
			}
			else
			{
				res.Foreground = Brushes.White;
				res.Background = new LinearGradientBrush
				{
					StartPoint = new Point(0.5, 0),
					EndPoint = new Point(0.5, 1),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(Colors.Black,0),
						new GradientStop(Colors.White,1),
						new GradientStop(Color.FromArgb(0xFF,0x29,0x29,0x29),0.487)
					}
				};
			}
		}
		private void TreeViewChecker_OnClick(object sender, RoutedEventArgs e)
		{
			_isGroupsVisible = !_isGroupsVisible;
			ShowAllMenuItem.IsEnabled = !_isGroupsVisible;
			SelectAll.IsEnabled = !_isGroupsVisible;
			ChooseAndFillView(Groups, Articles);
			if (!isXml && !_isGroupsVisible)
			{
				SortButton.Visibility = Visibility.Visible;
			}
			else
			{
				SortButton.Visibility = Visibility.Hidden;
			}
		}
		private void ArticleTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				var view = sender as TreeView;
				var res = TreeViewHelper.Instance.GetArticleFromTreeView(view, Articles);
				if (res != null)
				{
					FillContent(res);
					_viewingArticle = res;
					TreeViewHelper.Instance.SelectAndExpand(view, res);
					//CreateMatchingSelection(Finder.Text);
				}

				var enabled = Articles.Count > 0;
				Remove.IsEnabled = enabled || _viewingArticle != null;
				Edit.IsEnabled = enabled || _viewingArticle != null;
				Statisticks.IsEnabled = enabled || _viewingArticle != null;
			}
			catch (Exception exception)
			{
				ShowMessage(exception.Message, "Ошибка", MessageBoxImage.Error);
			}
		}
		private void SelectAll_OnClick(object sender, RoutedEventArgs e)
		{
			if (!_isGroupsVisible)
			{
				if (SelectAll.IsChecked)
				{
					foreach (var item in ListOfArticle.Items)
					{
						item.MaybeAs<ListBoxItem>().Do((c) =>
						{
							c.IsSelected = true;
						});
					}
				}
				else
				{
					foreach (var item in ListOfArticle.Items)
					{
						item.MaybeAs<ListBoxItem>().Do((c) =>
						{
							c.IsSelected = false;
						});
					}
				}
			}
		}
		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				UserConfigService.ResetLogin();
				this.Close();
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
			}

		}
		private void EmailSender_OnClick(object sender, RoutedEventArgs e)
		{
			var before = EmailSender.Header;
			try
			{
				EmailSender.Header = "Подаждите идет загрузка";
				IocHelper.ToggleToXml();
				Task task1 = Task.Factory.StartNew(() =>
				{
					this.Dispatcher.Invoke((Action) (() =>
					{
						EmailSender.IsEnabled = false;
					}));
					Try(() => EmailService.Instance.SendMail("shkorodenok@gmail.com", "Говно", "Получи говно"),
						() => this.Dispatcher.Invoke((Action) (() =>
						{
							EmailSender.IsEnabled = true;
							EmailSender.Header = before.ToString();
						})));
				});
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
			}

		}
		#endregion

		#region Helper Methods
		private void InitListOfArticles()
		{
			var result = IocHelper.ArticleService.GetAllArticles(c => c.ArticleName);
			var groups = IocHelper.GroupService.GetAllGroups(c => c.GroupId);
			Articles = result == null ? new List<Article>() : result.ToList();
			Groups = groups == null ? new List<ArticleGroup>() : groups.ToList();
			ChooseAndFillView(groups, result);
		}

		private void ChooseAndFillView(IEnumerable<ArticleGroup> groups, IEnumerable<Article> result)
		{
			if (_isGroupsVisible)
			{
				TreeViewHelper.Instance.FillTreeView(groups, result, ArticleTreeView);
				ArticleTreeView.Visibility = Visibility.Visible;
				ListOfArticle.Visibility = Visibility.Hidden;
			}
			else
			{
				CreateListOfArticles(result);
				ArticleTreeView.Visibility = Visibility.Hidden;
				ListOfArticle.Visibility = Visibility.Visible;
			}
			InvokeCounter();
			if (Articles.Any())
			{
				FillContent(Articles.First());
				_viewingArticle = Articles.First();
				if (_isGroupsVisible)
				{
					TreeViewHelper.Instance.SelectAndExpand(ArticleTreeView, _viewingArticle);
				}
				else
				{
					ListOfArticle.SelectedItem = ListOfArticle.Items[0];
				}

			}
		}

		private void StretchImages()
		{
			foreach (var item in ImageListBox.Items)
			{
				var res = item as Image;
				if (res != null)
				{
					res.Width = ImageListBox.ActualWidth;
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
				myBitmapImage.DecodePixelWidth = (int)ImageListBox.ActualWidth;
				myBitmapImage.EndInit();
				image.Source = myBitmapImage;

			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
			}
		}
		void MainWindow_ArticlesCountChanged(object sender, EventArgs e)
		{
			Count.Content = Articles.Count;
		}
		private void CreateListOfArticles(IEnumerable<Article> result)
		{
			var listOfIndex = new List<int>();
			result = result ?? new List<Article>();
			ListOfArticle.SelectedItems.MaybeAs<IList<object>>().If(c => c.Any()).Do(c =>
			{ listOfIndex.AddRange(c.Select(o => ListOfArticle.Items.IndexOf(o as ListBoxItem))); }, () => ListOfArticle.Items.Clear());
			if (listOfIndex.Any())
			{
				foreach (var i in listOfIndex)
				{
					ListOfArticle.Items[i].MaybeAs<ListBoxItem>().Bind(c => c.IsSelected = false);
				}
				ListOfArticle.Items.Clear();
			}
			foreach (var article in result)
			{
				ListOfArticle.Items.Add(new ListBoxItem
				{
					Margin = new Thickness(0, 2, 0, 0),
					Content = article.ArticleName,
					Name = "article_" + article.ArticleId,
					//Foreground = Brushes.White,
					ToolTip = article.ArticleName,
					FontWeight = FontWeights.Bold,
					Background = new LinearGradientBrush
					{
						GradientStops = new GradientStopCollection()
						{
							new GradientStop(Colors.White, 0.213),
							new GradientStop(Color.FromArgb(0xFF, 0x63, 0x63, 0x63), 1),
						},
						StartPoint = new Point(0.5, 0),
						EndPoint = new Point(0.5, 1),
						MappingMode = BrushMappingMode.RelativeToBoundingBox,
					},
					Height = 25,
					FontSize = 14
				});
			}
			ListOfArticle.Items.Refresh();
		}
		private void SaveArticles()
		{
			bool assert = false;
			if (!_isGroupsVisible)
			{
				GetArticlesFromListBox().NothingIfNull().Do((c) =>
				{

					if (c.Any())
					{
						IocHelper.ArticleService.SaveManyArticles(c);
						MessageBox.Show("Данные сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
						assert = true;
					}
				});
			}
			else
			{
				TreeViewHelper.Instance.GetArticleFromTreeView(ArticleTreeView, Articles).NothingIfNull().Do((c) =>
				{
					if (c != null)
					{
						IocHelper.ArticleService.SaveArticle(c);
						MessageBox.Show("Данные сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
						assert = true;
					}
				});
			}

			if (!assert)
			{
				MessageBox.Show("Не выбраны данные для сохранения.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
		private void ShowMessage(string message, string title = "Сообщение", MessageBoxImage boxImage = MessageBoxImage.Information)
		{
			var task = Task.Factory.StartNew(() => MessageBox.Show(message, title, MessageBoxButton.OK, boxImage));
		}
		private IEnumerable<Article> GetArticlesFromListBox()
		{
			var ids =
				ListOfArticle.SelectedItems.MaybeAs<IList<object>>()
					.Bind(c => c.Select(x => int.Parse((x as ListBoxItem).Name.Replace("article_", ""))));

			var articles = ids.Bind(c => Articles.Where(x => c.Contains(x.ArticleId))).GetOrDefault(null);
			return articles;
		}
		private void InvokeCounter()
		{
			if (ArticlesCountChanged != null)
			{
				ArticlesCountChanged.Invoke(this, new EventArgs());
			}
		}
		private void FillContent(Article article)
		{
			if (article != null)
			{
				Description.Text = article.InitialText;
				Hyperlink.NavigateUri = new Uri(article.Link);
				HyperlinkText.Text = article.Link;
				HyperlinkText.ToolTip = article.Link;
				AuthorName.Content = article.AuthorName;
				AuthorName.ToolTip = article.AuthorName;
				Remove.IsEnabled = ListOfArticle.SelectedItems.Count > 0 || _viewingArticle == null;
				ImageListBox.Items.Clear();
				if (article.Images != null)
				{
					foreach (var img in article.Images)
					{
						var image = new Image();
						image.Stretch = Stretch.Fill;
						image.Name = IMAGE_NAME_SUFFIX + img.ImageId.ToString();
						CreateBitmapImageFromData(img.Data, image);
						image.Margin = new Thickness(0, 0, 0, 10);
						ImageListBox.Items.Add(image);
					}
				}
			}
		}
		private void CreateMatchingSelection(string value)
		{
			if (Description.Text.Contains(value))
			{
				Description.SelectionStart = Description.Text.IndexOf(value);
				Description.SelectionLength = value.Length;
				Description.Focus();
			}
		}
		private  void Try(Action action, Action exceptionAction)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				exceptionAction();
				ShowMessage(ex.Message, "Ошибка", MessageBoxImage.Error);
				throw;
			}
		}
		#endregion
	}
}
