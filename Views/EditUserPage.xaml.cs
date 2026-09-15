using E_store.Connection;
using E_store.Models;
using E_store.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for EditUserPage.xaml
    /// </summary>
    public partial class EditUserPage : Page
    {

        private readonly UserRepository userRepository = new UserRepository();
        private readonly RoleRepository roleRepository = new RoleRepository();

        private User selectedUser;
        private ObservableCollection<RoleCheckboxItem> roles;

        public EditUserPage()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Red;
            LblMessage.Text = "";
            PanelEdit.Visibility = Visibility.Collapsed;

            string username = TxtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                LblMessage.Text = "Add a username.";
                return;
            }

            selectedUser = userRepository.GetByUsername(username);

            if (selectedUser == null)
            {
                LblMessage.Text = "User with selected username doesn't exist.";
                return;
            }

            TxtFirstName.Text = selectedUser.FirstName; 
            TxtLastName.Text = selectedUser.LastName;
            TxtUsername.Text = selectedUser.Username;
            TxtEmail.Text = selectedUser.Email;
            TxtAddress.Text = selectedUser.Address;
            TxtPhone.Text = selectedUser.Phone;
            //TxtPassword.Password = "";

            GetRoles();
            GetPermissions();

            PanelEdit.Visibility = Visibility.Visible;
        }

        private void GetRoles()
        {
            var allRoles = roleRepository.GetAllRoles();
            var userRoles = roleRepository.GetRolesForUser(selectedUser.Id)
                                            .Select(r => r.Id)
                                            .ToHashSet();

            roles = new ObservableCollection<RoleCheckboxItem>(
                allRoles.Select(r => new RoleCheckboxItem
                {
                    RoleId = r.Id,
                    RoleName = r.RoleName,
                    isChecked = userRoles.Contains(r.Id)
                })
            );

            Roles.ItemsSource = roles;
        }

        private void GetPermissions()
        {
            var permissions = roleRepository.GetPermissionsForUser(selectedUser.Id);

            LblPermissions.Text = permissions.Count > 0
                ? string.Join(", ", permissions.Select(p => p.Description))
                : "No permission granted.";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Red;
            LblMessage.Text = "";

            if (selectedUser == null)
                return;

            string firstName = TxtFirstName.Text.Trim();
            string lastName = TxtLastName.Text.Trim();
            string username = TxtUsername.Text.Trim();
            string email = TxtEmail.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            //string newPassword = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone))
            {
                LblMessage.Text = "All fields (except password) are mandatory.";
                return;
            }

            try
            {
                selectedUser.FirstName = firstName;
                selectedUser.LastName = lastName;
                selectedUser.Username = username;
                selectedUser.Email = email;
                selectedUser.Address = address;
                selectedUser.Phone = phone;

                userRepository.EditUser(selectedUser);
/*
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    userRepository.UpdatePassword(selectedUser.Id, newPassword);
                }*/

                foreach (var role in roles)
                {
                    bool hasRole = roleRepository.HasRole(selectedUser.Id, role.RoleId);

                    if (role.isChecked && !hasRole)
                        roleRepository.AddRoleToUser(selectedUser.Id, role.RoleId);
                    else if (!role.isChecked && hasRole)
                        roleRepository.RemoveUserRole(selectedUser.Id, role.RoleId);
                }

                GetPermissions();

                LblMessage.Foreground = System.Windows.Media.Brushes.Green;
                LblMessage.Text = "User edited successfully.";
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessage.Text = "Error at editing user: " + ex.Message;
            }
        }
    }
}
