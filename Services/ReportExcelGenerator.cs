using System;
using System.Collections.Generic;
using System.Text;

using ClosedXML.Excel;
using E_store.Models;

namespace E_store.Services
{
    internal class ReportExcelGenerator
    {
        public static void Generate(ReportSummary report, string title, DateTime start, DateTime end, string path)
        {
            using (var workbook = new XLWorkbook())
            {
                var wsGeneral = workbook.Worksheets.Add("Overview");
                wsGeneral.Cell(1, 1).Value = title;
                wsGeneral.Cell(1, 1).Style.Font.Bold = true;
                wsGeneral.Cell(1, 1).Style.Font.FontSize = 16;

                wsGeneral.Cell(2, 1).Value = $"Period: {start:dd.MM.yyyy} - {end:dd.MM.yyyy}";
                wsGeneral.Cell(4, 1).Value = "Total amount of orders:";
                wsGeneral.Cell(4, 2).Value = report.OrderAmount;
                wsGeneral.Cell(5, 1).Value = "Total value:";
                wsGeneral.Cell(5, 2).Value = report.TotalValue;
                wsGeneral.Cell(5, 2).Style.NumberFormat.Format = "#,##0.00 RSD";

                var wsStores = workbook.Worksheets.Add("Per store");
                wsStores.Cell(1, 1).Value = "Store";
                wsStores.Cell(1, 2).Value = "Order amount";
                wsStores.Cell(1, 3).Value = "Value";
                wsStores.Range(1, 1, 1, 3).Style.Font.Bold = true;

                int row = 2;
                foreach (var store in report.Store)
                {
                    wsStores.Cell(row, 1).Value = store.StoreName;
                    wsStores.Cell(row, 2).Value = store.OrderCount;
                    wsStores.Cell(row, 3).Value = store.TotalAmount;
                    wsStores.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 RSD";
                    row++;
                }
                wsStores.Columns().AdjustToContents();

                var wsTypes = workbook.Worksheets.Add("Per product type");
                wsTypes.Cell(1, 1).Value = "Product type";
                wsTypes.Cell(1, 2).Value = "Number of orders";
                wsTypes.Cell(1, 3).Value = "Value";
                wsTypes.Range(1, 1, 1, 3).Style.Font.Bold = true;

                row = 2;
                foreach (var type in report.Types)
                {
                    wsTypes.Cell(row, 1).Value = type.ProductType;
                    wsTypes.Cell(row, 2).Value = type.OrderCount;
                    wsTypes.Cell(row, 3).Value = type.TotalAmount;
                    wsTypes.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 RSD";
                    row++;
                }
                wsTypes.Columns().AdjustToContents();

                wsStores.Columns().AdjustToContents();
                wsGeneral.Columns().AdjustToContents();

                workbook.SaveAs(path);
            }
        }
    }
}
