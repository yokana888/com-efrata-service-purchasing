using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote
{
    public class DetailCreditBalanceReportViewModel
    {
        public DetailCreditBalanceReportViewModel()
        {
            Reports = new List<DetailCreditBalanceReport>();
            AccountingUnitSummaries = new List<SummaryDCB>();
            //CurrencySummaries = new List<SummaryDCB>();
        }
        public List<DetailCreditBalanceReport> Reports { get; set; }
        public List<SummaryDCB> AccountingUnitSummaries { get; set; }
        public List<SummaryDCB> CurrencySummaries { get; set; }
        public List<SummaryDCB> CategorySummaries { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AccountingUnitSummaryTotal { get; set; }
    }

    public class SummaryDCB
    {
        public string AccountingUnitName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal SubTotalIDR { get; set; }
        public int AccountingLayoutIndex { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class DetailCreditBalanceReport
    {
        public string CurrencyId { get; set; }
        public double CurrencyRate { get; set; }
        public string CategoryCode { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public bool IsImport { get; set; }
        public bool IsPaid { get; set; }
        public double DebtPrice { get; set; }
        public double DebtQuantity { get; set; }
        public double DebtTotal { get; set; }
        public string IncomeTaxBy { get; internal set; }
        public bool UseIncomeTax { get; internal set; }
        public string IncomeTaxRate { get; internal set; }
        public bool UseVat { get; internal set; }

        public DateTimeOffset? UPODate { get; set; }
        public string UPONo { get; set; }
        public string URNNo { get; set; }
        public string InvoiceNo { get; set; }
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
        public string AccountingUnitName { get; internal set; }
        public DateTimeOffset? DueDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIDR { get; set; }
        public string CategoryId { get; set; }
        public string DivisionName { get; set; }
        public int CategoryLayoutIndex { get; internal set; }
    }
}