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
                string query = @"INSERT INTO stores (name, address, type, phone, manager, is_active)
                                    VALUES (@name, @address, @type, @phone, @manager, @isActive)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", store.Name);
                cmd.Parameters.AddWithValue("@address", store.Address);
                cmd.Parameters.AddWithValue("@type", store.Type);
                cmd.Parameters.AddWithValue("@phone", store.Phone);
                cmd.Parameters.AddWithValue("@manager", store.Manager);
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
                string query = @"SELECT s.*, CONCAT(u.firstname, ' ', u.lastname) AS manager_name
                                  FROM stores s
                                  INNER JOIN users u ON u.id = s.manager";

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
                            Type = reader.GetString("type"),
                            Phone = reader.GetString("phone"),
                            Manager = reader.GetUInt32("manager"),
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
                string query = "SELECT * FROM stores WHERE manager = @managerId AND is_active = 1";
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
                            Type = reader.GetString("type"),
                            Phone = reader.GetString("phone"),
                            Manager = reader.GetUInt32("manager"),
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
