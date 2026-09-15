using E_store.Services;
using System.Windows;

namespace E_store.Views
{
    /// <summary>
    /// Interaction logic for ManagerMainWindow.xaml
    /// </summary>
    public partial class ManagerMainWindow : Window
    {
        public ManagerMainWindow()
        {
            InitializeComponent();

            if (!AuthService.Instance.HasRole("Manager"))
            {
                MessageBox.Show("You don't have a permission to access this page.");
                this.Close();
                return;
            }

            var user = AuthService.Instance.CurrentUser;
            LblWelcome.Text = $"Welcome, {user.FirstName} {user.LastName}";
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

        private void BtnShowStores_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new StoreListPage());
        }
        private void BtnMonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new ManagerReportsPage());
        }

        private void BtnDailyReport_Click(object sender, RoutedEventArgs e)
        {
            LblWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new ManagerReportsPage());
        }
    }
}
