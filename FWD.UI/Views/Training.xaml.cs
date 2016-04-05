using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using FWD.BusinessObjects.Domain;
using FWD.UI.Helper;

namespace FWD.UI.Views
{
	/// <summary>
	/// Interaction logic for Training.xaml
	/// </summary>
	public partial class Training : Window
	{
		public Training(IEnumerable<ArticleGroup> groups, IEnumerable<Article> articles)
		{
			try
			{
				InitializeComponent();
				FillTreeView(groups, articles, ArticleTreeView);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		
		}

		private string  _suffix = "Article_";
		private void FillTreeView(IEnumerable<ArticleGroup> groups, IEnumerable<Article> articles,TreeView treeView)
		{
			treeView.Items.Clear();
			var res = GroupHelper.Instance.GetGroups(groups, articles);
			foreach (var groupViewItem in res.GroupViewItems)
			{
				var treeViewItem = new TreeViewItem
				{
					Header = groupViewItem.Name,
					ToolTip = groupViewItem.Name,
					FontSize = 14,
					BorderThickness = new Thickness(2),
					FontWeight = FontWeights.Bold,
					HorizontalAlignment = HorizontalAlignment.Stretch,
				};

				DependencyProperty pr = Border.CornerRadiusProperty;
				var style = new Style(typeof (TreeViewItem));
				var borderStyle = new Style(typeof (Border));
				borderStyle.Setters.Add(new Setter(pr, new CornerRadius(10)));
				style.Resources.Add("", borderStyle);
				var trigger = new Trigger
				{
					Property = TreeViewItem.IsSelectedProperty,
					Value = true,
				};
				trigger.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(Color.FromRgb(0x7d, 0xA2, 0xce))));
				style.Triggers.Add(trigger);
				treeViewItem.ItemContainerStyle = style;

				foreach (var article in groupViewItem.Articles)
				{
					treeViewItem.Items.Add(new TreeViewItem
					{
						Header = article.ArticleName,
						ToolTip = article.ArticleName,
						Name = _suffix + article.ArticleId,
						FontSize = 14,
						BorderThickness = new Thickness(2),
						FontWeight = FontWeights.Bold,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						ItemContainerStyle = style
					});
				}
				treeView.Items.Add(treeViewItem);
			}
		}

		public Article GetArticleFromTreeView(TreeView view,IEnumerable<Article> cachedArticles)
		{
			var res = (view.SelectedItem as TreeViewItem).Name;
			if (!string.IsNullOrEmpty(res))
			{
				var id = int.Parse(res.Replace(_suffix, ""));
				var resuslt = cachedArticles.FirstOrDefault(c => c.ArticleId == id);
				if (resuslt == null)
				{
					resuslt = IocHelper.ArticleService.GetArticleById(id);
				}
				return resuslt;
			}
			return null;
		}


		private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var name = (e.NewValue as TreeViewItem).Name;
		}
	}
}
