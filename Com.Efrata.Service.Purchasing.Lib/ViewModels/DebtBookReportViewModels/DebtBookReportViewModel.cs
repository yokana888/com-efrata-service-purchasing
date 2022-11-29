using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.DebtBookReportViewModels
{
    public class DebtBookReportViewModel
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string DOCurrencyCode { get; set; }
        public double? TotalInitialBalance { get; set; }
        public double? TotalCurrencyInitialBalance { get; set; }
        public double? TotalIDR { get; set; }
        public string BillNo { get; set; }
        public string DONo { get; set; }
        public string PaymentBill { get; set; }
        public string INNo { get; set; }
        public double DebtAge { get; set; }
        public double? InitialBalance { get; set; }
        public double? CurrencyInitialBalance { get; set; }
        public double? IDR { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal CurrencyDebit { get; set; }
        public decimal TotalIDRDebit { get; set; }
        public string NoDebit { get; set; }
        public DateTimeOffset? TglDebit { get; set; }
        public double TotalKredit { get; set; }
        public double? CurrencyKredit { get; set; }
        public double? TotalIDRKredit { get; set; }
        public decimal DifferenceRate { get; set; }
        public double? TotalEndingBalance { get; set; }
        public double? CurrencyEndingBalance { get; set; }
        public double? TotalIDREndingBalance { get; set; }
    }
}
