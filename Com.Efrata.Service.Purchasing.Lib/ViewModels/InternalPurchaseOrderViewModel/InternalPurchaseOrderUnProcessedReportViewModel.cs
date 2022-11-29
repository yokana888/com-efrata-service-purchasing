using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel
{
    public class InternalPurchaseOrderUnProcessedReportViewModel : BaseViewModel
    {
        public string PRNo { get; set; }
        public DateTimeOffset PRDate { get; set; }
        public DateTimeOffset ExpectedDeliveryDate { get; set; }
        public string UnitName { get; set; }
        public string CategoryName { get; set; }        
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string UOMUnit { get; set; }
        public string StaffName { get; set; }
    }
}
