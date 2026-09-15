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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static QuestPDF.Helpers.Colors;

namespace E_store.Views
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = AuthService.Instance.CurrentUser;
            LblIme.Text = $"Name and surname: {user.FirstName} {user.LastName}";
            LblEmail.Text = $"Email: {user.Email}";
            LblUsername.Text = $"Username: {user.Username}";
            LblAdresa.Text = $"Address: {user.Address}";
            LblTelephone.Text = $"Telephone: {user.Phone}";
        }
    }
}
