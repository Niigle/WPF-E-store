using E_store.Connection;
using E_store.Models;
using MySql.Data.MySqlClient;

namespace E_store.Repository
{
    internal class OrderRepository
    {
        private readonly DatabaseManager db = new DatabaseManager();

        //TODO
        public (bool succesful, string message) CompleteOrder(uint userId, List<CartItem> items)
        {
            if (items == null || items.Count == 0)
                return (false, "Cart is empty.");

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    foreach (var item in items)
                    {
                        string queryState = "SELECT stock FROM store_products WHERE id = @id FOR UPDATE";
                        MySqlCommand cmdState = new MySqlCommand(queryState, conn, transaction);
                        cmdState.Parameters.AddWithValue("@id", item.StoreProductId);

                        object result = cmdState.ExecuteScalar();

                        if (result == null)
                        {
                            transaction.Rollback();
                            return (false, $"Product '{item.Name}' no more exists.");
                        }

                        int currentState = Convert.ToInt32(result);

                        if (currentState < item.Quantity)
                        {
                            transaction.Rollback();
                            return (false, $"Not enough amount on stock '{item.Name}' (available: {currentState}).");
                        }
                    }

                    decimal totalAmount = 0;
                    foreach (var item in items) 
                        totalAmount += item.Price * item.Quantity;

                    string queryOrder = "INSERT INTO orders (user_id, total) VALUES (@userId, @total)";
                    MySqlCommand cmdOrder = new MySqlCommand(queryOrder, conn, transaction);
                    cmdOrder.Parameters.AddWithValue("@userId", userId);
                    cmdOrder.Parameters.AddWithValue("@total", totalAmount);
                    cmdOrder.ExecuteNonQuery();

                    uint orderId = (uint)cmdOrder.LastInsertedId;

                    foreach (var item in items)
                    {
                        string queryItem = @"INSERT INTO order_items (order_id, store_product_id, quantity, price_at_purchase)
                                              VALUES (@orderId, @storeProductId, @quantity, @price)";
                        
                        MySqlCommand cmdItem = new MySqlCommand(queryItem, conn, transaction);
                        cmdItem.Parameters.AddWithValue("@orderId", orderId);
                        cmdItem.Parameters.AddWithValue("@storeProductId", item.StoreProductId);
                        cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                        cmdItem.Parameters.AddWithValue("@price", item.Price);
                        cmdItem.ExecuteNonQuery();

                        string queryUpdateStock = "UPDATE store_products SET stock = stock - @qty WHERE id = @id";
                        MySqlCommand cmdUpdateStock = new MySqlCommand(queryUpdateStock, conn, transaction);
                        cmdUpdateStock.Parameters.AddWithValue("@qty", item.Quantity);
                        cmdUpdateStock.Parameters.AddWithValue("@id", item.StoreProductId);
                        cmdUpdateStock.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return (true, "Order created successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (false, "Error at creating order: " + ex.Message);
                }
            }
        }

