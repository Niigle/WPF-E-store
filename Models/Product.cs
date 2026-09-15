using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class Product
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
