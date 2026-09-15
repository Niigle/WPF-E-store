using System;
using System.Collections.Generic;
using System.Text;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using E_store.Models;

namespace E_store.Services
{
    internal class ReportPdfGenerator
    {
        public static void Generate(ReportSummary report, string title, DateTime start, DateTime end, string path)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text(title).FontSize(18).Bold();
                        col.Item().Text($"Period: {start:dd.MM.yyyy} - {end:dd.MM.yyyy}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Item().Text($"Total amount of orders: {report.OrderAmount}").Bold();
                        col.Item().Text($"Total value: {report.TotalValue:N2} RSD").Bold();

                        col.Item().PaddingTop(20).Text("Per store").FontSize(14).Bold();
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Text("Store").Bold();
                                h.Cell().Text("Order amount").Bold();
                                h.Cell().Text("Value").Bold();
                            });

                            foreach (var row in report.Store)
                            {
                                table.Cell().Text(row.StoreName);
                                table.Cell().Text(row.OrderCount.ToString());
                                table.Cell().Text($"{row.TotalAmount:N2} RSD");
                            }
                        });

                        col.Item().PaddingTop(20).Text("Per product type").FontSize(14).Bold();
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Text("Product type").Bold();
                                h.Cell().Text("Order number").Bold();
                                h.Cell().Text("Value").Bold();
                            });

                            foreach (var row in report.Types)
                            {
                                table.Cell().Text(row.ProductType);
                                table.Cell().Text(row.OrderCount.ToString());
                                table.Cell().Text($"{row.TotalAmount:N2} RSD");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            }).GeneratePdf(path);
        }
    }
}
