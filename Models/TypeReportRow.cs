using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class TypeReportRow
    {
        public string ProductType { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
