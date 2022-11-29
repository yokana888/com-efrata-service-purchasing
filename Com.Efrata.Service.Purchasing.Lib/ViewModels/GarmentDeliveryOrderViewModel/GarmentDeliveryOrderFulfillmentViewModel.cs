using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel
{
    public class GarmentDeliveryOrderFulfillmentViewModel : BaseViewModel
    {
        public long ePOItemId { get; set; }
        public int pOId { get; set; }
        public int pOItemId { get; set; }
        public long pRId { get; set; }
        public string pRNo { get; set; }
        public long pRItemId { get; set; }
        public string poSerialNumber { get; set; }

        public UnitViewModel unit { get; set; }

        public GarmentProductViewModel product { get; set; }

        public double doQuantity { get; set; }
        public double returQuantity { get; set; }
        public double dealQuantity { get; set; }
        public double conversion { get; set; }
        public UomViewModel purchaseOrderUom { get; set; } // UOM

        public double smallQuantity { get; set; }
        public UomViewModel smallUom { get; set; }

        public double pricePerDealUnit { get; set; }
        public double priceTotal { get; set; }


        public string rONo { get; set; }
        public double receiptQuantity { get; set; }

        public bool isSave { get; set; }

        public double quantityCorrection { get; set; }
        public double pricePerDealUnitCorrection { get; set; }
        public double priceTotalCorrection { get; set; }

        public string codeRequirment { get; set; }
        public string customsCategory { get; set; }
    }
}
