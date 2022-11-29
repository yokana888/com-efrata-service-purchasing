using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils
{
    public class UnitReceiptNoteItemDataUtil
    {
        //private UnitReceiptNoteDataUtil unitReceiptNoteDataUtil;

        public Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem GetNewData(List<DeliveryOrderItem> deliveryOrderItem)
        {
            List<DeliveryOrderDetail> doDetail = new List<DeliveryOrderDetail>(deliveryOrderItem[0].Details);
            return new Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem
            {
                ProductId = doDetail[0].ProductId,
                ProductCode = doDetail[0].ProductCode,
                ProductName = doDetail[0].ProductName,
                ReceiptQuantity = doDetail[0].DOQuantity,
                UomId = doDetail[0].UomId,
                Uom = doDetail[0].UomUnit,
                ProductRemark = "Test",
                PRItemId = doDetail[0].PRItemId,
                PRId= doDetail[0].PRId,
                PRNo= doDetail[0].PRNo,
                EPODetailId= doDetail[0].EPODetailId,
                EPOId = deliveryOrderItem[0].EPOId,
                EPONo = deliveryOrderItem[0].EPONo,
                IncomeTaxBy = "Supplier",
                DODetailId =doDetail[0].Id
            };
        }

        public UnitReceiptNoteItemViewModel GetNewDataViewModel(List<DeliveryOrderItem> deliveryOrderItem)
        {
            List<DeliveryOrderDetail> doDetail = new List<DeliveryOrderDetail>(deliveryOrderItem[0].Details);

            return new UnitReceiptNoteItemViewModel
            {
                epoId = deliveryOrderItem[0].EPOId,
                epoNo = deliveryOrderItem[0].EPONo,
                incomeTaxBy = "Supplier",
                prId = doDetail[0].PRId,
                prNo = doDetail[0].PRNo,
                prItemId= doDetail[0].PRItemId,
                doDetailId= doDetail[0].Id,
                deliveredQuantity= doDetail[0].DOQuantity,
                epoDetailId=doDetail[0].EPODetailId,
                product = new ProductViewModel
                {
                    _id = "ProductId",
                    code = "ProductCode",
                    name = "ProductName",
                    uom= new UomViewModel
                    {
                        _id= doDetail[0].UomId,
                        unit= "uom"
                    }
                },
                uom= "uom",
                uomId= doDetail[0].UomId
            };
        }
    }
}
