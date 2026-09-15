using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class StoreReportRow
    {
        public string StoreName { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
