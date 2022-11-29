using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchasingBookReportViewModel
{
    public class GarmentPurchasingBookReportViewModel
    {
        public string SupplierName { get; set; }
        public string BillNo { get; set; }
        public string PaymentBill { get; set; }
        public string Dono { get; set; }
        public string InvoiceNo { get; set; }
        public string InternNo { get; set; }
        public string Tipe { get; set; }
        public DateTimeOffset internDate { get; set; }
        public DateTimeOffset paymentduedate { get; set; }
        public double dpp { get; set; }
        public double priceTotal { get; set; }
        public double ppn { get; set; }
        //public double ppn2 { get; set; }
        public double? total { get; set; }
        public double? totalppn { get; set; }
        public string CurrencyCode { get; set; }
        public double? CurrencyRate { get; set; }
    }
}
