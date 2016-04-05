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
using FWD.Services;
using FWD.UI.Helper;

namespace FWD.UI.Views
{
	/// <summary>
	/// Interaction logic for Auth.xaml
	/// </summary>
	public partial class Auth : Window
	{
		public Auth()
		{
			InitializeComponent();
		}

		private void EnterButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (IsAuthCheckBox.IsChecked != null && IsAuthCheckBox.IsChecked == true)
				{
					if (
						MessageBox.Show("Будет доступна только работа с XML.Вы уверены?", "Информация", MessageBoxButton.YesNo,
							MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						UserConfigService.IsMissingAuth = true;
						var mainWindow = new MainWindow();
						this.Close();
					}
				}
				else
				{
					Validate();
					var service = IocHelper.UserService;
					if (UserConfigService.CheckExistingUser(PasswordBox.Password, LoginBox.Text, service))
					{
						var mainWindow = new MainWindow();
						this.Close();
					}
				}
				
			}
			catch (Exception ex)
			{
				ErrorLabel.Content = ex.Message;
				ErrorLabel.ToolTip = ex.Message;
			}
		}

		private void Validate()
		{
			var builder = new StringBuilder();
			if (string.IsNullOrEmpty(PasswordBox.Password))
			{
				builder.Append("Поле \"Пароль\" должно быть заполнено.");
			}
			if (string.IsNullOrEmpty(LoginBox.Text))
			{
				builder.Append("Поле \"Логин\" должно быть заполнено.");
			}
			var rs = builder.ToString();
			if (!string.IsNullOrEmpty(rs))
			{
				throw new Exception(rs);
			}
		}
	}
}