        public Order GetOrCreateActiveCart(uint userId)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string querySelect = "SELECT * FROM orders WHERE user_id = @userId AND status = 'IN_PROGRESS' LIMIT 1";
                MySqlCommand cmdSelect = new MySqlCommand(querySelect, conn);
                cmdSelect.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = cmdSelect.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Order
                        {
                            Id = (uint)reader.GetInt32("id"),
                            UserId = (uint)reader.GetInt32("user_id"),
                            Total = reader.GetDecimal("total"),
                            Status = reader.GetString("status"),
                            CreatedOn = reader.GetDateTime("created_on")
                        };
                    }
                }

                string queryInsert = "INSERT INTO orders (user_id, total, status) VALUES (@userId, 0, 'IN_PROGRESS')";
                MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn);
                cmdInsert.Parameters.AddWithValue("@userId", userId);
                cmdInsert.ExecuteNonQuery();

                return new Order
                {
                    Id = (uint)cmdInsert.LastInsertedId,
                    UserId = userId,
                    Total = 0,
                    Status = "IN_PROGRESS",
                    CreatedOn = DateTime.Now
                };
            }
        }

        public void AddToCart(uint orderId, StoreProductDisplay product, int quantity)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string querySelect = "SELECT id, quantity FROM order_items WHERE order_id = @orderId AND store_product_id = @storeProductId";
                MySqlCommand cmdSelect = new MySqlCommand(querySelect, conn);
                cmdSelect.Parameters.AddWithValue("@orderId", orderId);
                cmdSelect.Parameters.AddWithValue("@storeProductId", product.Id);

                uint? existingItemId = null;
                int existingQuantity = 0;

                using (MySqlDataReader reader = cmdSelect.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        existingItemId = (uint)reader.GetInt32("id");
                        existingQuantity = reader.GetInt32("quantity");
                    }
                }

                if (existingItemId.HasValue)
                {
                    string queryUpdate = "UPDATE order_items SET quantity = @quantity WHERE id = @id";
                    MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@quantity", existingQuantity + quantity);
                    cmdUpdate.Parameters.AddWithValue("@id", existingItemId.Value);
                    cmdUpdate.ExecuteNonQuery();
                }
                else
                {
                    string queryInsert = @"INSERT INTO order_items (order_id, store_product_id, quantity, price_at_purchase)
                                            VALUES (@orderId, @storeProductId, @quantity, @price)";
                    MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn);
                    cmdInsert.Parameters.AddWithValue("@orderId", orderId);
                    cmdInsert.Parameters.AddWithValue("@storeProductId", product.Id);
                    cmdInsert.Parameters.AddWithValue("@quantity", quantity);
                    cmdInsert.Parameters.AddWithValue("@price", product.Price);
                    cmdInsert.ExecuteNonQuery();
                }
            }

            UpdateTotalAmount(orderId);
        }

        public void RemoveFromCart(uint orderItemId, uint orderId)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM order_items WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", orderItemId);
                cmd.ExecuteNonQuery();
            }

            UpdateTotalAmount(orderId);
        }

        public void UpdateQuantity(uint orderItemId, uint orderId, int newQuantity)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "UPDATE order_items SET quantity = @quantity WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@quantity", newQuantity);
                cmd.Parameters.AddWithValue("@id", orderItemId);
                cmd.ExecuteNonQuery();
            }

            UpdateTotalAmount(orderId);
        }

        private void UpdateTotalAmount(uint orderId)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string querySum = @"SELECT COALESCE(SUM(quantity * price_at_purchase), 0) 
                                      FROM order_items WHERE order_id = @orderId";
                MySqlCommand cmdSum = new MySqlCommand(querySum, conn);
                cmdSum.Parameters.AddWithValue("@orderId", orderId);
                decimal total = Convert.ToDecimal(cmdSum.ExecuteScalar());

                string queryUpdate = "UPDATE orders SET total = @total WHERE id = @id";
                MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, conn);
                cmdUpdate.Parameters.AddWithValue("@total", total);
                cmdUpdate.Parameters.AddWithValue("@id", orderId);
                cmdUpdate.ExecuteNonQuery();
            }
        }

        public List<OrderItem> GetOrderItems(uint orderId)
        {
            var lista = new List<OrderItem>();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT oi.id, oi.store_product_id, oi.quantity, oi.price_at_purchase,
                                         sp.name, sp.stock, s.name AS store_name
                                  FROM order_items oi
                                  INNER JOIN store_products sp ON sp.id = oi.store_product_id
                                  INNER JOIN stores s ON s.id = sp.store_id
                                  WHERE oi.order_id = @orderId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new OrderItem
                        {
                            Id = (uint)reader.GetInt32("id"),
                            StoreProductId = (uint)reader.GetInt32("store_product_id"),
                            Name = reader.GetString("name"),
                            StoreName = reader.GetString("store_name"),
                            Price = reader.GetDecimal("price_at_purchase"),
                            Quantity = reader.GetInt32("quantity"),
                            AvailableStock = reader.GetInt32("stock")
                        });
                    }
                }
            }

            return lista;
        }

        public int ItemNumberInCart(uint orderId)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT COALESCE(SUM(quantity), 0) FROM order_items WHERE order_id = @orderId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public (bool successful, string message) FinishOrder(uint orderId)
        {
            var orderItems = GetOrderItems(orderId);

            if (orderItems.Count == 0)
                return (false, "ORder is empty.");

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                foreach (var orderItem in orderItems)
                {
                    string queryStanje = "SELECT stock FROM store_products WHERE id = @id";
                    MySqlCommand cmdStanje = new MySqlCommand(queryStanje, conn);
                    cmdStanje.Parameters.AddWithValue("@id", orderItem.StoreProductId);

                    object result = cmdStanje.ExecuteScalar();
                    if (result == null)
                        return (false, $"Product '{orderItem.Name}' no more exists.");

                    int currentState = Convert.ToInt32(result);
                    if (currentState < orderItem.Quantity)
                        return (false, $"Not enough on stock '{orderItem.Name}' (available: {currentState}).");
                }

                foreach (var orderItem in orderItems)
                {
                    string queryUpdateStock = "UPDATE store_products SET stock = stock - @qty WHERE id = @id";
                    MySqlCommand cmdUpdateStock = new MySqlCommand(queryUpdateStock, conn);
                    cmdUpdateStock.Parameters.AddWithValue("@qty", orderItem.Quantity);
                    cmdUpdateStock.Parameters.AddWithValue("@id", orderItem.StoreProductId);
                    cmdUpdateStock.ExecuteNonQuery();
                }

                string queryStatus = "UPDATE orders SET status = 'COMPLETED' WHERE id = @id";
                MySqlCommand cmdStatus = new MySqlCommand(queryStatus, conn);
                cmdStatus.Parameters.AddWithValue("@id", orderId);
                cmdStatus.ExecuteNonQuery();
            }

            return (true, "ORder completed successfully.");
        }
    }
}
