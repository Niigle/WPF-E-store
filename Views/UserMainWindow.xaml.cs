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
    /// <summary>
    /// Interaction logic for UserMainWindow.xaml
    /// </summary>
    public partial class UserMainWindow : Window
    {
        private readonly OrderRepository orderRepository = new OrderRepository();

        public UserMainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new ProductListPage());

            CartNotifier.CartUpdated += RefreshCart;
            RefreshCart();
        }

        private void RefreshCart()
        {
            var user = AuthService.Instance.CurrentUser;
            var cart = orderRepository.GetOrCreateActiveCart(user.Id);
            int itemNumber = orderRepository.ItemNumberInCart(cart.Id);

            BtnCart.Content = $"Cart ({itemNumber})";
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage());
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CartPage());
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var newPage = new ProductListPage();
                MainFrame.Navigate(newPage);
                newPage.Loaded += (s, args) => newPage.SearchByText(TxtSearch.Text.Trim());
            }
        }
    }
}
