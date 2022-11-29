using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils
{
    public class DeliveryOrderDetailDataUtil
    {
        public List<DeliveryOrderDetail> GetNewData(List<ExternalPurchaseOrderItem> externalPurchaseOrderItems)
        {
            List<DeliveryOrderDetail> deliveryOrderDetails = new List<DeliveryOrderDetail>();
            foreach (var item in externalPurchaseOrderItems)
            {
                foreach (var detail in item.Details)
                {
                    deliveryOrderDetails.Add(new DeliveryOrderDetail
                    {
                        EPODetailId = detail.Id,
                        POItemId = detail.POItemId,
                        PRId = item.PRId,
                        PRNo = item.PRNo,
                        PRItemId = detail.PRItemId,
                        UnitId = item.UnitId,
                        UnitCode = item.UnitCode,
                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        DealQuantity = detail.DealQuantity,
                        UomId = detail.DealUomId,
                        UomUnit = detail.DealUomUnit,
                        DOQuantity = detail.DealQuantity - detail.DOQuantity
                    });
                }
            }
            return deliveryOrderDetails;
        }

        public List<DeliveryOrderFulFillMentViewModel> GetNewDataViewModel(List<ExternalPurchaseOrderItem> externalPurchaseOrderItems)
        {
            List<DeliveryOrderFulFillMentViewModel> deliveryOrderFulFillMentViewModels = new List<DeliveryOrderFulFillMentViewModel>();
            foreach (var item in externalPurchaseOrderItems)
            {
                foreach (var detail in item.Details)
                {
                    deliveryOrderFulFillMentViewModels.Add(new DeliveryOrderFulFillMentViewModel
                    {
                        EPODetailId = detail.Id,
                        POItemId = detail.POItemId,
                        purchaseOrder = new PurchaseOrder
                        {
                            purchaseRequest = new PurchaseRequest
                            {
                                _id = item.PRId,
                                no = item.PRNo,
                                unit = new UnitViewModel
                                {
                                    _id = item.UnitId,
                                    code = item.UnitCode,
                                }
                            }
                        },
                        PRItemId = detail.PRItemId,
                        product = new ProductViewModel
                        {
                            _id = detail.ProductId,
                            code = detail.ProductCode,
                            name = detail.ProductName,
                        },
                        deliveredQuantity = detail.DealQuantity - detail.DOQuantity,
                        purchaseOrderQuantity = detail.DealQuantity,
                        purchaseOrderUom = new UomViewModel
                        {
                            _id = detail.DealUomId,
                            unit = detail.DealUomUnit,
                        }
                    });
                }
            }
            return deliveryOrderFulFillMentViewModels;
        }
    }
}
