using E_store.Connection;
using E_store.Repository;
using E_store.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace E_store.Views
{
    /// <summary>
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        private readonly ReportRepository reportRepository = new ReportRepository();

        public ReportsPage()
        {
            InitializeComponent();
        }

        private void Period_Checked(object sender, RoutedEventArgs e)
        {
            if (LblPeriodDescription == null) return;

            var (start, end) = CalculatePeriod();
            LblPeriodDescription.Text = $"Report for period: {start:dd.MM.yyyy} - {end:dd.MM.yyyy}";
        }

        private (DateTime start, DateTime end) CalculatePeriod()
        {
            DateTime datum = DatePicker.SelectedDate ?? DateTime.Now;

            if (RBDaily.IsChecked == true)
            {
                return (datum.Date, datum.Date.AddDays(1).AddSeconds(-1));
            }
            else if (RbMonthly.IsChecked == true)
            {
                DateTime start = new DateTime(datum.Year, datum.Month, 1);
                DateTime end = start.AddMonths(1).AddSeconds(-1);
                return (start, end);
            }
            else
            {
                DateTime start = new DateTime(datum.Year, 1, 1);
                DateTime end = start.AddYears(1).AddSeconds(-1);
                return (start, end);
            }
        }

        private string GetTitle()
        {
            if (RBDaily.IsChecked == true) return "Daily report";
            if (RbMonthly.IsChecked == true) return "Monthly report";
            return "Yearly report";
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Red;
            LblMessage.Text = "";

            try
            {
                var (start, end) = CalculatePeriod();
                var report = reportRepository.CreateReport(start, end);

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "PDF file (*.pdf)|*.pdf",
                    FileName = $"report_{start:yyyyMMdd}_{end:yyyyMMdd}.pdf"
                };

                if (dialog.ShowDialog() == true)
                {
                    ReportPdfGenerator.Generate(report, GetTitle(), start, end, dialog.FileName);
                    LblMessage.Foreground = System.Windows.Media.Brushes.Green;
                    LblMessage.Text = "PDF report created.";
                }
            }
            catch (Exception ex)
            {
                LblMessage.Text = "Error while genearating PDF: " + ex.Message;
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            LblMessage.Foreground = System.Windows.Media.Brushes.Red;
            LblMessage.Text = "";

            try
            {
                var (start, end) = CalculatePeriod();
                var report = reportRepository.CreateReport(start, end);

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "Excel file (*.xlsx)|*.xlsx",
                    FileName = $"report_{start:yyyyMMdd}_{end:yyyyMMdd}.xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    ReportExcelGenerator.Generate(report, GetTitle(), start, end, dialog.FileName);
                    LblMessage.Foreground = System.Windows.Media.Brushes.Green;
                    LblMessage.Text = "Excel report craeted.";
                }
            }
            catch (Exception ex)
            {
                LblMessage.Text = "Error while generating Excel file: " + ex.Message;
            }
        }
    }
}
