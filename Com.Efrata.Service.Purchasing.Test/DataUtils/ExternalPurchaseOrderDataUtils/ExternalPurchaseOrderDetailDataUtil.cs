using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils
{
    public class ExternalPurchaseOrderDetailDataUtil
    {

        public ExternalPurchaseOrderDetail GetNewData(List<InternalPurchaseOrderItem> internalPurchaseOrderItem) => new ExternalPurchaseOrderDetail
        {

            POItemId = internalPurchaseOrderItem[0].Id,
            PRItemId = Convert.ToInt64(internalPurchaseOrderItem[0].PRItemId),
            ProductId = "1",
            ProductCode = "ProductCode",
            ProductName = "ProductName",
            DefaultQuantity = internalPurchaseOrderItem[0].Quantity,
            DealUomId = "UomId",
            DealUomUnit = "Uom",
            DefaultUomId = "1",
            DefaultUomUnit = "Uom",
            ProductRemark = "Remark",
            PriceBeforeTax = 1000,
            PricePerDealUnit = 200,
            DealQuantity = internalPurchaseOrderItem[0].Quantity,
            Conversion=1
        };

        public ExternalPurchaseOrderDetailViewModel GetNewDataViewModel(List<InternalPurchaseOrderItem> internalPurchaseOrderItem) => new ExternalPurchaseOrderDetailViewModel
        {
            poItemId = internalPurchaseOrderItem[0].Id,
            prItemId = Convert.ToInt64(internalPurchaseOrderItem[0].PRItemId),
            product = new ProductViewModel
            {
                _id = "ProductId",
                code = "ProductCode",
                name = "ProductName",
                uom =  new UomViewModel
                {
                    _id = "UomId",
                    unit = "Uom",
                }
            },
            defaultQuantity = 100,
            dealUom=new UomViewModel
            {
                _id = "UomId",
                unit = "Uom",
            },
            defaultUom = new UomViewModel
            {
                _id = "UomId",
                unit = "Uom",
            },
            productRemark = "Remark",
            priceBeforeTax = 1000,
            pricePerDealUnit = 200,
            conversion=1,
            dealQuantity = 100,
            productPrice=10000
        };
    }
}
