using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderDataUtils
{
    public class GarmentUnitDeliveryOrderDataUtil
    {
        private readonly GarmentUnitReceiptNoteDataUtil UNDataUtil;
        private readonly GarmentUnitDeliveryOrderFacade facade;
        public GarmentUnitDeliveryOrderDataUtil(GarmentUnitDeliveryOrderFacade facade, GarmentUnitReceiptNoteDataUtil UNDataUtil)
        {
            this.facade = facade;
            this.UNDataUtil = UNDataUtil;
        }

        public async Task<GarmentUnitDeliveryOrder> GetNewData(GarmentUnitReceiptNote unitReceiptNote1 = null)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;

            var garmentUnitReceiptNote = unitReceiptNote1 ?? await Task.Run(() => UNDataUtil.GetTestDataWithStorage(nowTicks));

            GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = new GarmentUnitDeliveryOrder
            {
                UnitDOType = "SAMPLE",
                UnitDODate = DateTimeOffset.Now,
                UnitSenderId = garmentUnitReceiptNote.UnitId,
                UnitRequestCode = garmentUnitReceiptNote.UnitCode,
                UnitRequestName = garmentUnitReceiptNote.UnitName,
                UnitRequestId = garmentUnitReceiptNote.UnitId,
                UnitSenderCode = garmentUnitReceiptNote.UnitCode,
                UnitSenderName = garmentUnitReceiptNote.UnitName,
                StorageId = garmentUnitReceiptNote.StorageId,
                StorageCode = garmentUnitReceiptNote.StorageCode,
                StorageName = garmentUnitReceiptNote.StorageName,
                RONo = garmentUnitReceiptNote.Items.Select(i => i.RONo).FirstOrDefault(),
                Article = $"Article{nowTicks}",
                Items = new List<GarmentUnitDeliveryOrderItem>()
            };

            foreach (var item in garmentUnitReceiptNote.Items)
            {
                var garmentDOItems = UNDataUtil.ReadDOItemsByURNItemId((int)item.Id);
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        FabricType = "FABRIC",
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
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DOItemsId = (int)garmentDOItems.Id,
						DOCurrencyRate=1
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

        public async Task<GarmentUnitDeliveryOrder> GetTestDataMarketing()
        {
            var data = await GetNewData();
            data.UnitDOType = "MARKETING";
            await facade.Create(data);
            return data;
        }
        public async Task<GarmentUnitDeliveryOrder> GetNewDataMultipleItem(GarmentUnitReceiptNote unitReceiptNote1 = null, GarmentUnitReceiptNote unitReceiptNote2 = null)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;

            var garmentUnitReceiptNote1 = unitReceiptNote1 ?? await Task.Run(() => UNDataUtil.GetTestDataWithStorage());
            var garmentUnitReceiptNote2 = unitReceiptNote2 ?? await Task.Run(() => UNDataUtil.GetTestDataWithStorage(nowTicks + 1));
            GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = new GarmentUnitDeliveryOrder
            {
                UnitDOType = "SAMPLE",
                DOId= garmentUnitReceiptNote1.DOId,
                UnitDODate = DateTimeOffset.Now,
                UnitSenderId = garmentUnitReceiptNote1.UnitId,
                UnitRequestCode = garmentUnitReceiptNote1.UnitCode,
                UnitRequestName = garmentUnitReceiptNote1.UnitName,
                UnitRequestId = garmentUnitReceiptNote1.UnitId,
                UnitSenderCode = garmentUnitReceiptNote1.UnitCode,
                UnitSenderName = garmentUnitReceiptNote1.UnitName,
                StorageId = garmentUnitReceiptNote1.StorageId,
                StorageCode = garmentUnitReceiptNote1.StorageCode,
                StorageName = garmentUnitReceiptNote1.StorageName,
                RONo = garmentUnitReceiptNote1.Items.Select(i => i.RONo).FirstOrDefault(),
                Article = $"Article{nowTicks}",
                Items = new List<GarmentUnitDeliveryOrderItem>()
            };

            foreach (var item in garmentUnitReceiptNote1.Items)
            {
                var garmentDOItems = UNDataUtil.ReadDOItemsByURNItemId((int)item.Id);
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        FabricType = "FABRIC",
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
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DOCurrencyRate = garmentUnitReceiptNote1.DOCurrencyRate,
                        DOItemsId = (int)garmentDOItems.Id
                    });
            }


            

            foreach (var item in garmentUnitReceiptNote2.Items)
            {
                var garmentDOItems = UNDataUtil.ReadDOItemsByURNItemId((int)item.Id);
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        FabricType = "FABRIC",
                        URNId = garmentUnitReceiptNote1.Id,
                        URNItemId = item.Id,
                        URNNo = garmentUnitReceiptNote1.URNNo,
                        POSerialNumber = item.POSerialNumber,
                        RONo = item.RONo,
                        ProductId = item.ProductId+1,
                        ProductCode = item.ProductCode + $"{nowTicks}",
                        ProductName = item.ProductName + $"{nowTicks}",
                        Quantity = (double)(item.SmallQuantity - item.OrderQuantity),
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        DOCurrencyRate = garmentUnitReceiptNote1.DOCurrencyRate,
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DOItemsId = (int)garmentDOItems.Id
                    });
            }


            return garmentUnitDeliveryOrder;
        }

        public async Task<GarmentUnitDeliveryOrder> GetNewDataMultipleItem_DOCurrency(GarmentUnitReceiptNote unitReceiptNote1 = null, GarmentUnitReceiptNote unitReceiptNote2 = null)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;

            var garmentUnitReceiptNote1 = unitReceiptNote1 ?? await Task.Run(() => UNDataUtil.GetTestDataWithStorage_DOCurrency());
            var garmentUnitReceiptNote2 = unitReceiptNote2 ?? await Task.Run(() => UNDataUtil.GetTestDataWithStorage_DOCurrency(nowTicks + 1));
            GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = new GarmentUnitDeliveryOrder
            {
                UnitDOType = "SAMPLE",
                DOId = garmentUnitReceiptNote1.DOId,
                UnitDODate = DateTimeOffset.Now,
                UnitSenderId = garmentUnitReceiptNote1.UnitId,
                UnitRequestCode = garmentUnitReceiptNote1.UnitCode,
                UnitRequestName = garmentUnitReceiptNote1.UnitName,
                UnitRequestId = garmentUnitReceiptNote1.UnitId,
                UnitSenderCode = garmentUnitReceiptNote1.UnitCode,
                UnitSenderName = garmentUnitReceiptNote1.UnitName,
                StorageId = garmentUnitReceiptNote1.StorageId,
                StorageCode = garmentUnitReceiptNote1.StorageCode,
                StorageName = garmentUnitReceiptNote1.StorageName,
                RONo = garmentUnitReceiptNote1.Items.Select(i => i.RONo).FirstOrDefault(),
                Article = $"Article{nowTicks}",
                Items = new List<GarmentUnitDeliveryOrderItem>()
            };

            foreach (var item in garmentUnitReceiptNote1.Items)
            {
                var garmentDOItems = UNDataUtil.ReadDOItemsByURNItemId((int)item.Id);
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        FabricType = "FABRIC",
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
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DOCurrencyRate = garmentUnitReceiptNote1.DOCurrencyRate,
                        DOItemsId = (int)garmentDOItems.Id
                    });
            }




            foreach (var item in garmentUnitReceiptNote2.Items)
            {
                var garmentDOItems = UNDataUtil.ReadDOItemsByURNItemId((int)item.Id);
                garmentUnitDeliveryOrder.Items.Add(
                    new GarmentUnitDeliveryOrderItem
                    {
                        IsSave = true,
                        DODetailId = item.DODetailId,
                        EPOItemId = item.EPOItemId,
                        POItemId = item.POItemId,
                        PRItemId = item.PRItemId,
                        FabricType = "FABRIC",
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
                        DOCurrencyRate = garmentUnitReceiptNote1.DOCurrencyRate,
                        ReturUomId = item.UomId,
                        ReturUomUnit = item.UomUnit,
                        DOItemsId = (int)garmentDOItems.Id
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

        public async Task<GarmentUnitDeliveryOrder> GetTestDataMultipleItem_DOCurrency()
        {
            var data = await GetNewDataMultipleItem_DOCurrency();
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitDeliveryOrder> GetTestDataMultipleItemForURNProcess()
        {
            var data = await Task.Run(() => GetTestDataMultipleItem());

            var data2= await GetNewDataMultipleItem();
            data2.UnitDOFromId = data.Id;
            await facade.Create(data2);
            return data2;
        }

    }
}
