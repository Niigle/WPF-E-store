using E_store.Connection;
using E_store.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace E_store.Repository
{
    internal class ProductRepository
    {
        private readonly DatabaseManager db = new DatabaseManager();

        public uint AddProduct(Product product)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO products (name, type, description, barcode)
                                  VALUES (@name, @type, @description, @barcode)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@type", product.Type);
                cmd.Parameters.AddWithValue("@description", (object)product.Description ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@barcode", product.Barcode);

                cmd.ExecuteNonQuery();
                return (uint)cmd.LastInsertedId;
            }
        }

        public Product GetByBarcode(string barcode)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM products WHERE barcode = @barcode";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Product
                        {
                            Id = reader.GetUInt32("id"),
                            Name = reader.GetString("name"),
                            Type = reader.GetString("type"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                            Barcode = reader.GetString("barcode")
                        };
                    }
                }
            }
            return null;
        }

        public List<Product> GetOtherProducts(uint storeId)
        {
            List<Product> products = new List<Product>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT p.* FROM products p
                                  WHERE p.id NOT IN (
                                      SELECT product_id FROM store_products WHERE store_id = @storeId
                                  )";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@storeId", storeId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetUInt32("id"),
                            Name = reader.GetString("name"),
                            Type = reader.GetString("type"),
                            Barcode = reader.GetString("barcode")
                        });
                    }
                }
            }

            return products;
        }

        public List<string> GetAllTypes()
        {
            var types = new List<string>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT DISTINCT type FROM products ORDER BY type";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        types.Add(reader.GetString("type"));
                }
            }

            return types;
        }
    }
}
