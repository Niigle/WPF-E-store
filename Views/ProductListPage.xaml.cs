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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace E_store.Views
{
    /// <summary>
    /// Interaction logic for ProductListPage.xaml
    /// </summary>
    public partial class ProductListPage : Page
    {

        private readonly StoreProductRepository storeProductRepository = new StoreProductRepository();
        private readonly StoreRepository storeRepository = new StoreRepository();
        private readonly ProductRepository productRepository = new ProductRepository();
        private string currentSearch = "";

        public ProductListPage()
        {
            InitializeComponent();
        }

        private readonly OrderRepository orderRepository = new OrderRepository();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetFilters();
            GetProducts();
        }

        private void GetFilters()
        {
            var stores = storeRepository.GetActiveStores();
            stores.Insert(0, new Store { Id = 0, Name = "All stores" });
            CmbStores.ItemsSource = stores;
            CmbStores.SelectedIndex = 0;

            var types = productRepository.GetAllTypes();
            types.Insert(0, "All categories");
            CmbCategory.ItemsSource = types;
            CmbCategory.SelectedIndex = 0;
        }

        public void SearchByText(string text)
        {
            currentSearch = text;
            GetProducts();
        }

        private void GetProducts()
        {
            uint? storeId = (CmbStores.SelectedValue is uint id && id != 0) ? id : (uint?)null;
            string type = (CmbCategory.SelectedItem as string == "All categories") ? null : CmbCategory.SelectedItem as string;

            decimal? minPrice = decimal.TryParse(TxtPriceFrom.Text, out decimal min) ? min : (decimal?)null;
            decimal? maxPrice = decimal.TryParse(TxtPriceTo.Text, out decimal max) ? max : (decimal?)null;

            var products = storeProductRepository.SearchProducts(currentSearch, storeId, type, minPrice, maxPrice);
            ProductsList.ItemsSource = products;
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            GetProducts();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbStores.SelectedIndex = 0;
            CmbCategory.SelectedIndex = 0;
            TxtPriceFrom.Clear();
            TxtPriceTo.Clear();
            currentSearch = "";
            GetProducts();
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (StoreProductDisplay)button.Tag;

            var stackPanel = (StackPanel)button.Parent;
            var txtQuantity = stackPanel.Children.OfType<TextBox>().First();

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Insert correct quantity.");
                return;
            }

            if (quantity > product.Stock)
            {
                MessageBox.Show($"Not enough in stock (available: {product.Stock}).");
                return;
            }

            var user = AuthService.Instance.CurrentUser;
            var cart = orderRepository.GetOrCreateActiveCart(user.Id);

            orderRepository.AddToCart(cart.Id, product, quantity);
            CartNotifier.Notify();

            MessageBox.Show($"'{product.Name}' is added to cart.");
        }
    }
}
