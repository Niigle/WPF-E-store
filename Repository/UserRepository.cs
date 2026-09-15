using System;
using System.Collections.Generic;
using System.Text;

using MySql.Data.MySqlClient;
using BCrypt.Net;
using E_store.Models;
using E_store.Connection;

namespace E_store.Repository
{
    class UserRepository
    {
        private readonly DatabaseManager db = new DatabaseManager();

        public User GetByUsername(string username)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM users WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapUser(reader);
                }
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM users";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        users.Add(MapUser(reader));
                }
            }

            return users;
        }

        public void AddUser(User user)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO users (firstname, lastname, username, email, address, phone, password) 
                                  VALUES (@firstname, @lastname, @username, @email, @address, @phone, @password)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@firstname", user.FirstName);
                cmd.Parameters.AddWithValue("@lastname", user.LastName);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@address", user.Address);
                cmd.Parameters.AddWithValue("@phone", user.Phone);
                cmd.Parameters.AddWithValue("@password", hash);

                cmd.ExecuteNonQuery();
            }
        }

        public void EditUser(User user)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE users SET 
                                  firstname = @firstname, 
                                  lastname = @lastname, 
                                  username = @username,
                                  email = @email, 
                                  address = @address, 
                                  phone = @phone,
                                  modified_on = CURRENT_TIMESTAMP
                                  WHERE id = @id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@firstname", user.FirstName);
                cmd.Parameters.AddWithValue("@lastname", user.LastName);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@address", user.Address);
                cmd.Parameters.AddWithValue("@phone", user.Phone);
                cmd.Parameters.AddWithValue("@id", user.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteUser(uint id)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM users WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePassword(uint userId, string newPassword)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "UPDATE users SET password = @password WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@password", hash);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        private User MapUser(MySqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetUInt32("id"),
                FirstName = reader.GetString("firstname"),
                LastName = reader.GetString("lastname"),
                Username = reader.GetString("username"),
                Email = reader.GetString("email"),
                Address = reader.GetString("address"),
                Phone = reader.GetString("phone"),
                Password = reader.GetString("password"),
                CreatedOn = reader.GetDateTime("created_on"),
                ModifiedOn = reader.GetDateTime("modified_on")
            };
        }
    }
}
