using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class Order
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
