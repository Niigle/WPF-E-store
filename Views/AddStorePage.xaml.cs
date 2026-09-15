using E_store.Models;
using E_store.Repository;
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

namespace E_store.Views
{
    public class ManagerUser
    {
        public uint Id { get; set; }
        public string DisplayText { get; set; }
    }

    /// <summary>
    /// Interaction logic for AddStorePage.xaml
    /// </summary>
    public partial class AddStorePage : Page
    {
        private readonly StoreRepository storeRepository = new StoreRepository();
        private readonly RoleRepository roleRepository = new RoleRepository();

        public AddStorePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetManagers();
        }

        private void GetManagers()
        {
            List<User> managers = roleRepository.GetUsersByRole("Manager");

            var manager = managers.Select(m => new ManagerUser
            {
                Id = m.Id,
                DisplayText = $"{m.FirstName} {m.LastName} ({m.Username})"
            }).ToList();

            CmbManager.ItemsSource = manager;

            if (manager.Count == 0)
            {
                LblMessage.Foreground = System.Windows.Media.Brushes.Red;
                LblMessage.Text = "No manager available. Add a new manager.";
            }
        }

        private void BtnAddStore_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Red;
            LblMessage.Text = "";

            string name = TxtName.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string type = TxtType.Text.Trim();
            string phone = TxtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(phone))
            {
                LblMessage.Text = "All fields are mandatory.";
                return;
            }

            if (CmbManager.SelectedValue == null)
            {
                LblMessage.Text = "Choose a manager.";
                return;
            }

            try
            {
                Store newStore = new Store
                {
                    Name = name,
                    Address = address,
                    Type = type,
                    Phone = phone,
                    Manager = (uint)CmbManager.SelectedValue,
                    IsActive = ChkActive.IsChecked == true ? 1 : 0
                };

                storeRepository.AddStore(newStore);

                LblMessage.Foreground = System.Windows.Media.Brushes.Green;
                LblMessage.Text = "Store is added successfully.";
                ClearForm();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessage.Text = "Error at creating store: " + ex.Message;
            }
        }

        private void ClearForm()
        {
            TxtName.Clear();
            TxtAddress.Clear();
            TxtType.Clear();
            TxtPhone.Clear();
            CmbManager.SelectedIndex = -1;
            ChkActive.IsChecked = true;
        }
    }
}
