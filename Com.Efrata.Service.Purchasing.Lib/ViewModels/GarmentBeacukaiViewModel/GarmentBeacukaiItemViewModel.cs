using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel
{
	public class GarmentBeacukaiItemViewModel : BaseViewModel
	{
		public Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel.GarmentDeliveryOrderViewModel deliveryOrder { get; set; }
		public string billNo { get; set; }
		public double quantity { get; set; }
		public bool selected { get; set; }



	}
}
