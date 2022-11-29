using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel
{
    public class DeliveryOrderFulFillMentViewModel : BaseViewModel
    {
        public long EPODetailId { get; set; }
        public long POItemId { get; set; }
        public PurchaseOrder purchaseOrder { get; set; }
        public long PRItemId { get; set; }

        public ProductViewModel product { get; set; }
        public string remark { get; set; }
        public double deliveredQuantity { get; set; } // DOQuantity
        public double purchaseOrderQuantity { get; set; } // DealQuantity
        public UomViewModel purchaseOrderUom { get; set; }
        public double receiptQuantity { get; set; }
        public bool isClosed { get; set; }
    }

    public class PurchaseOrder
    {
        public PurchaseRequest purchaseRequest { get; set; }
    }

    public class PurchaseRequest
    {
        public long _id { get; set; }
        public string no { get; set; }
        public UnitViewModel unit { get; set; }
    }
}
