using E_store.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace E_store.Views
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();

            if (!AuthService.Instance.HasRole("Admin"))
            {
                MessageBox.Show("You don't have permission to access this page.");
                this.Close();
                return;
            }

            var korisnik = AuthService.Instance.CurrentUser;
            LblWelcome.Text = $"Welcome, {korisnik.FirstName} {korisnik.LastName}";
        }

        private void BtnAddStore_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new AddStorePage());
        }

        private void BtnAddManager_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new AddManagerPage());
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new EditUserPage());
        }

        private void BtnViewStores_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new StoreListPage());
        }
        private void BtnMonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new ReportsPage());
            // MainFrame.Navigate(new MonthlyReportPage());
        }

        private void BtnDailyReport_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new ReportsPage());
            // MainFrame.Navigate(new DailyReportPage());
        }
    }
}
