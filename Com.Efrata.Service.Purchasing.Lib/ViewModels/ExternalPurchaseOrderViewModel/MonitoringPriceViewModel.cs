using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel 
{
    public class MonitoringPriceViewModel
    {
        public string EPONo { get; set; }
        public DateTimeOffset EPODate { get; set; }
        public string UnitName { get; set; }
        public string PRNo { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset InvoiceDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string UOMUnit { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyRate { get; set; }
        public string PricePerDeal { get; set; }
        public string Amount { get; set; }
    }
}
