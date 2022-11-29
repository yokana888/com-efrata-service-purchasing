using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel
{
    public class InternalPurchaseOrderReportViewModel : BaseViewModel
    {
        public string prNo { get; set; }
        public DateTimeOffset prDate { get; set; }
        public DateTimeOffset expectedDeliveryDatePO { get; set; }
        public string unit { get; set; }
        public string category { get; set; }
        public string productName { get; set; }
        public double quantity { get; set; }
        public string uom { get; set; }
        public string poStatus { get; set; }
        public string staff { get; set; }
    }
}
