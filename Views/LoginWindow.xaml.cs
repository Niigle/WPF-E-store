using E_store.Repository;
using E_store.Services;
using E_store.Views;
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
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameInput.Text;
            string password = passwordInput.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                LblMessage.Text = "Insert username and password.";
                return;
            }

            /*UserRepository userRepository = new UserRepository();
            bool loginSuccessful = userRepository.CheckLogin(username, password);*/

            bool loginSuccessful = AuthService.Instance.Login(username, password);

            if (loginSuccessful)
            {
                //posevban admin/user/manager stranica
                //MainWindow main = new MainWindow();
                //main.Show();
                //this.Close();
                this.DialogResult = true;
            }
            else
            {
                LblMessage.Text = "Incorrect username or password.";
            }
        }

        private void BtnOpenRegistrationPage_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registration = new RegisterWindow();
            bool? result = registration.ShowDialog();

            if (result == true)
            {
                LblMessage.Text = "Registration successful, log in.";
                LblMessage.Foreground = System.Windows.Media.Brushes.Green;
            }
        }
    }
}
