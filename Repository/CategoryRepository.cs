using E_store.Connection;
using E_store.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Repository
{
    internal class CategoryRepository
    {

        private readonly DatabaseManager db = new DatabaseManager();

        public List<Category> UzmiSve()
        {
            var lista = new List<Category>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, name, description FROM category ORDER BY name";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Category
                        {
                            Id = reader.GetUInt32("id"),
                            Name = reader.GetString("name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description")
                        });
                    }
                }
            }

            return lista;
        }

        public void DodajKategoriju(Category kategorija)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO category (name, description) VALUES (@name, @description)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", kategorija.Name);
                cmd.Parameters.AddWithValue("@description", (object)kategorija.Description ?? System.DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
