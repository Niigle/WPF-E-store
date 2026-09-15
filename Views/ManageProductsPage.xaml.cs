using E_store.Models;
using E_store.Repository;
using E_store.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for ManageProductsPage.xaml
    /// </summary>
    public partial class ManageProductsPage : Page
    {
        private readonly StoreRepository storeRepository = new StoreRepository();
        private readonly StoreProductRepository storeProductRepository = new StoreProductRepository();
        private readonly ProductRepository productRepository = new ProductRepository();

        private ObservableCollection<StoreProduct> storeProducts;
        private uint? selectedStoreId;

        public ManageProductsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var manager = AuthService.Instance.CurrentUser;
            var stores = storeRepository.GetStoresByManager(manager.Id);

            CmbStore.ItemsSource = stores;

            if (stores.Count == 0)
            {
                LblMessage.Foreground = System.Windows.Media.Brushes.Red;
                LblMessage.Text = "You don;t have any stores.";
            }
        }

        private void CmbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbStore.SelectedValue == null)
            {
                ContentPanel.Visibility = Visibility.Collapsed;
                return;
            }

            selectedStoreId = (uint)CmbStore.SelectedValue;
            ContentPanel.Visibility = Visibility.Visible;

            GetStoreProducts();
            GetOtherProducts();
        }

        private void GetStoreProducts()
        {
            var products = storeProductRepository.GetStoreProducts(selectedStoreId.Value);
            storeProducts = new ObservableCollection<StoreProduct>(products);
            GridProducts.ItemsSource = storeProducts;
        }

        private void GetOtherProducts()
        {
            var products = productRepository.GetOtherProducts(selectedStoreId.Value);
            CmbExistingProduct.ItemsSource = products;
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Green;
            LblMessage.Text = "";

            try
            {
                foreach (var storeProduct in storeProducts)
                {
                    storeProductRepository.UpdateStockPrice(storeProduct.Id, storeProduct.Price, storeProduct.Stock);
                }

                LblMessage.Text = "Changes saved successfully.";
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessage.Foreground = System.Windows.Media.Brushes.Red;
                LblMessage.Text = "Error while saving: " + ex.Message;
            }
        }

        private void BtnAddExistingProduct_Click(object sender, RoutedEventArgs e)
        {
            LblMessageExisting.Foreground = System.Windows.Media.Brushes.Red;
            LblMessageExisting.Text = "";

            if (CmbExistingProduct.SelectedValue == null)
            {
                LblMessageExisting.Text = "Choose a product.";
                return;
            }

            if (!decimal.TryParse(TxtPriceExistingProduct.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                LblMessageExisting.Text = "Insert correct price.";
                return;
            }

            if (!int.TryParse(TxtQuantityExisting.Text.Trim(), out int quantity) || quantity < 0)
            {
                LblMessageExisting.Text = "Insert correct quantity.";
                return;
            }

            var selectedProduct = (Product)CmbExistingProduct.SelectedItem;

            try
            {
                storeProductRepository.AddToStore(selectedStoreId.Value, selectedProduct.Id,
                                              selectedProduct.Name, price, quantity);

                LblMessageExisting.Foreground = System.Windows.Media.Brushes.Green;
                LblMessageExisting.Text = "Product succesfully added to store.";

                TxtPriceExistingProduct.Clear();
                TxtQuantityExisting.Clear();

                GetStoreProducts();
                GetOtherProducts();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessageExisting.Text = "Error: " + ex.Message;
            }
        }

        private void BtnCreateNewProduct_Click(object sender, RoutedEventArgs e)
        {
            LblMessageNewProduct.Foreground = System.Windows.Media.Brushes.Red;
            LblMessageNewProduct.Text = "";

            string name = TxtNewName.Text.Trim();
            string type = TxtNewType.Text.Trim();
            string description = TxtNewDescription.Text.Trim();
            string barcode = TxtNewBarcode.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
            {
                LblMessageNewProduct.Text = "Name and type are mandatory.";
                return;
            }

            if (!decimal.TryParse(TxtNewPrice.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                LblMessageNewProduct.Text = "Insert correct price.";
                return;
            }

            if (!int.TryParse(TxtNewQuantity.Text.Trim(), out int quantity) || quantity < 0)
            {
                LblMessageNewProduct.Text = "Inser correct quantity.";
                return;
            }

            if (!string.IsNullOrWhiteSpace(barcode) && productRepository.GetByBarcode(barcode) != null)
            {
                LblMessageNewProduct.Text = "Product with this bbarcode already exist.";
                return;
            }

            try
            {
                Product newProduct = new Product
                {
                    Name = name,
                    Type = type,
                    Description = string.IsNullOrWhiteSpace(description) ? null : description,
                    Barcode = barcode
                };

                uint newId = productRepository.AddProduct(newProduct);

                storeProductRepository.AddToStore(selectedStoreId.Value, newId, name, price, quantity);

                LblMessageNewProduct.Foreground = System.Windows.Media.Brushes.Green;
                LblMessageNewProduct.Text = "Product created succesfully and added to store.";

                TxtNewName.Clear();
                TxtNewType.Clear();
                TxtNewDescription.Clear();
                TxtNewBarcode.Clear();
                TxtNewPrice.Clear();
                TxtNewQuantity.Clear();

                GetStoreProducts();
                GetOtherProducts();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                LblMessageNewProduct.Text = "Error while creating product: " + ex.Message;
            }
        }
    }
}
