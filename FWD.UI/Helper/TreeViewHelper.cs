using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Helper
{
	public class TreeViewHelper
	{
		private static TreeViewHelper _instance;

		private string _suffix = "Article_";

		public static TreeViewHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TreeViewHelper();
				}
				return _instance;
			}
		}

		private TreeViewHelper()
		{
			
		}

		public void FillTreeView(IEnumerable<ArticleGroup> groups, IEnumerable<Article> articles, TreeView treeView)
		{
			treeView.Items.Clear();
			var res = GroupHelper.Instance.GetGroups(groups, articles);
			foreach (var groupViewItem in res.GroupViewItems)
			{
				var treeViewItem = new TreeViewItem
				{
					Header = string.Format("{0}({1})", groupViewItem.Name, groupViewItem.ArticlesCount),
					ToolTip = string.Format("{0}({1})", groupViewItem.Name, groupViewItem.ArticlesCount),
					FontSize = 14,
					BorderThickness = new Thickness(2),
					FontWeight = FontWeights.Bold,
					HorizontalAlignment = HorizontalAlignment.Stretch,
				};

				DependencyProperty pr = Border.CornerRadiusProperty;
				var style = new Style(typeof(TreeViewItem));
				var borderStyle = new Style(typeof(Border));
				borderStyle.Setters.Add(new Setter(pr, new CornerRadius(10)));
				style.Resources.Add("", borderStyle);
				var trigger = new Trigger
				{
					Property = TreeViewItem.IsSelectedProperty,
					Value = true,
				};
				trigger.Setters.Add(new Setter(TreeViewItem.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(0x7d, 0xA2, 0xce))));
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

		public Article GetArticleFromTreeView(TreeView view, IEnumerable<Article> cachedArticles)
		{
			var item = view.SelectedItem as TreeViewItem;
			var res = string.Empty;
			if (item != null)
			{
				res = item.Name;
			}
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

		public ArticleGroup GetGroupFromTreeView(TreeView view, IEnumerable<Article> cachedArticles)
		{
			var item = view.SelectedItem as TreeViewItem;
			if (item != null)
			{
				var strItem = item.Header.ToString().Split('(')[0];
				var group = IocHelper.GroupService.GetGroupsByParams(c => c.GroupName.Equals(strItem), c => c.GroupId).First();
				group.Articles = GroupHelper.Instance.InitGroup(group, cachedArticles).Articles;
				return group;
			}
			return null;
		}

		public void SelectAndExpand(TreeView view, Article selectedArticle)
		{
			if (selectedArticle != null)
			{
				foreach (var item in view.Items)
				{
					var converted = item as TreeViewItem;
					foreach (var item1 in converted.Items)
					{
						var converted1 = item1 as TreeViewItem;
						if (converted1.Header.ToString() == selectedArticle.ArticleName)
						{
							converted.IsExpanded = true;
							converted1.IsSelected = true;
							break;
						}
					}
				}
			}
		}
	}
}
