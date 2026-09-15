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
    /// <summary>
    /// Interaction logic for StoreListPage.xaml
    /// </summary>
    public partial class StoreListPage : Page
    {

        private readonly StoreRepository storeRepository = new StoreRepository();

        public StoreListPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var stores = storeRepository.GetAllStores();
                GridStores.ItemsSource = stores;

                if (stores.Count == 0)
                    LblMessage.Text = "No stores available.";
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessage.Text = "Error at loading stores: " + ex.Message;
            }
        }
    }
}
