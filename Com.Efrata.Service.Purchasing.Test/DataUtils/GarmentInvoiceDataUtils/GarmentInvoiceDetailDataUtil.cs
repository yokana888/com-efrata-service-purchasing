using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInvoiceDataUtils
{
    public class GarmentInvoiceDetailDataUtil
    {
		public List<GarmentInvoiceDetail> GetNewData(List<GarmentDeliveryOrderItem> garmentDeliveryOrderItems)
		{
			List<GarmentInvoiceDetail> deliveryOrderDetails = new List<GarmentInvoiceDetail>();
			foreach (var item in garmentDeliveryOrderItems)
			{
				foreach (var detail in item.Details)
				{
					deliveryOrderDetails.Add(new GarmentInvoiceDetail
					{
						IPOId = detail.POId,
						POSerialNumber=detail.POSerialNumber,
						PRItemId = detail.PRItemId,
						ProductId = detail.ProductId,
						ProductCode = detail.ProductCode,
						ProductName = detail.ProductName,
						DOQuantity = detail.DOQuantity ,
                        RONo = detail.RONo
					});
				}
			}
			return deliveryOrderDetails;
		}

		public List<GarmentInvoiceDetail> GetNewDataViewModel(List<GarmentDeliveryOrderItem> garmentDeliveryOrderItems)
		{
			List<GarmentInvoiceDetail> garmentInvoiceDetailViewModels = new List<GarmentInvoiceDetail>();
			foreach (var item in garmentDeliveryOrderItems)
			{
				foreach (var detail in item.Details)
				{
					garmentInvoiceDetailViewModels.Add(new GarmentInvoiceDetail
					{
                        EPOId = item.EPOId,
						IPOId = detail.POId,
						POSerialNumber = detail.POSerialNumber,
                        RONo = detail.RONo,
						PRItemId = detail.PRItemId,
						ProductId = detail.ProductId,
						ProductCode = detail.ProductCode,
						ProductName = detail.ProductName,
						DOQuantity = detail.DOQuantity,
						UomId =Convert.ToInt64( detail.UomId),
						UomUnit = detail.UomUnit
					});
				}
			}
			return garmentInvoiceDetailViewModels;
		}

	}
}


