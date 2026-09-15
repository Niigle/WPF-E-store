using E_store.Models;
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
    /// Interaction logic for AddManagerWindow.xaml
    /// </summary>
    public partial class AddManagerPage : Page
    {
        private readonly AuthService authService = AuthService.Instance;
        public AddManagerPage()
        {
            InitializeComponent();
        }

        private void BtnAddManager_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Red;
            LblMessage.Text = "";

            string firstName = TxtFirstName.Text.Trim();
            string lastName = TxtLastName.Text.Trim();
            string username = TxtUsername.Text.Trim();
            string email = TxtEmail.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password))
            {
                LblMessage.Text = "All fields are mandatory.";
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                LblMessage.Text = "Email format is incorrect.";
                return;
            }

            try
            {
                User newManager = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Email = email,
                    Address = address,
                    Phone = phone,
                    Password = password
                };

                var (successful, message) = authService.AddManager(newManager);

                if (successful)
                {
                    LblMessage.Foreground = System.Windows.Media.Brushes.Green;
                    LblMessage.Text = message;
                    ClearForm();
                }
                else
                {
                    LblMessage.Text = message;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessage.Text = "Error while adding a manager: " + ex.Message;
            }
        }

        private void ClearForm()
        {
            TxtFirstName.Clear();
            TxtLastName.Clear();
            TxtUsername.Clear();
            TxtEmail.Clear();
            TxtAddress.Clear();
            TxtPhone.Clear();
            TxtPassword.Clear();
        }
    }
}
