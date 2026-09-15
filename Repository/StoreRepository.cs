using System;
using System.Collections.Generic;
using System.Text;

using E_store.Models;
using E_store.Connection;
using MySql.Data.MySqlClient;

namespace E_store.Repository
{
    internal class StoreRepository
    {

        private readonly DatabaseManager db = new DatabaseManager();

        public void AddStore(Store store)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO stores (name, address, category_id, phone, manager_id, is_active)
                          VALUES (@name, @address, @categoryId, @phone, @managerId, @isActive)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", store.Name);
                cmd.Parameters.AddWithValue("@address", store.Address);
                cmd.Parameters.AddWithValue("@categoryId", store.CategoryId);
                cmd.Parameters.AddWithValue("@phone", store.Phone);
                cmd.Parameters.AddWithValue("@managerId", store.ManagerId);
                cmd.Parameters.AddWithValue("@isActive", store.IsActive);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Store> GetAllStores()
        {
            List<Store> stores = new List<Store>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT s.*, CONCAT(u.firstname, ' ', u.lastname) AS manager_name, c.name AS category_name
                                  FROM stores s
                                  INNER JOIN users u ON u.id = s.manager_id
                                  INNER JOIN category c ON c.id = s.category_id";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stores.Add(new Store
                        {
                            Id = reader.GetUInt32("id"),
                            Name = reader.GetString("name"),
                            Address = reader.GetString("address"),
                            CategoryId = reader.GetUInt32("category_id"),
                            CategoryName = reader.GetString("category_name"),
                            Phone = reader.GetString("phone"),
                            ManagerId = reader.GetUInt32("manager_id"),
                            IsActive = reader.GetInt32("is_active"),
                            CreatedOn = reader.GetDateTime("created_on"),
                            ModifiedOn = reader.GetDateTime("modified_on"),
                            ManagerFullName = reader.GetString("manager_name")
                        });
                    }
                }
            }

            
            return stores;
        }

        public List<Store> GetStoresByManager(uint managerId)
        {
            List<Store> stores = new List<Store>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT s.*, c.name AS category_name
                          FROM stores s
                          INNER JOIN category c ON c.id = s.category_id
                          WHERE s.manager_id = @managerId AND s.is_active = 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@managerId", managerId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stores.Add(new Store
                        {
                            Id = reader.GetUInt32("id"),
                            Name = reader.GetString("name"),
                            Address = reader.GetString("address"),
                            CategoryId = reader.GetUInt32("category_id"),
                            Phone = reader.GetString("phone"),
                            ManagerId = reader.GetUInt32("manager_id"),
                            IsActive = reader.GetInt32("is_active")
                        });
                    }
                }
            }

            return stores;
        }

        public List<Store> GetActiveStores()
        {
            var stores = new List<Store>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, name FROM stores WHERE is_active = 1 ORDER BY name";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stores.Add(new Store
                        {
                            Id = reader.GetUInt32("id"),
                            Name = reader.GetString("name")
                        });
                    }
                }
            }

            return stores;
        }
    }
}
