using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;
using FWD.UI.Helper;
using Microsoft.SqlServer.Server;
using WebRock.Utils.Monad;

namespace FWD.UI.Views
{
	/// <summary>
	/// Interaction logic for Plan.xaml
	/// </summary>
	public partial class Plan : Window
	{
		#region Fields and Properties
		private List<IPlan> _plans;

		private const string FORMAT = "Страница {0} из {1}";

		private static int _fontSize = 12;
		private static int _windowHeight;

		private int _skip = 0;

		private bool _isXml;

		private bool _isFirstTime = true;
		#endregion
		
		public Plan(bool isXml)
		{
			InitializeComponent();
			_isXml = isXml;
			var res = IocHelper.PlanService.GetAllPlans(c => c.Id, _skip, 10);
			_plans = res == null ? new List<IPlan>() : res.ToList();
			InitPagerInfo();
			DeleteOldPlans();
			_windowHeight = (int)this.Height;
			PlanDataGrid.ItemsSource = _plans;
			
		}

		#region Controls Event's

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			_isFirstTime = false;
		}
		protected override void OnClosed(EventArgs e)
		{
			if (_isXml)
			{
				IocHelper.ToggleToXml();
			}
			else
			{
				IocHelper.ToggleToDb();
			}
			_isFirstTime = true;
			base.OnClosed(e);
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{

			try
			{
				Validate();
				var plan = new CurrentPlan()
				{
					Description = DescriptionTextBox.Text,
					AddedDate = DateTime.Now,
					Name = NameTextBox.Text
				};
				IocHelper.PlanService.SavePlan(plan);
				_plans.Add(plan);
				PlanDataGrid.ItemsSource = _plans.Skip(_skip).Take(10);
				PlanDataGrid.Items.Refresh();

				InitPagerInfo();
				DescriptionTextBox.Text = "";
				NameTextBox.Text = "";
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void PlanDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				if (MessageBox.Show("Удалить?", "Сообщение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					var bufferList = new List<IPlan>();
					foreach (var selectedItem in PlanDataGrid.SelectedItems)
					{
						var res = selectedItem as IPlan;
						if (res != null)
						{
							bufferList.Add(res);
							IocHelper.PlanService.DeletePlan(res);
						}
					}
					foreach (var plan in bufferList)
					{
						_plans.Remove(plan);
					}
					InitPagerInfo();
					PlanDataGrid.ItemsSource = _plans.ToList();
					PlanDataGrid.Items.Refresh();

				}
			}
		}

		private void PrevButton_Click(object sender, RoutedEventArgs e)
		{
			_skip -= 10;
			if (_skip / 10 >= 0)
			{
				var res = IocHelper.PlanService.GetAllPlans(c => c.Id, _skip, 10);
				_plans = res == null ? null : res.ToList();
				PlanDataGrid.ItemsSource = _plans;
				PlanDataGrid.Items.Refresh();
				InitPagerInfo();
			}
			else
			{
				_skip += 10;
			}
		}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			_skip += 10;
			var allPages = (int)IocHelper.PlanService.TotalCount / 10;
			if (IocHelper.PlanService.TotalCount % 10 != 0)
			{
				allPages += 1;
			}
			if (_skip / 10 < allPages)
			{
				var res = IocHelper.PlanService.GetAllPlans(c => c.Id, _skip, 10);
				_plans = res == null ? null : res.ToList();
				PlanDataGrid.ItemsSource = _plans;
				PlanDataGrid.Items.Refresh();
				InitPagerInfo();
			}
			else
			{
				_skip -= 10;
			}
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (!_isFirstTime)
			{
				UpdateIsDone(sender, e);
			}
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!_isFirstTime)
			{
				UpdateIsDone(sender, e);
			}
		}

		private void Plan_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			JustHelper.MakeFontSizeStretchable(_fontSize, _windowHeight, (int)e.NewSize.Height,null, FormatLabel, NextButton, PrevButton);
		}

		private void FindButton_Click(object sender, RoutedEventArgs e)
		{
			var str = new string(SearchBox.Text.ToCharArray());
			if (!string.IsNullOrEmpty(str))
			{
				var res = IocHelper.PlanService.GetPlansByParams(c => c.Description.Contains(str) || c.Name.Contains(str),
					c => c.Id, _skip, 10);
				_plans = res == null ? new List<IPlan>() : res.ToList();
			}
			else
			{
				var res = IocHelper.PlanService.GetAllPlans(c => c.Id, _skip, 10);
				_plans = res == null ? new List<IPlan>() : res.ToList();
			}
			PlanDataGrid.ItemsSource = _plans;
			InitPagerInfo();
		}
		#endregion

		#region Helpers
		private void DeleteOldPlans()
		{
			var deletingPlans = _plans.Where(c => c.PossibleChangeDate != default(DateTime));
			var lst = new List<IPlan>();
			lst.AddRange(deletingPlans);
			foreach (var deletingPlan in lst)
			{
				var span = DateTime.Now - deletingPlan.PossibleChangeDate.Value;
				if (span.Days > 3 && deletingPlan.IsDone)
				{
					IocHelper.PlanService.DeletePlan(deletingPlan);
					_plans.Remove(deletingPlan);
				}
			}
		}

		private void InitPagerInfo()
		{
			var allPages = (int)IocHelper.PlanService.TotalCount / 10;
			if (IocHelper.PlanService.TotalCount % 10 != 0)
			{
				allPages += 1;
			}
			var currentPage = (_skip + 10) / 10;
			if (currentPage > allPages)
			{
				currentPage = allPages;
				_skip = currentPage * 10;
			}
			FormatLabel.Content = string.Format(FORMAT, currentPage, allPages);
		}

		private void Validate()
		{
			try
			{
				var builder = new StringBuilder();
				if (string.IsNullOrEmpty(DescriptionTextBox.Text))
				{
					builder.Append("Поле \"Описание\" не заполнено.");
				}
				if (string.IsNullOrEmpty(NameTextBox.Text))
				{
					builder.Append("Поле \"Имя\" не заполнено.");
				}
				if (_plans.FirstOrDefault(c => c.Name == NameTextBox.Text) != null)
				{
					builder.Append(string.Format("План с названием \"{0}\" уже существует.", NameTextBox.Text));
				}
				if (!string.IsNullOrEmpty(builder.ToString()))
				{
					throw new Exception(builder.ToString());
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		private void UpdateIsDone(object sender, RoutedEventArgs e)
		{
			var isChecked = (e.Source as CheckBox).IsChecked;
			var plan = (sender as DataGridCell).DataContext as IPlan;
			plan.IsDone = isChecked ?? false;
			var currentPlan = _plans.FirstOrDefault(c => c.Id == plan.Id);
			currentPlan.IsDone = plan.IsDone;
			IocHelper.PlanService.UpdatePlan(plan);
		}
		#endregion	
	}
}
