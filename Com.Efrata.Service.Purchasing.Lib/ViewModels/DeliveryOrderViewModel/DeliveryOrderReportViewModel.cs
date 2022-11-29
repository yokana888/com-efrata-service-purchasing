using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel
{
    public class DeliveryOrderReportViewModel : BaseViewModel
    {
        public string no { get; set; }
        public DateTimeOffset supplierDoDate { get; set; } // DODate
        public DateTimeOffset date { get; set; } // ArrivalDate
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string ePONo { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string productRemark { get; set; }
        public double dealQuantity { get; set; }
        public double dOQuantity { get; set; }
        public double remainingQuantity { get; set; }
        public string uomUnit { get; set; }
        public long ePODetailId { get; set; }
    }
}