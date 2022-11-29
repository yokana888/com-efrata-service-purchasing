using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel
{
    public class PurchaseRequestGenerateDataViewModel : BaseViewModel
    {
        public string PRNo { get; set; }
        public DateTimeOffset PRDate { get; set; }
        public DateTimeOffset PRExpectedDeliveryDate { get; set; }
        public DateTimeOffset PRReceiptDate { get; set; }
        public string Budget { get; set; }
        public string UnitName { get; set; }
        public string Category { get; set; }
        public string PRRemark { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string UOMUnit { get; set; }
        public string Remark { get; set; }
        public string UserCreated { get; set; }
        public string IsPosted { get; set; }
    }
}
