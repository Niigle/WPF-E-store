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

using E_store.Models;
using E_store.Services;

namespace E_store.Views
{
    /// <summary>
    /// Interaction logic for CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        private readonly OrderRepository orderRepository = new OrderRepository();
        private Order currentCart;

        public CartPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            var user = AuthService.Instance.CurrentUser;
            currentCart = orderRepository.GetOrCreateActiveCart(user.Id);

            var orderItems = orderRepository.GetOrderItems(currentCart.Id);
            GridCart.ItemsSource = orderItems;

            decimal totalAmount = 0;
            foreach (var orderItem in orderItems) totalAmount += orderItem.TotalAmount;
            LblTotal.Text = $"Total: {totalAmount:N2} RSD";
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var orderItem = (OrderItem)((Button)sender).Tag;
            orderRepository.RemoveFromCart(orderItem.Id, currentCart.Id);
            CartNotifier.Notify();
            RefreshView();
        }

        private void BtnCompletePurchase_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Red;
            LblMessage.Text = "";

            var (successful, message) = orderRepository.FinishOrder(currentCart.Id);

            if (successful)
            {
                MessageBox.Show(message);
                CartNotifier.Notify();
                RefreshView();
            }
            else
            {
                LblMessage.Text = message;
            }
        }
    }
}
