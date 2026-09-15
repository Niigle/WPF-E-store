using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class ReportSummary
    {
        public int OrderAmount { get; set; }
        public decimal TotalValue { get; set; }
        public List<StoreReportRow> Store { get; set; } = new List<StoreReportRow>();
        public List<TypeReportRow> Types { get; set; } = new List<TypeReportRow>();
    }
}
