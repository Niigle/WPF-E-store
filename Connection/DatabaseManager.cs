using System;
using System.Collections.Generic;
using System.Text;

using MySql.Data.MySqlClient;

namespace E_store.Connection
{
    class DatabaseManager
    {

        private string connectionString = "Server=localhost;Port=3306;Database=prodavnica;Uid=root;Pwd=;SslMode=disabled;AllowPublicKeyRetrieval=True;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

    }
}
