using System;
using System.Collections.Generic;
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
using FWD.BusinessObjects;
using FWD.BusinessObjects.Domain;
using FWD.UI.Helper;

namespace FWD.UI.Views
{
	/// <summary>
	/// Interaction logic for Statistics.xaml
	/// </summary>
	public partial class Statistics : Window
	{
		private Article _article;
		public Statistics(Article article)
		{
			InitializeComponent();
			_article = article;

			LettersLabel.Content = IocHelper.ArticleHelperService.GetCountOfLetters(_article);
			WordsLabel.Content = IocHelper.ArticleHelperService.GetCountOfWords(_article);
			SentencesLabel.Content = IocHelper.ArticleHelperService.GetCountOfSentences(_article);
			ArticleNameLabel.Content = _article.ArticleName;
			ArticleNameLabel.ToolTip = _article.ArticleName;
		}
	}
}
