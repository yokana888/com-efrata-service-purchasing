using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel
{
    public class PurchaseRequestReportViewModel : BaseViewModel
    {
        public string no { get; set; }
        public DateTimeOffset date { get; set; }
        public DateTimeOffset expectedDeliveryDatePR { get; set; }
        public DateTimeOffset expectedDeliveryDatePO { get; set; }
        public string budget { get; set; }
        public string unit { get; set; }
        public string category { get; set; }
        public string productName { get; set; }
        public string productCode { get; set; }
        public double quantity { get; set; }
        public string uom { get; set; }
        public double dealQuantity { get; set; }
        public string dealUom { get; set; }
        public string prStatus { get; set; }
        public string poStatus { get; set; }
        public string remark { get; set; }
    }
}
