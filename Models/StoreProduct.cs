using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class StoreProduct
    {
        public uint Id { get; set; }
        public uint StoreId { get; set; }
        public uint ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Barcode { get; set; }
    }
}
