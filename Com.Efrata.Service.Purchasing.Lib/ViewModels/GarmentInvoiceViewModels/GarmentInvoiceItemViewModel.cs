using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels
{
    public class GarmentInvoiceItemViewModel : BaseViewModel
    {
        public Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel.GarmentDeliveryOrderViewModel deliveryOrder { get; set; }
        public List<GarmentInvoiceDetailViewModel> details { get; set; }
	}
}
