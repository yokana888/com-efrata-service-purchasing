using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel
{
    public class GarmentDeliveryOrderItemViewModel : BaseViewModel
    {
        public PurchaseOrderExternal purchaseOrderExternal { get; set; }
        public CurrencyViewModel currency { get; set; }
        public List<GarmentDeliveryOrderFulfillmentViewModel> fulfillments { get; set; }
        public int paymentDueDays { get; set; }
		public double incomeTaxRate { get; set; }
		public bool useVat { get; set; }
        public bool useIncomeTax { get; set; }
        public int ePOId { get; set; }
    }

    public class PurchaseOrderExternal
    {
        public long Id { get; set; }
        public string no { get; set; }
    }
}
