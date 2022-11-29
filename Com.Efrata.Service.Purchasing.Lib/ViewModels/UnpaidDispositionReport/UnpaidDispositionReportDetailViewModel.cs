using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnpaidDispositionReport
{
    public class UnpaidDispositionReportDetailViewModel
    {
        public UnpaidDispositionReportDetailViewModel()
        {
            Reports = new List<DispositionReport>();
            UnitSummaries = new List<Summary>();
        }
        public List<DispositionReport> Reports { get; set; }
        public List<Summary> UnitSummaries { get; set; }
        public List<Summary> CurrencySummaries { get; set; }
        public List<Summary> CategorySummaries { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal UnitSummaryTotal { get; set; }
    }

    public class Summary
    {
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal SubTotalCurrency { get; set; }
        public int AccountingLayoutIndex { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class DispositionReport
    {
        public string URNNo { get; set; }
        public string InvoiceNo { get; set; }
        public decimal DPP { get; set; }
        public decimal DPPCurrency { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }
        public decimal TotalCurrency { get; set; }
        public string SupplierName { get; set; }
        public string UPONo { get; set; }
        public decimal IncomeTax { get; set; }
        public string DispositionNo { get; set; }
        public DateTimeOffset? DispositionDate { get; set; }
        public DateTimeOffset? PaymentDueDate { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string AccountingCategoryName { get; internal set; }
        public string AccountingCategoryCode { get; internal set; }
        public string AccountingUnitName { get; internal set; }
        public string AccountingUnitCode { get; internal set; }
        public int AccountingLayoutIndex { get; set; }
        public string IncomeTaxBy { get; set; }
        public string DivisionName { get; set; }
        public int CategoryLayoutIndex { get; internal set; }
    }
}
