using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport.Excel
{
    public static class GarmentPurchasingBookReportLocalExcel
    {
        public static async Task<MemoryStream> GenerateExcel(DateTimeOffset startDate, DateTimeOffset endDate, ReportDto data, int timeZone)
        {
            CultureInfo ci = new CultureInfo("en-us");

            var result = data;
            var reportDataTable = new DataTable();
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(string) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No BP Besar", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No BP Kecil", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Faktur Pajak", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No NI", DataType = typeof(string) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori Pembelian", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori Pembukuan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "DPP", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPh", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Koreksi", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Total(IDR)", DataType = typeof(string) });

            var categoryDataTable = new DataTable();
            categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
            categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(string) });
            //categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (Valas)", DataType = typeof(string) });


            var currencyDataTable = new DataTable();
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(string) });
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(string) });

            if (result.Data.Count > 0)
            {
                foreach (var report in result.Data)
                {
                    reportDataTable.Rows.Add(report.CustomsArrivalDate.AddHours(timeZone).ToString("dd/MM/yyyy"),
                        //report.SupplierName,
                        report.SupplierCode + " - " + report.SupplierName,

                        report.ProductName, 
                        report.GarmentDeliveryOrderNo, 
                        //report.BillNo, 
                        report.PaymentBill, 
                        report.InvoiceNo, 
                        report.VATNo, 
                        report.InternalNoteNo, 
                        //report.PurchasingCategoryName, 
                        report.AccountingCategoryName, 
                        report.InternalNoteQuantity, 
                        report.CurrencyCode, 
                        report.DPPAmount.ToString("N2", ci), 
                        report.VATAmount.ToString("N2", ci), 
                        report.IncomeTaxAmount.ToString("N2", ci),
                        report.PriceCorrection.ToString("N2",ci),
                        report.Total.ToString("N2", ci));
                }
                foreach (var categorySummary in result.Categories)
                    categoryDataTable.Rows.Add(categorySummary.CategoryName, categorySummary.Amount.ToString("N2", ci));

                foreach (var currencySummary in result.Currencies)
                    currencyDataTable.Rows.Add(currencySummary.CurrencyCode, currencySummary.Amount.ToString("N2", ci), currencySummary.Amount.ToString("N2", ci));//TODO : change to Currency TOtal Idr
            }

            using (var package = new ExcelPackage())
            {
                var company = "PT EFRATA GARMINDO UTAMA";
                var title = "LAPORAN BUKU PEMBELIAN LOKAL";

                var startDateStr = startDate == DateTimeOffset.MinValue ? "-" : startDate.AddHours(timeZone).ToString("dd/MM/yyyy");
                var endDateStr = endDate == DateTimeOffset.MaxValue ? "-" : endDate.AddHours(timeZone).ToString("dd/MM/yyyy");
                var period = $"Dari {startDateStr} Sampai {endDateStr}";

                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells["A1"].Value = company;
                worksheet.Cells["A2"].Value = title;
                worksheet.Cells["A3"].Value = period;
                #region PrintHeaderExcel
                var rowStartHeader = 4;
                var colStartHeader = 1;
                foreach (var columns in reportDataTable.Columns)
                {
                    DataColumn column = (DataColumn)columns;
                    if (column.ColumnName == "DPP")
                    {
                        var rowStartHeaderSpan = rowStartHeader + 1;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Value = column.ColumnName;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[rowStartHeader, colStartHeader].Value = "Pembelian";
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 2].Merge = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 3].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    else if (column.ColumnName == "PPN" || column.ColumnName == "PPh")
                    {
                        var rowStartHeaderSpan = rowStartHeader + 1;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Value = column.ColumnName;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    else
                    {
                        worksheet.Cells[rowStartHeader, colStartHeader].Value = column.ColumnName;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Merge = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    colStartHeader++;
                }
                #endregion
                worksheet.Cells["A6"].LoadFromDataTable(reportDataTable, false);
                for (int i = 6; i < result.Data.Count + 6; i++)
                {
                    for (int j = 1; j <= reportDataTable.Columns.Count; j++)
                    {
                        worksheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                }
                worksheet.Cells[$"A{6 + 3 + result.Data.Count}"].LoadFromDataTable(categoryDataTable, true,OfficeOpenXml.Table.TableStyles.Light18);
                worksheet.Cells[$"A{6 + result.Data.Count + 3 + result.Categories.Count + 3}"].LoadFromDataTable(currencyDataTable, true,OfficeOpenXml.Table.TableStyles.Light18);

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }
    }
}
