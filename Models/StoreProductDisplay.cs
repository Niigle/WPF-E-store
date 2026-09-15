using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class StoreProductDisplay
    {
        public uint Id { get; set; } // store_products.id
        public uint ProductId { get; set; }
        public uint StoreId { get; set; }
        public string Name { get; set; }
        public string StoreName { get; set; }
        public string ProductType { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
