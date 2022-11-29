using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;


namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel
{
    public class ExternalPurchaseDeliveryOrderDurationReportViewModel : BaseViewModel
    {
        public DateTimeOffset prDate { get; set; }
        public string prNo { get; set; }
        public DateTimeOffset prCreatedDate { get; set; }
        public string division { get; set; }
        public string unit { get; set; }
        public string budget { get; set; }
        public string category { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public double dealQuantity { get; set; }
        public string dealUomUnit { get; set; }
        public double pricePerDealUnit { get; set; }
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public DateTimeOffset poDate { get; set; }
        public DateTimeOffset orderDate { get; set; }
        public DateTimeOffset ePOCreatedDate { get; set; }
        public DateTimeOffset deliveryDate { get; set; }
        public string ePONo { get; set; }
        public DateTimeOffset dODate { get; set; }
        public DateTimeOffset arrivalDate { get; set; }
        public string dONo { get; set; }
        public int dateDiff { get; set; }
        public string staff { get; set; }
    }
}