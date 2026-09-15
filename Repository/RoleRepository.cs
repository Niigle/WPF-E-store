using E_store.Connection;
using E_store.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Repository
{
    class RoleRepository
    {
        private readonly DatabaseManager db = new DatabaseManager();

        public Role GetRoleByName(string roleName)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, role_name FROM roles WHERE role_name = @roleName";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roleName", roleName);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Role
                        {
                            Id = reader.GetUInt32("id"),
                            RoleName = reader.GetString("role_name")
                        };
                    }
                }
            }

            return null;
        }

        public List<Role> GetAllRoles()
        {
            List<Role> roles = new List<Role>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, role_name FROM roles";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Role
                        {
                            Id = reader.GetUInt32("id"),
                            RoleName = reader.GetString("role_name")
                        });
                    }
                }
            }

            return roles;
        }

        public List<Role> GetRolesForUser(uint userId)
        {
            List<Role> roles = new List<Role>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT r.id, r.role_name 
                                  FROM roles r
                                  INNER JOIN user_roles ur ON ur.role_id = r.id
                                  WHERE ur.user_id = @userId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Role
                        {
                            Id = reader.GetUInt32("id"),
                            RoleName = reader.GetString("role_name")
                        });
                    }
                }
            }

            return roles;
        }

        public void RemoveUserRole(uint userId, uint roleId)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM user_roles WHERE user_id = @userId AND role_id = @roleId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@roleId", roleId);
                cmd.ExecuteNonQuery();
            }
        }

        public bool HasRole(uint userId, uint roleId)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM user_roles WHERE user_id = @userId AND role_id = @roleId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@roleId", roleId);

                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public void AddRoleToUser(uint userId, uint roleId)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO user_roles (user_id, role_id) VALUES (@userId, @roleId)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@roleId", roleId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<User> GetUsersByRole(string roleName)
        {
            List<User> users = new List<User>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT u.* FROM users u
                          INNER JOIN user_roles ur ON ur.user_id = u.id
                          INNER JOIN roles r ON r.id = ur.role_id
                          WHERE r.role_name = @roleName";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@roleName", roleName);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
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
                        });
                    }
                }
            }

            return users;
        }

        public List<Permission> GetPermissionsForUser(uint userId)
        {
            List<Permission> permissions = new List<Permission>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT DISTINCT p.id, p.description
                                  FROM permissions p
                                  INNER JOIN role_permission rp ON rp.permission_id = p.id
                                  INNER JOIN user_roles ur ON ur.role_id = rp.role_id
                                  WHERE ur.user_id = @userId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(new Permission
                        {
                            Id = reader.GetUInt32("id"),
                            Description = reader.GetString("description")
                        });
                    }
                }
            }

            return permissions;
        }
    }
}
