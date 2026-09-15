using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class OrderItem
    {
        public uint Id { get; set; }
        public uint StoreProductId { get; set; }
        public string Name { get; set; }
        public string StoreName { get; set; }
        public decimal Price { get; set; } // price_at_purchase
        public int Quantity { get; set; }
        public int AvailableStock { get; set; }

        public decimal TotalAmount => Price * Quantity;
    }
}
