using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel
{
    public class GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel : BaseViewModel
    {
        public string roNo { get; set; }
        public string planPO { get; set; }
        public string artikelNo { get; set; }
        public string buyerName { get; set; }
        public string unit { get; set; }
        public string category { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public double productQuantity { get; set; }
        public string productUom { get; set; }
        public double productPrice { get; set; }
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string poIntNo { get; set; }
        public DateTimeOffset poIntCreatedDate { get; set; }
        public string poEksNo { get; set; }
        public DateTimeOffset poEksCreatedDate { get; set; }
        public DateTimeOffset expectedDate { get; set; }
        public int dateDiff { get; set; }
        public string staff { get; set; }
      
    }
}
