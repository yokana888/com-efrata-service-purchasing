using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class GarmentBC23ReportViewModel
    {
        public string BeacukaiNo { get; set; }
        public DateTimeOffset BCDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Country { get; set; }
        public string CurrencyCode { get; set; }
        public double TotalAmount { get; set; }
    }
}
