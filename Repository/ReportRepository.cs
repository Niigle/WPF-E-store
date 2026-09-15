using E_store.Connection;
using E_store.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Repository
{
    internal class ReportRepository
    {
        private readonly DatabaseManager db = new DatabaseManager();

        public ReportSummary CreateReport(DateTime start, DateTime end)
        {
            var report = new ReportSummary();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string queryTotal = @"
                    SELECT COUNT(DISTINCT o.id) AS order_count, 
                           COALESCE(SUM(oi.quantity * oi.price_at_purchase), 0) AS total_value
                    FROM orders o
                    JOIN order_items oi ON oi.order_id = o.id
                    WHERE o.created_at BETWEEN @start AND @end";

                using (MySqlCommand cmd = new MySqlCommand(queryTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            report.OrderAmount = reader.GetInt32("order_count");
                            report.TotalValue = reader.GetDecimal("total_value");
                        }
                    }
                }

                string queryStores = @"
                    SELECT s.name AS store_name,
                           COUNT(DISTINCT o.id) AS order_count,
                           SUM(oi.quantity * oi.price_at_purchase) AS total_value
                    FROM orders o
                    JOIN order_items oi ON oi.order_id = o.id
                    JOIN store_products sp ON sp.id = oi.store_product_id
                    JOIN stores s ON s.id = sp.store_id
                    WHERE o.created_at BETWEEN @start AND @end
                    GROUP BY s.id, s.name
                    ORDER BY total_value DESC";

                using (MySqlCommand cmd = new MySqlCommand(queryStores, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            report.Store.Add(new StoreReportRow
                            {
                                StoreName = reader.GetString("store_name"),
                                OrderCount = reader.GetInt32("order_count"),
                                TotalAmount = reader.GetDecimal("total_value")
                            });
                        }
                    }
                }

                string queryTypes = @"
                    SELECT p.type AS product_type,
                           COUNT(DISTINCT o.id) AS order_count,
                           SUM(oi.quantity * oi.price_at_purchase) AS total_value
                    FROM orders o
                    JOIN order_items oi ON oi.order_id = o.id
                    JOIN store_products sp ON sp.id = oi.store_product_id
                    JOIN products p ON p.id = sp.product_id
                    WHERE o.created_at BETWEEN @start AND @end
                    GROUP BY p.type
                    ORDER BY total_value DESC";

                using (MySqlCommand cmd = new MySqlCommand(queryTypes, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            report.Types.Add(new TypeReportRow
                            {
                                ProductType = reader.GetString("product_type"),
                                OrderCount = reader.GetInt32("order_count"),
                                TotalAmount = reader.GetDecimal("total_value")
                            });
                        }
                    }
                }
            }

            return report;
        }

        public ReportSummary CreateReportForStores(DateTime start, DateTime end, List<uint> storeIds)
        {
            var report = new ReportSummary();

            if (storeIds == null || storeIds.Count == 0)
                return report;

            string storeIdParams = string.Join(",", storeIds.Select((id, i) => $"@storeId{i}"));

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string queryTotal = $@"
            SELECT COUNT(DISTINCT o.id) AS order_count,
                   COALESCE(SUM(oi.quantity * oi.price_at_purchase), 0) AS total_value
            FROM orders o
            JOIN order_items oi ON oi.order_id = o.id
            JOIN store_products sp ON sp.id = oi.store_product_id
            WHERE o.created_at BETWEEN @start AND @end
              AND sp.store_id IN ({storeIdParams})";

                using (MySqlCommand cmd = new MySqlCommand(queryTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);
                    for (int i = 0; i < storeIds.Count; i++)
                        cmd.Parameters.AddWithValue($"@storeId{i}", storeIds[i]);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            report.OrderAmount = reader.GetInt32("order_count");
                            report.TotalValue = reader.GetDecimal("total_value");
                        }
                    }
                }

                string queryStores = $@"
            SELECT s.name AS store_name,
                   COUNT(DISTINCT o.id) AS order_count,
                   SUM(oi.quantity * oi.price_at_purchase) AS total_value
            FROM orders o
            JOIN order_items oi ON oi.order_id = o.id
            JOIN store_products sp ON sp.id = oi.store_product_id
            JOIN stores s ON s.id = sp.store_id
            WHERE o.created_at BETWEEN @start AND @end
              AND sp.store_id IN ({storeIdParams})
            GROUP BY s.id, s.name
            ORDER BY total_value DESC";

                using (MySqlCommand cmd = new MySqlCommand(queryStores, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);
                    for (int i = 0; i < storeIds.Count; i++)
                        cmd.Parameters.AddWithValue($"@storeId{i}", storeIds[i]);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            report.Store.Add(new StoreReportRow
                            {
                                StoreName = reader.GetString("store_name"),
                                OrderCount = reader.GetInt32("order_count"),
                                TotalAmount = reader.GetDecimal("total_value")
                            });
                        }
                    }
                }

                string queryTipovi = $@"
            SELECT p.type AS product_type,
                   COUNT(DISTINCT o.id) AS order_count,
                   SUM(oi.quantity * oi.price_at_purchase) AS total_value
            FROM orders o
            JOIN order_items oi ON oi.order_id = o.id
            JOIN store_products sp ON sp.id = oi.store_product_id
            JOIN products p ON p.id = sp.product_id
            WHERE o.created_at BETWEEN @start AND @end
              AND sp.store_id IN ({storeIdParams})
            GROUP BY p.type
            ORDER BY total_value DESC";

                using (MySqlCommand cmd = new MySqlCommand(queryTipovi, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);
                    for (int i = 0; i < storeIds.Count; i++)
                        cmd.Parameters.AddWithValue($"@storeId{i}", storeIds[i]);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            report.Types.Add(new TypeReportRow
                            {
                                ProductType = reader.GetString("product_type"),
                                OrderCount = reader.GetInt32("order_count"),
                                TotalAmount = reader.GetDecimal("total_value")
                            });
                        }
                    }
                }
            }

            return report;
        }
    }
}
