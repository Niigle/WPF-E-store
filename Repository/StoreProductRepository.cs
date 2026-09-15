using E_store.Connection;
using E_store.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Repository
{
    internal class StoreProductRepository
    {
        private readonly DatabaseManager db = new DatabaseManager();

        public void AddToStore(uint storeId, uint productId, string name, decimal price, int stock)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO store_products (store_id, product_id, name, price, stock)
                                  VALUES (@storeId, @productId, @name, @price, @stock)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@storeId", storeId);
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@stock", stock);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStockPrice(uint storeProductId, decimal price, int stock)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "UPDATE store_products SET price = @price, stock = @stock WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@stock", stock);
                cmd.Parameters.AddWithValue("@id", storeProductId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<StoreProduct> GetStoreProducts(uint storeId)
        {
            List<StoreProduct> storeProducts = new List<StoreProduct>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT sp.*, p.barcode 
                                  FROM store_products sp
                                  INNER JOIN products p ON p.id = sp.product_id
                                  WHERE sp.store_id = @storeId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@storeId", storeId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storeProducts.Add(new StoreProduct
                        {
                            Id = reader.GetUInt32("id"),
                            StoreId = reader.GetUInt32("store_id"),
                            ProductId = reader.GetUInt32("product_id"),
                            Name = reader.GetString("name"),
                            Price = reader.GetDecimal("price"),
                            Stock = reader.GetInt32("stock"),
                            Barcode = reader.IsDBNull(reader.GetOrdinal("barcode")) ? null : reader.GetString("barcode")
                        });
                    }
                }
            }

            return storeProducts;
        }

        // TODO searchText @description @name?
        public List<StoreProductDisplay> SearchProducts(string searchText, uint? storeId, string type, decimal? minPrice, decimal? maxPrice)
        {
            var storeProducts = new List<StoreProductDisplay>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT sp.id, sp.product_id, sp.store_id, sp.name, sp.price, sp.stock,
                   s.name AS store_name, p.type AS product_type
            FROM store_products sp
            INNER JOIN stores s ON s.id = sp.store_id
            INNER JOIN products p ON p.id = sp.product_id
            WHERE s.is_active = 1";

                if (!string.IsNullOrWhiteSpace(searchText))
                    query += " AND sp.name LIKE @searchText";
                if (storeId.HasValue)
                    query += " AND sp.store_id = @storeId";
                if (!string.IsNullOrWhiteSpace(type))
                    query += " AND p.type = @type";
                if (minPrice.HasValue)
                    query += " AND sp.price >= @minPrice";
                if (maxPrice.HasValue)
                    query += " AND sp.price <= @maxPrice";

                query += " ORDER BY sp.name";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                if (!string.IsNullOrWhiteSpace(searchText))
                    cmd.Parameters.AddWithValue("@searchText", $"%{searchText}%");
                if (storeId.HasValue)
                    cmd.Parameters.AddWithValue("@storeId", storeId.Value);
                if (!string.IsNullOrWhiteSpace(type))
                    cmd.Parameters.AddWithValue("@type", type);
                if (minPrice.HasValue)
                    cmd.Parameters.AddWithValue("@minPrice", minPrice.Value);
                if (maxPrice.HasValue)
                    cmd.Parameters.AddWithValue("@maxPrice", maxPrice.Value);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storeProducts.Add(new StoreProductDisplay
                        {
                            Id = reader.GetUInt32("id"),
                            ProductId = reader.GetUInt32("product_id"),
                            StoreId = reader.GetUInt32("store_id"),
                            Name = reader.GetString("name"),
                            StoreName = reader.GetString("store_name"),
                            ProductType = reader.GetString("product_type"),
                            Price = reader.GetDecimal("price"),
                            Stock = reader.GetInt32("stock")
                        });
                    }
                }
            }

            return storeProducts;
        }
    }
}
