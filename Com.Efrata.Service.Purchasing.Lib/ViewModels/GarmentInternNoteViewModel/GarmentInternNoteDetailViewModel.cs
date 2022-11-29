using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Moonlay.Models;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class GarmentInternNoteDetailViewModel : BaseViewModel
    {
        public long ePOId { get; set; }
        public string ePONo { get; set; }
        public string poSerialNumber { get; set; }
        public string roNo { get; set; }
        public double pricePerDealUnit { get; set; }
        public double priceTotal { get; set; }
        public int paymentDueDays { get; set; }
        public double quantity { get; set; }
        public int invoiceDetailId { get; set; }
        public DateTimeOffset paymentDueDate { get; set; }

        public Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel.GarmentDeliveryOrderViewModel deliveryOrder { get; set; }
        
        /*Product*/
        public ProductViewModel product { get; set; }

        public UomViewModel uomUnit { get; set; }

        public UnitViewModel unit { get; set; }
        public long dODetailId { get; set; }

    }
}
