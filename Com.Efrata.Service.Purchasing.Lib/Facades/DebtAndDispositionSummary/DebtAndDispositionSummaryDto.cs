using System;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary
{
    public class DebtAndDispositionSummaryDto
    {
        public string CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        public string CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public bool IsImport { get; set; }
        public bool IsPaid { get; set; }
        public double DebtPrice { get; set; }
        public double DebtQuantity { get; set; }
        public double DispositionPrice { get; set; }
        public double DispositionQuantity { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public double Total { get; set; }
        public double DispositionTotal { get; set; }
        public double DebtTotal { get; set; }
        public string IncomeTaxBy { get; set; }
        public bool UseIncomeTax { get; set; }
        public string IncomeTaxRate { get; set; }
        public bool UseVat { get; set; }
        public int CategoryLayoutIndex { get; set; }
        public string AccountingUnitName { get; set; }
        public string AccountingUnitId { get; set; }
        public DateTimeOffset UPODate { get; set; }
        public string UPONo { get; set; }
        public string URNNo { get; set; }
        public string InvoiceNo { get; set; }
        public string SupplierName { get; set; }
    }
}