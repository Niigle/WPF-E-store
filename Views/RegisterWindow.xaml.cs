using E_store.Models;
using E_store.Repository;
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
    public partial class RegisterWindow : Window
    {
        private readonly AuthService authService = new AuthService();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Text = "";

            string firstName = TxtFirstName.Text.Trim();
            string lastName = TxtLastName.Text.Trim();
            string username = TxtUsername.Text.Trim();
            string email = TxtEmail.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            string password = TxtPassword.Password;
            string passwordConfirm = TxtPasswordConfirm.Password;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password))
            {
                LblMessage.Text = "All fields are mandatory.";
                return;
            }

            if (password != passwordConfirm)
            {
                LblMessage.Text = "Passwords do not match.";
                return;
            }

            try
            {
                User newUser = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Email = email,
                    Address = address,
                    Phone = phone,
                    Password = password
                };

                var (successful, message) = authService.Register(newUser);

                if (successful)
                {
                    this.DialogResult = true;
                }
                else
                {
                    LblMessage.Text = message;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessage.Text = "Error while registrating: " + ex.Message;
            }
        }
    }
}
