using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class GarmentDebtCardReportViewModel
    {
        public string DONo { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string NoDebit { get; set; }
        public DateTimeOffset? DateDebit { get; set; }
        public decimal? TotalDebit { get; set; }
        public string CurrencyCodeDebit { get; set; }
        public decimal? TotalIDRDebit { get; set; }
        public string Remark { get; set; }
        public string NoKredit { get; set; }
        public DateTimeOffset? DateKredit { get; set; }
        public double? TotalKredit { get; set; }
        public string CurrencyCodeKredit { get; set; }
        public double? TotalIDRKredit { get; set; }
        public double? TotalEndingBalance{ get; set; }
        public string CurrencyCodeEndingBalance { get; set; }
        public double? TotalIDREndingBalance { get; set; }

    }
}
