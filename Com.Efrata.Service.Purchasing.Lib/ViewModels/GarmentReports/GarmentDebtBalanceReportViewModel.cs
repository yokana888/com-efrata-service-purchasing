using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports

{
    public class GarmentDebtBalanceViewModel
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string DOCurrencyCode { get; set; }
        public double? TotalInitialBalance { get; set; }
        public string BillNo { get; set; }
        public string CodeRequirment { get; set; }
        public string DONo { get; set; }
        public string PaymentBill { get; set; }
        public double? InitialBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public string NoDebit { get; set; }
        public DateTimeOffset? TglDebit { get; set; }
        public double TotalKredit { get; set; }
        public double? TotalEndingBalance { get; set; }
    }
}
