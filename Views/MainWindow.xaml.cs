using E_store.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace E_store.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new ProductListPage());
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();
            
            if (result == true)
            {
                var auth = AuthService.Instance;

                Window nextWindow;

                if (auth.HasRole("Admin"))
                    nextWindow = new AdminMainWindow();
                else if (auth.HasRole("Manager"))
                    nextWindow = new ManagerMainWindow();
                else
                    nextWindow = new UserMainWindow();

                nextWindow.Show();
                this.Close();
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var newPage = new ProductListPage();
                MainFrame.Navigate(newPage);
                newPage.Loaded += (s, args) => newPage.SearchByText(TxtPretraga.Text.Trim());
            }
        }
    }
}