using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInvoiceDataUtils
{
    public class GarmentInvoiceItemDataUtil
    {
        private GarmentInvoiceDetailDataUtil garmentInvoiceDetailDataUtil;

        public GarmentInvoiceItemDataUtil(GarmentInvoiceDetailDataUtil garmentInvoiceDetailDataUtil)
        {
            this.garmentInvoiceDetailDataUtil = garmentInvoiceDetailDataUtil;
        }

		public GarmentInvoiceItem GetNewDataViewModel(GarmentDeliveryOrder garmentDeliveryOrder)
		{
			return new GarmentInvoiceItem
			{
				DeliveryOrderId = garmentDeliveryOrder.Id,
			    DeliveryOrderNo = garmentDeliveryOrder.DONo,
				DODate=garmentDeliveryOrder.DODate,
				Details = garmentInvoiceDetailDataUtil.GetNewDataViewModel(garmentDeliveryOrder.Items.ToList())
			};
		}
	}
}
