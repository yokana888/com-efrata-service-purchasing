using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport
{
    public class GarmentPurchasingBookReportService : IGarmentPurchasingBookReportService
    {
        private readonly PurchasingDbContext _dbContext;

        public GarmentPurchasingBookReportService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
        }

        public List<AutoCompleteDto> GetBillNos(string keyword)
        {
            var query = _dbContext.GarmentDeliveryOrders.Select(entity => entity.BillNo).Distinct();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.Contains(keyword));

            return query.Take(10).Select(entity => new AutoCompleteDto(entity)).ToList();
        }

        public List<AutoCompleteDto> GetPaymentBills(string keyword)
        {
            var query = _dbContext.GarmentDeliveryOrders.Select(entity => entity.PaymentBill).Distinct();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.Contains(keyword));

            return query.Take(10).Select(entity => new AutoCompleteDto(entity)).ToList();
        }

        public List<AutoCompleteDto> GetAccountingCategories(string keyword)
        {
            var query = _dbContext.GarmentDeliveryOrderDetails.Select(entity => entity.CodeRequirment).Distinct();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.Contains(keyword));

            return query.Take(10).Select(entity => new AutoCompleteDto(entity)).ToList();
        }

        public ReportDto GetReport(string billNo, string paymentBill, string garmentCategory, DateTimeOffset startDate, DateTimeOffset endDate, bool isForeignCurrency, bool isImportSupplier)
        {
            _dbContext.Database.SetCommandTimeout(18000);
            var query = from garmentDeliveryOrders in _dbContext.GarmentDeliveryOrders.AsQueryable()

                        join customsItems in _dbContext.GarmentBeacukaiItems.AsQueryable() on garmentDeliveryOrders.Id equals customsItems.GarmentDOId into doCustomItems
                        from deliveryOrderCustomItems in doCustomItems.DefaultIfEmpty()

                        join customs in _dbContext.GarmentBeacukais.AsQueryable() on deliveryOrderCustomItems.BeacukaiId equals customs.Id into doCustoms
                        from deliveryOrderCustoms in doCustoms.DefaultIfEmpty()

                        join garmentDeliveryOrderItems in _dbContext.GarmentDeliveryOrderItems.AsQueryable() on garmentDeliveryOrders.Id equals garmentDeliveryOrderItems.GarmentDOId into garmentDOItems
                        from deliveryOrderItems in garmentDOItems.DefaultIfEmpty()

                        join garmentDeliveryOrderDetails in _dbContext.GarmentDeliveryOrderDetails.AsQueryable() on deliveryOrderItems.Id equals garmentDeliveryOrderDetails.GarmentDOItemId into garmentDODetails
                        from deliveryOrderDetails in garmentDODetails.DefaultIfEmpty()

                        join invoiceItems in _dbContext.GarmentInvoiceItems.AsQueryable() on garmentDeliveryOrders.Id equals invoiceItems.DeliveryOrderId into garmentDOInvoiceItems
                        from deliveryOrderInvoiceItems in garmentDOInvoiceItems.DefaultIfEmpty()

                        join invoices in _dbContext.GarmentInvoices.AsQueryable() on deliveryOrderInvoiceItems.InvoiceId equals invoices.Id into garmentDOInvoices
                        from deliveryOrderInvoices in garmentDOInvoices.DefaultIfEmpty()

                        join garmentExternalPurchaseOrders in _dbContext.GarmentExternalPurchaseOrders.AsQueryable() on deliveryOrderItems.EPOId equals garmentExternalPurchaseOrders.Id into doExternalPurchaseOrders
                        from deliveryOrderExternalPurchaseOrders in doExternalPurchaseOrders.DefaultIfEmpty()

                        join internalNoteDetails in _dbContext.GarmentInternNoteDetails.AsQueryable() on garmentDeliveryOrders.Id equals internalNoteDetails.DOId into doInternalNoteDetails
                        from deliveryOrderInternalNoteDetails in doInternalNoteDetails.DefaultIfEmpty()

                        join internalNoteItems in _dbContext.GarmentInternNoteItems.AsQueryable() on deliveryOrderInternalNoteDetails.GarmentItemINId equals internalNoteItems.Id into doInternalNoteItems
                        from deliveryOrderInternalNoteItems in doInternalNoteItems.DefaultIfEmpty()

                        join internalNotes in _dbContext.GarmentInternNotes.AsQueryable() on deliveryOrderInternalNoteItems.GarmentINId equals internalNotes.Id into doInternalNotes
                        from deliveryOrderInternalNotes in doInternalNotes.DefaultIfEmpty()

                        select new
                        {
                            CustomsArrivalDate = deliveryOrderCustomItems != null ? deliveryOrderCustomItems.ArrivalDate : DateTimeOffset.MinValue,
                            CustomsDate = deliveryOrderCustoms != null ? deliveryOrderCustoms.BeacukaiDate : DateTimeOffset.MinValue,
                            CustomsNo = deliveryOrderCustoms != null ? deliveryOrderCustoms.BeacukaiNo : null,
                            CustomsType = deliveryOrderCustoms != null ? deliveryOrderCustoms.CustomsType : null,
                            ImportValueRemark = deliveryOrderCustoms != null ? deliveryOrderCustoms.ImportValue : null,
                            BillNo = garmentDeliveryOrders != null ? garmentDeliveryOrders.BillNo : null,
                            PaymentBill = garmentDeliveryOrders != null ? garmentDeliveryOrders.PaymentBill : null,
                            PurchasingCategoryName = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.Category : null,
                            SupplierId = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.SupplierId : 0,
                            SupplierCode = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.SupplierCode : null,
                            SupplierName = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.SupplierName : null,
                            IsImportSupplier = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.SupplierImport : false,
                            CurrencyCode = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.CurrencyCode : null,
                            CurrencyId = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.CurrencyId : 0,
                            CurrencyRate = deliveryOrderExternalPurchaseOrders != null ? deliveryOrderExternalPurchaseOrders.CurrencyRate : 0,
                            AccountingCategoryName = deliveryOrderDetails != null ? deliveryOrderDetails.CodeRequirment : null,
                            ProductName = deliveryOrderInternalNoteDetails != null ? deliveryOrderInternalNoteDetails.ProductName : null,
                            DeliveryOrderId = garmentDeliveryOrders != null ? garmentDeliveryOrders.Id : 0,
                            DeliveryOrderNo = garmentDeliveryOrders != null ? garmentDeliveryOrders.DONo : null,
                            InvoiceId = deliveryOrderInvoices != null ? deliveryOrderInvoices.Id : 0,
                            InvoiceNo = deliveryOrderInvoices != null ? deliveryOrderInvoices.InvoiceNo : null,
                            VATNo = deliveryOrderInvoices != null ? deliveryOrderInvoices.VatNo : null,
                            InternalNoteId = deliveryOrderInternalNotes != null ? deliveryOrderInternalNotes.Id : 0,
                            InternalNoteNo = deliveryOrderInternalNotes != null ? deliveryOrderInternalNotes.INNo : "",
                            InternalNoteQuantity = deliveryOrderInternalNoteDetails != null ? deliveryOrderInternalNoteDetails.Quantity : 0,
                            DPPAmount = deliveryOrderInternalNoteDetails != null ? deliveryOrderInternalNoteDetails.PriceTotal : 0,
                            IsUseVAT = deliveryOrderInvoices != null ? deliveryOrderInvoices.UseVat : false,
                            IsPayVAT = deliveryOrderInvoices != null ? deliveryOrderInvoices.IsPayVat : false,
                            IsUseIncomeTax = deliveryOrderInvoices != null ? deliveryOrderInvoices.UseIncomeTax : false,
                            IsIncomeTaxPaidBySupplier = deliveryOrderInvoices != null ? deliveryOrderInvoices.IsPayTax : false,
                            IncomeTaxRate = deliveryOrderInvoices != null ? deliveryOrderInvoices.IncomeTaxRate : 0.0,
                            PriceTotalCorrection = deliveryOrderDetails != null ? deliveryOrderDetails.PriceTotalCorrection : 0,
                        };

            //select new ReportIndexDto(deliveryOrderCustoms.ArrivalDate, deliveryOrderExternalPurchaseOrders.SupplierId, deliveryOrderExternalPurchaseOrders.SupplierName, deliveryOrderExternalPurchaseOrders.SupplierImport, deliveryOrderInternalNoteDetails.ProductName, (int) garmentDeliveryOrders.Id, garmentDeliveryOrders.DONo, garmentDeliveryOrders.BillNo, garmentDeliveryOrders.PaymentBill, (int) deliveryOrderInvoices.Id, deliveryOrderInvoices.InvoiceNo, deliveryOrderInvoices.VatNo, (int) deliveryOrderInternalNotes.Id, deliveryOrderInternalNotes.INNo, 0, deliveryOrderExternalPurchaseOrders.Category, 0, deliveryOrderExternalPurchaseOrders.Category, deliveryOrderInternalNoteDetails.Quantity, (int) deliveryOrderInternalNotes.CurrencyId.GetValueOrDefault(), deliveryOrderInternalNotes.CurrencyCode, deliveryOrderInternalNotes.CurrencyRate, deliveryOrderInternalNoteDetails.PriceTotal, deliveryOrderInvoices.UseVat, deliveryOrderInvoices.IsPayVat, deliveryOrderInvoices.UseIncomeTax, deliveryOrderInvoices.IsPayTax, deliveryOrderInvoices.IncomeTaxRate);

            query = query.Where(entity => entity.CustomsArrivalDate >= startDate && entity.CustomsArrivalDate <= endDate);

            if (!string.IsNullOrWhiteSpace(billNo))
            {
                query = query.Where(entity => entity.BillNo == billNo);
            }

            if (!string.IsNullOrWhiteSpace(paymentBill))
            {
                query = query.Where(entity => entity.PaymentBill == paymentBill);
            }

            if (!string.IsNullOrWhiteSpace(garmentCategory))
            {
                query = query.Where(entity => entity.AccountingCategoryName == garmentCategory);
            }

            if (isImportSupplier)
            {
                query = query.Where(entity => entity.IsImportSupplier);
            }
            else
            {
                query = query.Where(entity => !entity.IsImportSupplier);
            }

            if (isForeignCurrency)
            {
                query = query.Where(entity => entity.CurrencyCode != "IDR");
            }

            if (!isForeignCurrency && !isImportSupplier)
            {
                query = query.Where(entity => entity.CurrencyCode == "IDR");
            }

            var data = query.Distinct().ToList().Select(entity => new ReportIndexDto(entity.CustomsArrivalDate,
                entity.SupplierId, 
                entity.SupplierCode, 
                entity.SupplierName, 
                entity.IsImportSupplier, 
                entity.ProductName, 
                (int)entity.DeliveryOrderId, entity.DeliveryOrderNo, entity.BillNo, entity.PaymentBill, (int)entity.InvoiceId, entity.InvoiceNo, entity.VATNo, (int)entity.InternalNoteId, entity.InternalNoteNo, 0, entity.PurchasingCategoryName, 0, entity.AccountingCategoryName, entity.InternalNoteQuantity, entity.CurrencyId, entity.CurrencyCode, entity.CurrencyRate, entity.DPPAmount, entity.IsUseVAT, entity.IsPayVAT, entity.IsUseIncomeTax, entity.IsIncomeTaxPaidBySupplier, entity.IncomeTaxRate, entity.CustomsDate, entity.CustomsNo, entity.CustomsType, entity.ImportValueRemark,(double)entity.PriceTotalCorrection)).ToList();

            var reportCategories = data
                .GroupBy(element => new { element.CurrencyCode, element.AccountingCategoryName })
                .Select(element => new ReportCategoryDto(0, element.Key.AccountingCategoryName, element.Key.CurrencyCode, element.Sum(sum => sum.CurrencyDPPAmount), element.Sum(sum => sum.Total)))
                .ToList();

            var reportCurrencies = data
                .GroupBy(element => element.CurrencyId)
                .Select(element => new ReportCurrencyDto(element.Key, element.FirstOrDefault(e => element.Key == e.CurrencyId).CurrencyCode, element.FirstOrDefault(e => element.Key == e.CurrencyId).CurrencyRate, element.Sum(sum => sum.Total), element.Sum(sum => sum.CurrencyDPPAmount)))
                .ToList();

            var reportCurrencyAndCategory = data
                .GroupBy(element => new { element.CurrencyId, element.AccountingCategoryName })
                .Select(element => new ReportCurrencyAndCategoryDto(element.Key.CurrencyId, element.FirstOrDefault(e => element.Key.CurrencyId == e.CurrencyId).CurrencyCode, element.FirstOrDefault(e => element.Key.CurrencyId == e.CurrencyId).CurrencyRate, element.Sum(sum => sum.Total), element.Sum(sum => sum.CurrencyDPPAmount), element.Key.AccountingCategoryName))
                .ToList();

            return new ReportDto(data, reportCategories, reportCurrencies, reportCurrencyAndCategory);
        }

        public async Task<MemoryStream> GenerateExcel(string billNo, string paymentBill, string garmentCategory, DateTimeOffset startDate, DateTimeOffset endDate, bool isForeignCurrency, bool isImportSupplier, int timeZone)
        {
            var result = GetReport(billNo, paymentBill, garmentCategory, startDate, endDate, isForeignCurrency, isImportSupplier);
            //var Data = reportResult.Reports;
            var reportDataTable = new DataTable();
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No BP Besar", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No BP Kecil", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(string) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No SPB/NI", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Faktur Pajak", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No NI", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori Pembelian", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori Pembukuan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "DPP", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPh", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Koreksi", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Total(IDR)", DataType = typeof(decimal) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "DPP Valas", DataType = typeof(decimal) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Rate", DataType = typeof(decimal) });
            //reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });

            var categoryDataTable = new DataTable();
            categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
            categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });

            var currencyDataTable = new DataTable();
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });

            if (result.Data.Count > 0)
            {
                foreach (var report in result.Data)
                {
                    reportDataTable.Rows.Add(report.CustomsArrivalDate.ToString("dd/MM/yyyy"), report.SupplierName, report.ProductName, report.GarmentDeliveryOrderNo, report.BillNo, report.PaymentBill, report.InvoiceNo, report.VATNo, report.InternalNoteNo, report.PurchasingCategoryName, report.AccountingCategoryName, report.InternalNoteQuantity, report.CurrencyCode, report.DPPAmount, report.VATAmount, report.IncomeTaxAmount,report.PriceCorrection, report.Total);
                }
                foreach (var categorySummary in result.Categories)
                    categoryDataTable.Rows.Add(categorySummary.CategoryName, categorySummary.Amount);

                foreach (var currencySummary in result.Currencies)
                    currencyDataTable.Rows.Add(currencySummary.CurrencyCode, currencySummary.Amount, currencySummary.Amount);//TODO : change to Currency TOtal Idr
            }

            using (var package = new ExcelPackage())
            {
                var company = "PT EFRATA GARMINDO UTAMA";
                var title = "BUKU PEMBELIAN Lokal";
                if (isForeignCurrency)
                    title = "BUKU PEMBELIAN Lokal Valas";
                else if (!isForeignCurrency && isImportSupplier)
                    title = "BUKU PEMBELIAN Impor";

                var startDateStr = startDate == DateTimeOffset.MinValue ? "-" : startDate.AddHours(timeZone).ToString("dd/MM/yyyy");
                var endDateStr = endDate == DateTimeOffset.MaxValue ? "-" : endDate.AddHours(timeZone).ToString("dd/MM/yyyy");
                var period = $"Dari {startDateStr} Sampai {endDateStr}";

                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells["A1"].Value = company;
                worksheet.Cells["A2"].Value = title;
                worksheet.Cells["A3"].Value = period;
                worksheet.Cells["A4"].LoadFromDataTable(reportDataTable, true);
                worksheet.Cells[$"A{4 + 3 + result.Data.Count}"].LoadFromDataTable(categoryDataTable, true);
                worksheet.Cells[$"A{4 + result.Data.Count + 3 + result.Data.Count + 3}"].LoadFromDataTable(currencyDataTable, true);

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }
    }
}
