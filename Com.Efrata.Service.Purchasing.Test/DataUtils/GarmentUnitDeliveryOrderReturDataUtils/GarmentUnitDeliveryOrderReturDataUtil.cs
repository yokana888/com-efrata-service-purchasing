using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderReturFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderReturDataUtils
{
    public class GarmentUnitDeliveryOrderReturDataUtil
    {
        private readonly GarmentUnitReceiptNoteDataUtil UNDataUtil;
        private readonly GarmentUnitDeliveryOrderReturFacade facade;
        public GarmentUnitDeliveryOrderReturDataUtil(GarmentUnitDeliveryOrderReturFacade facade, GarmentUnitReceiptNoteDataUtil UNDataUtil)
        {
            this.facade = facade;
            this.UNDataUtil = UNDataUtil;
        }

        public async Task<GarmentUnitDeliveryOrder> GetNewData()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;

            var garmentUnitReceiptNote = await Task.Run(() => UNDataUtil.GetTestDataWithStorage(nowTicks));

            GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = new GarmentUnitDeliveryOrder
            {
                UnitDOType = "RETUR",
                UnitDODate = DateTimeOffset.Now,
                UnitSenderId = garmentUnitReceiptNote.UnitId,
                UnitSenderCode = garmentUnitReceiptNote.UnitCode,
                UnitSenderName = garmentUnitReceiptNote.UnitName,
                StorageId = garmentUnitReceiptNote.StorageId,
                StorageCode = garmentUnitReceiptNote.StorageCode,
                StorageName = garmentUnitReceiptNote.StorageName,
                DONo = garmentUnitReceiptNote.DONo,
                Items = new List<GarmentUnitDeliveryOrderItem>()
            };

            foreach (var item in garmentUnitReceiptNote.Items)
            {
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        URNId = garmentUnitReceiptNote.Id,
                        URNItemId = item.Id,
                        URNNo = garmentUnitReceiptNote.URNNo,
                        POSerialNumber = item.POSerialNumber,
                        RONo = item.RONo,
                        ProductId = item.ProductId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Quantity = (double)(item.SmallQuantity - item.OrderQuantity),
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        ReturQuantity = 20,
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DefaultDOQuantity = 20
            
                    });
            }

            return garmentUnitDeliveryOrder;
        }

        public async Task<GarmentUnitDeliveryOrder> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data);
            return data;
        }
        public async Task<GarmentUnitDeliveryOrder> GetNewDataMultipleItem()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;

            var garmentUnitReceiptNote1 = await Task.Run(() => UNDataUtil.GetTestDataWithStorage());
            var garmentUnitReceiptNote2 = await Task.Run(() => UNDataUtil.GetTestDataWithStorage(nowTicks + 1));
            GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = new GarmentUnitDeliveryOrder
            {
                UnitDOType = "RETUR",
                UnitDODate = DateTimeOffset.Now,
                UnitSenderId = garmentUnitReceiptNote1.UnitId,
                UnitSenderCode = garmentUnitReceiptNote1.UnitCode,
                UnitSenderName = garmentUnitReceiptNote1.UnitName,
                StorageId = garmentUnitReceiptNote1.StorageId,
                StorageCode = garmentUnitReceiptNote1.StorageCode,
                StorageName = garmentUnitReceiptNote1.StorageName,
                Items = new List<GarmentUnitDeliveryOrderItem>()
            };

            foreach (var item in garmentUnitReceiptNote1.Items)
            {
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        URNId = garmentUnitReceiptNote1.Id,
                        URNItemId = item.Id,
                        URNNo = garmentUnitReceiptNote1.URNNo,
                        POSerialNumber = item.POSerialNumber,
                        RONo = item.RONo,
                        ProductId = item.ProductId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Quantity = (double)(item.SmallQuantity - item.OrderQuantity),
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        DOCurrencyRate = garmentUnitReceiptNote1.DOCurrencyRate,
                        ReturQuantity = 20,
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DefaultDOQuantity = 20
                    });
            }




            foreach (var item in garmentUnitReceiptNote2.Items)
            {
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        URNId = garmentUnitReceiptNote1.Id,
                        URNItemId = item.Id,
                        URNNo = garmentUnitReceiptNote1.URNNo,
                        POSerialNumber = item.POSerialNumber,
                        RONo = item.RONo,
                        ProductId = item.ProductId + 1,
                        ProductCode = item.ProductCode + $"{nowTicks}",
                        ProductName = item.ProductName + $"{nowTicks}",
                        Quantity = (double)(item.SmallQuantity - item.OrderQuantity),
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        ReturQuantity = 20,
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DefaultDOQuantity = 20,
                        DOCurrencyRate = garmentUnitReceiptNote1.DOCurrencyRate
                    });
            }


            return garmentUnitDeliveryOrder;
        }

        public async Task<GarmentUnitDeliveryOrder> GetTestDataMultipleItem()
        {
            var data = await GetNewDataMultipleItem();
            await facade.Create(data);
            return data;
        }
    }
}
