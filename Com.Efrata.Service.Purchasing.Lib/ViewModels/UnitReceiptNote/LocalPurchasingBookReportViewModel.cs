using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote
{
    public class LocalPurchasingBookReportViewModel
    {
        public LocalPurchasingBookReportViewModel()
        {
            Reports = new List<PurchasingReport>();
            CategorySummaries = new List<Summary>();
            CurrencySummaries = new List<Summary>();
        }
        public List<PurchasingReport> Reports { get; set; }
        public List<Summary> CategorySummaries { get; set; }
        public List<Summary> CurrencySummaries { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CategorySummaryTotal { get; set; }
    }

    public class Summary
    {
        public string Category { get; set; }
        public string CurrencyCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal SubTotalCurrency { get; set; }
        public int AccountingLayoutIndex { get; set; }
    }

    public class PurchasingReport
    {
        public long URNId { get; set; }
        public int DataSourceSort { get; set; }
        public DateTimeOffset? ReceiptDate { get; set; }
        public string URNNo { get; set; }
        public string ProductName { get; set; }
        public string InvoiceNo { get; set; }
        public string CategoryName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public decimal DPP { get; set; }
        public decimal DPPCurrency { get; set; }
        public decimal VAT { get; set; }
        public decimal VATCurrency { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal Total { get; set; }
        public decimal TotalCurrency { get; set; }
        public bool IsUseVat { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string IPONo { get; set; }
        public string DONo { get; set; }
        public string UPONo { get; set; }
        public string CurrencyCode { get; set; }
        public string CategoryCode { get; set; }
        public string VATNo { get; set; }
        public double Quantity { get; set; }
        public string Uom { get; set; }
        public DateTimeOffset? PIBDate { get; internal set; }
        public string PIBNo { get; set; }
        public decimal PIBBM { get; set; }
        public decimal PIBIncomeTax { get; set; }
        public decimal PIBVat { get; set; }
        public string PIBImportInfo { get; set; }
        public string Remark { get; set; }
        public decimal IncomeTax { get; set; }
        public string AccountingCategoryName { get;  set; }
        public string AccountingCategoryCode { get; internal set; }
        public string AccountingUnitName { get; set; }
        public string AccountingUnitCode { get; internal set; }
        public int AccountingLayoutIndex { get; set; }
        public string IncomeTaxBy { get; set; }
        public string CorrectionNo { get; set; }
        public DateTimeOffset? CorrectionDate { get; set; }
    }
}