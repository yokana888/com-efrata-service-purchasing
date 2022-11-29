using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel
{
    public class UnitPaymentOrderItemViewModel : BaseViewModel
    {
        public UnitReceiptNote unitReceiptNote { get; set; }
    }

    public class UnitReceiptNote
    {
        public long _id { get; set; }
        public string no { get; set; }
        public DeliveyOrder deliveryOrder { get; set; }
        public List<UnitPaymentOrderDetailViewModel> items { get; set; }
    }

    public class DeliveyOrder
    {
        public long _id { get; set; }
        public string no { get; set; }
    }
}
