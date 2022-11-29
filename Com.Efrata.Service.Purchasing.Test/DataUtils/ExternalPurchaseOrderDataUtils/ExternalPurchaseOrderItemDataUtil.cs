using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils
{
    public class ExternalPurchaseOrderItemDataUtil
    {
        private ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil;
        public ExternalPurchaseOrderItemDataUtil(ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil)
        {
            this.externalPurchaseOrderDetailDataUtil = externalPurchaseOrderDetailDataUtil;
        }
        public ExternalPurchaseOrderItem GetNewData(InternalPurchaseOrder internalPurchaseOrder)
        {
            List<InternalPurchaseOrderItem> detail = new List<InternalPurchaseOrderItem>();
            foreach (var POdetail in internalPurchaseOrder.Items)
            {
                detail.Add(POdetail);
            }
            return new ExternalPurchaseOrderItem
            {
                POId = internalPurchaseOrder.Id,
                PRId = Convert.ToInt64(internalPurchaseOrder.PRId),
                PONo = internalPurchaseOrder.PONo,
                PRNo = internalPurchaseOrder.PRNo,
                UnitCode = "unitcode",
                UnitName = "unit",
                UnitId = "1",
                Details = new List<ExternalPurchaseOrderDetail> { externalPurchaseOrderDetailDataUtil.GetNewData(detail) }


            };



        }

        public ExternalPurchaseOrderItemViewModel GetNewDataViewModel(InternalPurchaseOrder internalPurchaseOrder)
        {
            List<InternalPurchaseOrderItem> detail = new List<InternalPurchaseOrderItem>();
            foreach (var POdetail in internalPurchaseOrder.Items)
            {
                detail.Add(POdetail);
            }
            return new ExternalPurchaseOrderItemViewModel
            {
                poId = internalPurchaseOrder.Id,
                prId = Convert.ToInt64(internalPurchaseOrder.PRId),
                poNo = internalPurchaseOrder.PONo,
                prNo = internalPurchaseOrder.PRNo,
                unit = new UnitViewModel
                {
                    _id = internalPurchaseOrder.UnitId,
                    code = internalPurchaseOrder.UnitCode,
                    name = internalPurchaseOrder.UnitName
                },
                details = new List<ExternalPurchaseOrderDetailViewModel> { externalPurchaseOrderDetailDataUtil.GetNewDataViewModel(detail) }

            };
        }
    }
}
