using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitExpenditureDataUtils
{
    public class GarmentUnitExpenditureNoteDataUtil
    {
        private readonly GarmentUnitExpenditureNoteFacade facade;
        private readonly GarmentUnitDeliveryOrderDataUtil garmentUnitDeliveryOrderDataUtil;

        public GarmentUnitExpenditureNoteDataUtil(GarmentUnitExpenditureNoteFacade facade, GarmentUnitDeliveryOrderDataUtil garmentUnitDeliveryOrderDataUtil)
        {
            this.facade = facade;
            this.garmentUnitDeliveryOrderDataUtil = garmentUnitDeliveryOrderDataUtil;
        }

        public async Task<GarmentUnitExpenditureNote> GetNewData(GarmentUnitDeliveryOrder garmentunitdeliveryorder = null)
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var garmentUnitDeliveryOrder = garmentunitdeliveryorder ?? await Task.Run(() => garmentUnitDeliveryOrderDataUtil.GetTestDataMultipleItem());

            var garmentUnitExpenditureNote = new GarmentUnitExpenditureNote
            {
                UnitSenderId = garmentUnitDeliveryOrder.UnitSenderId,
                UnitSenderCode = garmentUnitDeliveryOrder.UnitSenderCode,
                UnitSenderName = garmentUnitDeliveryOrder.UnitSenderName,

                UnitRequestId = garmentUnitDeliveryOrder.UnitRequestId,
                UnitRequestCode = garmentUnitDeliveryOrder.UnitRequestCode,
                UnitRequestName = garmentUnitDeliveryOrder.UnitRequestName,

                UnitDOId = garmentUnitDeliveryOrder.Id,
                UnitDONo = garmentUnitDeliveryOrder.UnitDONo,

                StorageId = garmentUnitDeliveryOrder.StorageId,
                StorageCode = garmentUnitDeliveryOrder.StorageCode,
                StorageName = garmentUnitDeliveryOrder.StorageName,

                StorageRequestId = garmentUnitDeliveryOrder.StorageRequestId,
                StorageRequestCode = garmentUnitDeliveryOrder.StorageRequestCode,
                StorageRequestName = garmentUnitDeliveryOrder.StorageRequestName,

                ExpenditureType = "PROSES",
                ExpenditureTo = "PROSES",
                UENNo = "UENNO12345",

                ExpenditureDate = DateTimeOffset.Now,

                IsPreparing = false,

                Items = new List<GarmentUnitExpenditureNoteItem>()
            };

            foreach (var item in garmentUnitDeliveryOrder.Items)
            {
                var garmentUnitExpenditureNoteItem = new GarmentUnitExpenditureNoteItem
                {
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = 5,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,
                };
                new GarmentUnitExpenditureNoteItem
                {
                    Id = 0,
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = item.Quantity,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,


                };

                garmentUnitExpenditureNote.Items.Add(garmentUnitExpenditureNoteItem);

            }

            return garmentUnitExpenditureNote;
        }

        public async Task<GarmentUnitExpenditureNote> GetNewDataTypeTransfer(GarmentUnitDeliveryOrder garmentunitdeliveryorder = null)
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var garmentUnitDeliveryOrder = garmentunitdeliveryorder ?? await Task.Run(() => garmentUnitDeliveryOrderDataUtil.GetTestDataMultipleItem());

            var garmentUnitExpenditureNote = new GarmentUnitExpenditureNote
            {
                UnitSenderId = garmentUnitDeliveryOrder.UnitSenderId,
                UnitSenderCode = garmentUnitDeliveryOrder.UnitSenderCode,
                UnitSenderName = garmentUnitDeliveryOrder.UnitSenderName,

                UnitRequestId = garmentUnitDeliveryOrder.UnitRequestId,
                UnitRequestCode = garmentUnitDeliveryOrder.UnitRequestCode,
                UnitRequestName = garmentUnitDeliveryOrder.UnitRequestName,

                UnitDOId = garmentUnitDeliveryOrder.Id,
                UnitDONo = garmentUnitDeliveryOrder.UnitDONo,

                StorageId = garmentUnitDeliveryOrder.StorageId,
                StorageCode = garmentUnitDeliveryOrder.StorageCode,
                StorageName = garmentUnitDeliveryOrder.StorageName,

                StorageRequestId = garmentUnitDeliveryOrder.StorageRequestId,
                StorageRequestCode = garmentUnitDeliveryOrder.StorageRequestCode,
                StorageRequestName = garmentUnitDeliveryOrder.StorageRequestName,

                ExpenditureType = "TRANSFER",
                ExpenditureTo = "TRANSFER",
                UENNo = "UENNO12345",

                ExpenditureDate = DateTimeOffset.Now,

                Items = new List<GarmentUnitExpenditureNoteItem>()
            };

            foreach (var item in garmentUnitDeliveryOrder.Items)
            {
                var garmentUnitExpenditureNoteItem = new GarmentUnitExpenditureNoteItem
                {
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = 10,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,
                };
                new GarmentUnitExpenditureNoteItem
                {
                    Id = 0,
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = item.Quantity,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,
                };

                garmentUnitExpenditureNote.Items.Add(garmentUnitExpenditureNoteItem);

            }

            return garmentUnitExpenditureNote;
        }

        public async Task<GarmentUnitExpenditureNote> GetNewDataForPreparing()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var garmentUnitDeliveryOrder = await Task.Run(() => garmentUnitDeliveryOrderDataUtil.GetTestDataMultipleItem());

            var garmentUnitExpenditureNote = new GarmentUnitExpenditureNote
            {
                UnitSenderId = garmentUnitDeliveryOrder.UnitSenderId,
                UnitSenderCode = garmentUnitDeliveryOrder.UnitSenderCode,
                UnitSenderName = garmentUnitDeliveryOrder.UnitSenderName,

                UnitRequestId = garmentUnitDeliveryOrder.UnitRequestId,
                UnitRequestCode = garmentUnitDeliveryOrder.UnitRequestCode,
                UnitRequestName = garmentUnitDeliveryOrder.UnitRequestName,

                UnitDOId = garmentUnitDeliveryOrder.Id,
                UnitDONo = garmentUnitDeliveryOrder.UnitDONo,

                StorageId = garmentUnitDeliveryOrder.StorageId,
                StorageCode = garmentUnitDeliveryOrder.StorageCode,
                StorageName = garmentUnitDeliveryOrder.StorageName,

                StorageRequestId = garmentUnitDeliveryOrder.StorageRequestId,
                StorageRequestCode = garmentUnitDeliveryOrder.StorageRequestCode,
                StorageRequestName = garmentUnitDeliveryOrder.StorageRequestName,

                ExpenditureType = "PROSES",
                ExpenditureTo = "PROSES",
                UENNo = "UENNO12345",

                ExpenditureDate = DateTimeOffset.Now,

                IsPreparing = false,

                Items = new List<GarmentUnitExpenditureNoteItem>()
            };

            foreach (var item in garmentUnitDeliveryOrder.Items)
            {
                var garmentUnitExpenditureNoteItem = new GarmentUnitExpenditureNoteItem
                {
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = "FABRIC",
                    ProductRemark = item.ProductRemark,
                    Quantity = 5,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,
                };
                new GarmentUnitExpenditureNoteItem
                {
                    Id = 0,
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = item.Quantity,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,


                };

                garmentUnitExpenditureNote.Items.Add(garmentUnitExpenditureNoteItem);

            }

            return garmentUnitExpenditureNote;
        }

        public async Task<GarmentUnitExpenditureNote> GetNewData_DOCurrency()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var garmentUnitDeliveryOrder = await Task.Run(() => garmentUnitDeliveryOrderDataUtil.GetTestDataMultipleItem_DOCurrency());

            var garmentUnitExpenditureNote = new GarmentUnitExpenditureNote
            {
                UnitSenderId = garmentUnitDeliveryOrder.UnitSenderId,
                UnitSenderCode = garmentUnitDeliveryOrder.UnitSenderCode,
                UnitSenderName = garmentUnitDeliveryOrder.UnitSenderName,

                UnitRequestId = garmentUnitDeliveryOrder.UnitRequestId,
                UnitRequestCode = garmentUnitDeliveryOrder.UnitRequestCode,
                UnitRequestName = garmentUnitDeliveryOrder.UnitRequestName,

                UnitDOId = garmentUnitDeliveryOrder.Id,
                UnitDONo = garmentUnitDeliveryOrder.UnitDONo,

                StorageId = garmentUnitDeliveryOrder.StorageId,
                StorageCode = garmentUnitDeliveryOrder.StorageCode,
                StorageName = garmentUnitDeliveryOrder.StorageName,

                StorageRequestId = garmentUnitDeliveryOrder.StorageRequestId,
                StorageRequestCode = garmentUnitDeliveryOrder.StorageRequestCode,
                StorageRequestName = garmentUnitDeliveryOrder.StorageRequestName,

                ExpenditureType = "PROSES",
                ExpenditureTo = "PROSES",
                UENNo = "UENNO12345",

                ExpenditureDate = DateTimeOffset.Now,

                IsPreparing = false,

                Items = new List<GarmentUnitExpenditureNoteItem>()
            };

            foreach (var item in garmentUnitDeliveryOrder.Items)
            {
                var garmentUnitExpenditureNoteItem = new GarmentUnitExpenditureNoteItem
                {
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = 5,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,
                };
                new GarmentUnitExpenditureNoteItem
                {
                    Id = 0,
                    IsSave = true,
                    DODetailId = item.DODetailId,

                    EPOItemId = item.EPOItemId,

                    URNItemId = item.URNItemId,
                    UnitDOItemId = item.Id,
                    PRItemId = item.PRItemId,

                    FabricType = item.FabricType,
                    POItemId = item.POItemId,
                    POSerialNumber = item.POSerialNumber,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = item.Quantity,

                    RONo = item.RONo,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,

                    PricePerDealUnit = item.PricePerDealUnit,
                    DOCurrencyRate = item.DOCurrencyRate,
                    Conversion = 1,
                    ReturQuantity = 1,


                };

                garmentUnitExpenditureNote.Items.Add(garmentUnitExpenditureNoteItem);

            }

            return garmentUnitExpenditureNote;
        }

        public void SetDataWithStorage(GarmentUnitExpenditureNote garmentUnitExpenditureNote, long? unitId = null)
        {
            long nowTicks = unitId ?? DateTimeOffset.Now.Ticks;

            garmentUnitExpenditureNote.StorageId = nowTicks;
            garmentUnitExpenditureNote.StorageCode = string.Concat("StorageCode", nowTicks);
            garmentUnitExpenditureNote.StorageName = string.Concat("StorageName", nowTicks);
        }


        public async Task<GarmentUnitExpenditureNote> GetNewDataWithStorage(long? ticks = null)
        {
            var data = await GetNewDataTypeTransfer();
            SetDataWithStorage(data, ticks);

            return data;
        }
        public void SetDataWithStorageRequest(GarmentUnitExpenditureNote garmentUnitExpenditureNote, long? unitId = null)
        {
            long nowTicks = unitId ?? DateTimeOffset.Now.Ticks;

            garmentUnitExpenditureNote.StorageRequestId = nowTicks;
            garmentUnitExpenditureNote.StorageRequestCode = string.Concat("StorageCode", nowTicks);
            garmentUnitExpenditureNote.StorageRequestName = string.Concat("StorageName", nowTicks);
        }


        public async Task<GarmentUnitExpenditureNote> GetNewDataWithStorageRequest(long? ticks = null)
        {
            var data = await GetNewDataTypeTransfer();
            SetDataWithStorageRequest(data, ticks);

            return data;
        }

		public async Task<GarmentUnitExpenditureNote> GetNewDataMonitoringFlow(GarmentUnitDeliveryOrder garmentunitdeliveryorder = null)
		{
			long nowTicks = DateTimeOffset.Now.Ticks;

			var garmentUnitDeliveryOrder = garmentunitdeliveryorder ?? await Task.Run(() => garmentUnitDeliveryOrderDataUtil.GetTestDataMultipleItem());

			var garmentUnitExpenditureNote = new GarmentUnitExpenditureNote
			{
				UnitSenderId = garmentUnitDeliveryOrder.UnitSenderId,
				UnitSenderCode = "SMP1",
				UnitSenderName = garmentUnitDeliveryOrder.UnitSenderName,

				UnitRequestId = garmentUnitDeliveryOrder.UnitRequestId,
				UnitRequestCode = garmentUnitDeliveryOrder.UnitRequestCode,
				UnitRequestName = garmentUnitDeliveryOrder.UnitRequestName,

				UnitDOId = garmentUnitDeliveryOrder.Id,
				UnitDONo = garmentUnitDeliveryOrder.UnitDONo,

				StorageId = garmentUnitDeliveryOrder.StorageId,
				StorageCode = garmentUnitDeliveryOrder.StorageCode,
				StorageName = garmentUnitDeliveryOrder.StorageName,

				StorageRequestId = garmentUnitDeliveryOrder.StorageRequestId,
				StorageRequestCode = garmentUnitDeliveryOrder.StorageRequestCode,
				StorageRequestName = garmentUnitDeliveryOrder.StorageRequestName,

				ExpenditureType = "PROSES",
				ExpenditureTo = "PROSES",
				UENNo = "UENNO12345",

				ExpenditureDate = DateTimeOffset.Now,

				IsPreparing = false,

				Items = new List<GarmentUnitExpenditureNoteItem>()
			};

			foreach (var item in garmentUnitDeliveryOrder.Items)
			{
				var garmentUnitExpenditureNoteItem = new GarmentUnitExpenditureNoteItem
				{
					IsSave = true,
					DODetailId = item.DODetailId,

					EPOItemId = item.EPOItemId,

					URNItemId = item.URNItemId,
					UnitDOItemId = item.Id,
					PRItemId = item.PRItemId,

					FabricType = item.FabricType,
					POItemId = item.POItemId,
					POSerialNumber = item.POSerialNumber,

					ProductId = item.ProductId,
					ProductCode = item.ProductCode,
					ProductName = "FABRIC",
					ProductRemark = item.ProductRemark,
					Quantity = 5,

					RONo = item.RONo,

					UomId = item.UomId,
					UomUnit = item.UomUnit,

					PricePerDealUnit = item.PricePerDealUnit,
					DOCurrencyRate = item.DOCurrencyRate,
					Conversion = 1,
					ReturQuantity = 1,
				};
				new GarmentUnitExpenditureNoteItem
				{
					Id = 0,
					IsSave = true,
					DODetailId = item.DODetailId,

					EPOItemId = item.EPOItemId,

					URNItemId = item.URNItemId,
					UnitDOItemId = item.Id,
					PRItemId = item.PRItemId,

					FabricType = item.FabricType,
					POItemId = item.POItemId,
					POSerialNumber = item.POSerialNumber,

					ProductId = item.ProductId,
					ProductCode = item.ProductCode,
					ProductName = "FABRIC",
					ProductRemark = item.ProductRemark,
					Quantity = item.Quantity,

					RONo = item.RONo,

					UomId = item.UomId,
					UomUnit = item.UomUnit,

					PricePerDealUnit = item.PricePerDealUnit,
					DOCurrencyRate = item.DOCurrencyRate,
					Conversion = 1,
					ReturQuantity = 1,


				};

				garmentUnitExpenditureNote.Items.Add(garmentUnitExpenditureNoteItem);

			}

			return garmentUnitExpenditureNote;
		}
		public async Task<GarmentUnitExpenditureNote> GetTestDataMonitoringFlow()
		{
			var data = await GetNewDataMonitoringFlow();
			await facade.Create(data);
			return data;
		}
		public async Task<GarmentUnitExpenditureNote> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitExpenditureNote> GetTestDataAcc()
        {
            var data = await GetNewDataTypeTransfer();
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitExpenditureNote> GetTestDataSample()
        {
            var data = await GetNewDataTypeTransfer();
            data.ExpenditureType = "SAMPLE";
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitExpenditureNote> GetTestDataWithStorage(long? ticks = null)
        {
            var data = await GetNewDataWithStorage(ticks);
            await facade.Create(data);
            return data;
        }
        public async Task<GarmentUnitExpenditureNote> GetTestDataWithStorageReqeust(long? ticks = null)
        {
            var data = await GetNewDataWithStorageRequest(ticks);
            await facade.Create(data);
            return data;
        }
        public async Task<GarmentUnitExpenditureNote> GetTestDataForPreparing()
        {
            var data = await GetNewDataForPreparing();
            await facade.Create(data);
            return data;
        }
    }
}