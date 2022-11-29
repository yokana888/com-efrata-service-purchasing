using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitExpenditureDataUtils;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils
{
    public class GarmentUnitReceiptNoteDataUtil
    {
        private readonly GarmentUnitReceiptNoteFacade facade;
        private readonly GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil;
        private readonly GarmentUnitExpenditureNoteDataUtil garmentUENDataUtil;

        public GarmentUnitReceiptNoteDataUtil(GarmentUnitReceiptNoteFacade facade, GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil, GarmentUnitExpenditureNoteDataUtil garmentUENDataUtil)
        {
            this.facade = facade;
            this.garmentDeliveryOrderDataUtil = garmentDeliveryOrderDataUtil;
            this.garmentUENDataUtil = garmentUENDataUtil;
        }

        public async Task<GarmentUnitReceiptNote> GetNewData(long? ticks = null, GarmentDeliveryOrder garmentDeliveryOrder=null, GarmentUnitExpenditureNote garmentUnitExpenditureNote=null)
        {
            long nowTicks = ticks ?? DateTimeOffset.Now.Ticks;

            garmentDeliveryOrder = garmentDeliveryOrder ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());
            var garmentUnitReceiptNote = new GarmentUnitReceiptNote
            {
                URNType="PEMBELIAN",
                UnitId = nowTicks,
                UnitCode = string.Concat("UnitCode", nowTicks),
                UnitName = string.Concat("UnitName", nowTicks),

                StorageId = nowTicks,
                StorageCode = string.Concat("StorageCode", nowTicks),
                StorageName = string.Concat("StorageName", nowTicks),

                SupplierId = garmentDeliveryOrder.SupplierId,
                SupplierCode = garmentDeliveryOrder.SupplierCode,
                SupplierName = garmentDeliveryOrder.SupplierName,

                DOId = garmentDeliveryOrder.Id,
                DONo = garmentDeliveryOrder.DONo,

                DeletedReason = nowTicks.ToString(),

                DOCurrencyRate = garmentDeliveryOrder.DOCurrencyRate,

                ReceiptDate = DateTimeOffset.Now,

                Items = new List<GarmentUnitReceiptNoteItem>()
            };

            foreach (var item in garmentDeliveryOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    var garmentUnitReceiptNoteItem = new GarmentUnitReceiptNoteItem
                    {
                        DODetailId = detail.Id,

                        EPOItemId = detail.EPOItemId,
                        DRItemId = string.Concat("drItemId", nowTicks),
                        PRId = detail.PRId,
                        PRNo = detail.PRNo,
                        PRItemId = detail.PRItemId,

                        POId = detail.POId,
                        POItemId = detail.POItemId,
                        POSerialNumber = detail.POSerialNumber,

                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        ProductRemark = detail.ProductRemark,

                        RONo = detail.RONo,

                        ReceiptQuantity =   (decimal)100,

						UomId = long.Parse(detail.UomId),
                        UomUnit = detail.UomUnit,

                        PricePerDealUnit = (decimal)detail.PricePerDealUnit,

                        DesignColor = string.Concat("DesignColor", nowTicks),

                        SmallQuantity = (decimal)detail.SmallQuantity,
						OrderQuantity=30,
                        SmallUomId = long.Parse(detail.SmallUomId),
                        SmallUomUnit = detail.SmallUomUnit,
                        Conversion = (decimal)1,
                        CorrectionConversion = (decimal)12,
						
                        DOCurrencyRate=1,

                        UENItemId=1
                    };
					var garmentUnitReceiptNoteItem2 = new GarmentUnitReceiptNoteItem
					{
						DODetailId = detail.Id,

						EPOItemId = detail.EPOItemId,
						DRItemId = string.Concat("drItemId", nowTicks),
						PRId = detail.PRId,
						PRNo = detail.PRNo,
						PRItemId = detail.PRItemId,

						POId = detail.POId,
						POItemId = detail.POItemId,
						POSerialNumber = detail.POSerialNumber,

						ProductId = detail.ProductId,
						ProductCode = detail.ProductCode,
						ProductName = detail.ProductName,
						ProductRemark = detail.ProductRemark,

						RONo = detail.RONo +"S",

						ReceiptQuantity = (decimal)100,

						UomId = long.Parse(detail.UomId),
						UomUnit = detail.UomUnit,

						PricePerDealUnit = (decimal)detail.PricePerDealUnit,

						DesignColor = string.Concat("DesignColor", nowTicks),

						SmallQuantity = (decimal)detail.SmallQuantity,
						OrderQuantity = 30,
						SmallUomId = long.Parse(detail.SmallUomId),
						SmallUomUnit = detail.SmallUomUnit,
						Conversion = (decimal)1,
						CorrectionConversion = (decimal)12,

						DOCurrencyRate = 1,

                        UENItemId = 1
                    };

					garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem);
					garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem2);
				}
        }

            return garmentUnitReceiptNote;
        }

        public async Task<GarmentUnitReceiptNote> GetNewData2(long? ticks = null, GarmentDeliveryOrder garmentDeliveryOrders = null)
        {
            long nowTicks = ticks ?? DateTimeOffset.Now.Ticks;

            var garmentDeliveryOrder = garmentDeliveryOrders ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData21());

            var garmentUnitReceiptNote = new GarmentUnitReceiptNote
            {
                URNType = "PEMBELIAN",
                UnitId = nowTicks,
                UnitCode = string.Concat("UnitCode", nowTicks),
                UnitName = string.Concat("UnitName", nowTicks),

                SupplierId = garmentDeliveryOrder.SupplierId,
                SupplierCode = garmentDeliveryOrder.SupplierCode,
                SupplierName = garmentDeliveryOrder.SupplierName,

                DOId = garmentDeliveryOrder.Id,
                DONo = garmentDeliveryOrder.DONo,

                DeletedReason = nowTicks.ToString(),

                ReceiptDate = DateTimeOffset.Now,

                Items = new List<GarmentUnitReceiptNoteItem>()
            };

            foreach (var item in garmentDeliveryOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    var garmentUnitReceiptNoteItem = new GarmentUnitReceiptNoteItem
                    {
                        DODetailId = detail.Id,

                        EPOItemId = detail.EPOItemId,
                        DRItemId = string.Concat("drItemId", nowTicks),

                        PRId = detail.PRId,
                        PRNo = detail.PRNo,
                        PRItemId = detail.PRItemId,

                        POId = detail.POId,
                        POItemId = detail.POItemId,
                        POSerialNumber = detail.POSerialNumber,

                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        ProductRemark = detail.ProductRemark,

                        RONo = detail.RONo,

                        ReceiptQuantity = (decimal)detail.ReceiptQuantity,

                        UomId = long.Parse(detail.UomId),
                        UomUnit = detail.UomUnit,

                        PricePerDealUnit = (decimal)detail.PricePerDealUnit,

                        DesignColor = string.Concat("DesignColor", nowTicks),

                        SmallQuantity = (decimal)detail.SmallQuantity,

                        SmallUomId = long.Parse(detail.SmallUomId),
                        SmallUomUnit = detail.SmallUomUnit,
                        Conversion = (decimal)detail.Conversion,
                        CorrectionConversion= (decimal)detail.Conversion,
                        DOCurrencyRate = 1
                    };

                    garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem);
                }
            }

            return garmentUnitReceiptNote;
        }


        public async Task<GarmentUnitReceiptNote> GetNewData3(long? ticks = null, GarmentDeliveryOrder garmentDeliveryOrder = null)
        {
            long nowTicks = ticks ?? DateTimeOffset.Now.Ticks;

            garmentDeliveryOrder = garmentDeliveryOrder ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestDataDO_Currency());

            var garmentUnitReceiptNote = new GarmentUnitReceiptNote
            {
                URNType = "PEMBELIAN",
                UnitId = nowTicks,
                UnitCode = string.Concat("UnitCode", nowTicks),
                UnitName = string.Concat("UnitName", nowTicks),

                SupplierId = garmentDeliveryOrder.SupplierId,
                SupplierCode = garmentDeliveryOrder.SupplierCode,
                SupplierName = garmentDeliveryOrder.SupplierName,

                DOId = garmentDeliveryOrder.Id,
                DONo = garmentDeliveryOrder.DONo,

                DeletedReason = nowTicks.ToString(),

                DOCurrencyRate = garmentDeliveryOrder.DOCurrencyRate,

                ReceiptDate = DateTimeOffset.Now,

                Items = new List<GarmentUnitReceiptNoteItem>()
            };

            foreach (var item in garmentDeliveryOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    var garmentUnitReceiptNoteItem = new GarmentUnitReceiptNoteItem
                    {
                        DODetailId = detail.Id,

                        EPOItemId = detail.EPOItemId,
                        DRItemId = string.Concat("drItemId", nowTicks),
                        PRId = detail.PRId,
                        PRNo = detail.PRNo,
                        PRItemId = detail.PRItemId,

                        POId = detail.POId,
                        POItemId = detail.POItemId,
                        POSerialNumber = detail.POSerialNumber,

                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        ProductRemark = detail.ProductRemark,

                        RONo = detail.RONo,

                        ReceiptQuantity = (decimal)detail.ReceiptQuantity,

                        UomId = long.Parse(detail.UomId),
                        UomUnit = detail.UomUnit,

                        PricePerDealUnit = (decimal)detail.PricePerDealUnit,

                        DesignColor = string.Concat("DesignColor", nowTicks),

                        SmallQuantity = (decimal)detail.SmallQuantity,

                        SmallUomId = long.Parse(detail.SmallUomId),
                        SmallUomUnit = detail.SmallUomUnit,
                        Conversion = (decimal)detail.Conversion,
                        CorrectionConversion = (decimal)detail.Conversion,
                        DOCurrencyRate = 1
                    };

                    garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem);
                }
            }

            return garmentUnitReceiptNote;
        }

        public async Task<GarmentUnitReceiptNote> GetNewDataSubcon(long? ticks = null, GarmentDeliveryOrder garmentDeliveryOrder = null, GarmentUnitExpenditureNote garmentUnitExpenditureNote = null)
        {
            long nowTicks = ticks ?? DateTimeOffset.Now.Ticks;

            garmentDeliveryOrder = garmentDeliveryOrder ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());
            garmentUnitExpenditureNote = garmentUnitExpenditureNote ?? await Task.Run(() => garmentUENDataUtil.GetTestData());
            var garmentUnitReceiptNote = new GarmentUnitReceiptNote
            {
                URNType = "PEMBELIAN",
                UnitId = nowTicks,
                UnitCode = string.Concat("UnitCode", nowTicks),
                UnitName = string.Concat("UnitName", nowTicks),

                StorageId = nowTicks,
                StorageCode = string.Concat("StorageCode", nowTicks),
                StorageName = string.Concat("StorageName", nowTicks),

                SupplierId = garmentDeliveryOrder.SupplierId,
                SupplierCode = garmentDeliveryOrder.SupplierCode,
                SupplierName = garmentDeliveryOrder.SupplierName,

                DOId = garmentDeliveryOrder.Id,
                DONo = garmentDeliveryOrder.DONo,

                DeletedReason = nowTicks.ToString(),

                DOCurrencyRate = garmentDeliveryOrder.DOCurrencyRate,
                UENNo= garmentUnitExpenditureNote.UENNo,
                UENId= garmentUnitExpenditureNote.Id,
                ReceiptDate = DateTimeOffset.Now,

                Items = new List<GarmentUnitReceiptNoteItem>()
            };

            foreach (var item in garmentDeliveryOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    var garmentUnitReceiptNoteItem = new GarmentUnitReceiptNoteItem
                    {
                        DODetailId = detail.Id,

                        EPOItemId = detail.EPOItemId,
                        DRItemId = string.Concat("drItemId", nowTicks),
                        PRId = detail.PRId,
                        PRNo = detail.PRNo,
                        PRItemId = detail.PRItemId,

                        POId = detail.POId,
                        POItemId = detail.POItemId,
                        POSerialNumber = detail.POSerialNumber,

                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        ProductRemark = detail.ProductRemark,

                        RONo = detail.RONo,

                        ReceiptQuantity = (decimal)100,

                        UomId = long.Parse(detail.UomId),
                        UomUnit = detail.UomUnit,

                        PricePerDealUnit = (decimal)detail.PricePerDealUnit,

                        DesignColor = string.Concat("DesignColor", nowTicks),

                        SmallQuantity = (decimal)detail.SmallQuantity,
                        OrderQuantity = 30,
                        SmallUomId = long.Parse(detail.SmallUomId),
                        SmallUomUnit = detail.SmallUomUnit,
                        Conversion = (decimal)1,
                        CorrectionConversion = (decimal)12,

                        DOCurrencyRate = 1,

                        UENItemId = garmentUnitExpenditureNote.Items.First().Id
                    };
                    var garmentUnitReceiptNoteItem2 = new GarmentUnitReceiptNoteItem
                    {
                        DODetailId = detail.Id,

                        EPOItemId = detail.EPOItemId,
                        DRItemId = string.Concat("drItemId", nowTicks),
                        PRId = detail.PRId,
                        PRNo = detail.PRNo,
                        PRItemId = detail.PRItemId,

                        POId = detail.POId,
                        POItemId = detail.POItemId,
                        POSerialNumber = detail.POSerialNumber,

                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        ProductRemark = detail.ProductRemark,

                        RONo = detail.RONo + "S",

                        ReceiptQuantity = (decimal)100,

                        UomId = long.Parse(detail.UomId),
                        UomUnit = detail.UomUnit,

                        PricePerDealUnit = (decimal)detail.PricePerDealUnit,

                        DesignColor = string.Concat("DesignColor", nowTicks),

                        SmallQuantity = (decimal)detail.SmallQuantity,
                        OrderQuantity = 30,
                        SmallUomId = long.Parse(detail.SmallUomId),
                        SmallUomUnit = detail.SmallUomUnit,
                        Conversion = (decimal)1,
                        CorrectionConversion = (decimal)12,

                        DOCurrencyRate = 1,

                        UENItemId = garmentUnitExpenditureNote.Items.First().Id
                    };

                    garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem);
                    garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem2);
                }
            }

            return garmentUnitReceiptNote;
        }
		public async Task<GarmentUnitReceiptNote> GetNewDataMonitoring(long? ticks = null, GarmentDeliveryOrder garmentDeliveryOrder = null, GarmentUnitExpenditureNote garmentUnitExpenditureNote = null)
		{
			long nowTicks = ticks ?? DateTimeOffset.Now.Ticks;

			garmentDeliveryOrder = garmentDeliveryOrder ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());
			var garmentUnitReceiptNote = new GarmentUnitReceiptNote
			{
				URNType = "PEMBELIAN",
				UnitId = nowTicks,
				UnitCode = string.Concat("UnitCode", nowTicks),
				UnitName = string.Concat("UnitName", nowTicks),

				StorageId = nowTicks,
				StorageCode = string.Concat("StorageCode", nowTicks),
				StorageName = string.Concat("StorageName", nowTicks),

				SupplierId = garmentDeliveryOrder.SupplierId,
				SupplierCode = garmentDeliveryOrder.SupplierCode,
				SupplierName = garmentDeliveryOrder.SupplierName,

				DOId = garmentDeliveryOrder.Id,
				DONo = garmentDeliveryOrder.DONo,

				DeletedReason = nowTicks.ToString(),

				DOCurrencyRate = garmentDeliveryOrder.DOCurrencyRate,

				ReceiptDate = DateTimeOffset.Now,

				Items = new List<GarmentUnitReceiptNoteItem>()
			};

			foreach (var item in garmentDeliveryOrder.Items)
			{
				foreach (var detail in item.Details)
				{
					var garmentUnitReceiptNoteItem = new GarmentUnitReceiptNoteItem
					{
						DODetailId = detail.Id,

						EPOItemId = detail.EPOItemId,
						DRItemId = string.Concat("drItemId", nowTicks),
						PRId = detail.PRId,
						PRNo = detail.PRNo,
						PRItemId = detail.PRItemId,

						POId = detail.POId,
						POItemId = detail.POItemId,
						POSerialNumber = detail.POSerialNumber,

						ProductId = detail.ProductId,
						ProductCode = detail.ProductCode,
						ProductName = "FABRIC",
						ProductRemark = detail.ProductRemark,

						RONo = detail.RONo,

						ReceiptQuantity = (decimal)100,

						UomId = long.Parse(detail.UomId),
						UomUnit = detail.UomUnit,

						PricePerDealUnit = (decimal)detail.PricePerDealUnit,

						DesignColor = string.Concat("DesignColor", nowTicks),

						SmallQuantity = (decimal)detail.SmallQuantity,
						OrderQuantity = 30,
						SmallUomId = long.Parse(detail.SmallUomId),
						SmallUomUnit = detail.SmallUomUnit,
						Conversion = (decimal)1,
						CorrectionConversion = (decimal)12,

						DOCurrencyRate = 1,

						UENItemId = 1
					};
					var garmentUnitReceiptNoteItem2 = new GarmentUnitReceiptNoteItem
					{
						DODetailId = detail.Id,

						EPOItemId = detail.EPOItemId,
						DRItemId = string.Concat("drItemId", nowTicks),
						PRId = detail.PRId,
						PRNo = detail.PRNo,
						PRItemId = detail.PRItemId,

						POId = detail.POId,
						POItemId = detail.POItemId,
						POSerialNumber = detail.POSerialNumber,

						ProductId = detail.ProductId,
						ProductCode = detail.ProductCode,
						ProductName = detail.ProductName,
						ProductRemark = detail.ProductRemark,

						RONo = detail.RONo + "S",

						ReceiptQuantity = (decimal)100,

						UomId = long.Parse(detail.UomId),
						UomUnit = detail.UomUnit,

						PricePerDealUnit = (decimal)detail.PricePerDealUnit,

						DesignColor = string.Concat("DesignColor", nowTicks),

						SmallQuantity = (decimal)detail.SmallQuantity,
						OrderQuantity = 30,
						SmallUomId = long.Parse(detail.SmallUomId),
						SmallUomUnit = detail.SmallUomUnit,
						Conversion = (decimal)1,
						CorrectionConversion = (decimal)12,

						DOCurrencyRate = 1,

						UENItemId = 1
					};

					garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem);
					garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem2);
				}
			}

			return garmentUnitReceiptNote;
		}
		public async Task<GarmentUnitReceiptNote> GetNewDataMonitoringFlow(long? ticks = null, GarmentDeliveryOrder garmentDeliveryOrder = null, GarmentUnitExpenditureNote garmentUnitExpenditureNote = null)
		{
			long nowTicks = ticks ?? DateTimeOffset.Now.Ticks;

			garmentDeliveryOrder = garmentDeliveryOrder ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());
			var garmentUnitReceiptNote = new GarmentUnitReceiptNote
			{
				URNType = "PEMBELIAN",
				UnitId = nowTicks,
				UnitCode = "SMP1",
				UnitName = string.Concat("UnitName", nowTicks),

				StorageId = nowTicks,
				StorageCode = string.Concat("StorageCode", nowTicks),
				StorageName = string.Concat("StorageName", nowTicks),

				SupplierId = garmentDeliveryOrder.SupplierId,
				SupplierCode = "SupplierCode",
				SupplierName = "SupplierName",

				DOId = garmentDeliveryOrder.Id,
				DONo = "DONo",

				DeletedReason = nowTicks.ToString(),

				DOCurrencyRate = garmentDeliveryOrder.DOCurrencyRate,

				ReceiptDate = DateTimeOffset.Now,
				URNNo="URNNo",

				Items = new List<GarmentUnitReceiptNoteItem>()
			};

			foreach (var item in garmentDeliveryOrder.Items)
			{
				foreach (var detail in item.Details)
				{
					var garmentUnitReceiptNoteItem = new GarmentUnitReceiptNoteItem
					{
						DODetailId = detail.Id,

						EPOItemId = detail.EPOItemId,
						DRItemId = string.Concat("drItemId", nowTicks),
						PRId = detail.PRId,
						PRNo = "PRNo",
						PRItemId = detail.PRItemId,

						POId = detail.POId,
						POItemId = detail.POItemId,
						POSerialNumber = "POSerialNumber",

						ProductId = detail.ProductId,
						ProductCode = detail.ProductCode,
						ProductName = "FABRIC",
						ProductRemark = detail.ProductRemark,

						RONo = "RONo",

						ReceiptQuantity = (decimal)100,

						UomId = long.Parse(detail.UomId),
						UomUnit = detail.UomUnit,

						PricePerDealUnit = (decimal)detail.PricePerDealUnit,

						DesignColor = string.Concat("DesignColor", nowTicks),

						SmallQuantity = (decimal)detail.SmallQuantity,
						OrderQuantity = 30,
						SmallUomId = long.Parse(detail.SmallUomId),
						SmallUomUnit = detail.SmallUomUnit,
						Conversion = (decimal)1,
						CorrectionConversion = (decimal)12,

						DOCurrencyRate = 1,

						UENItemId = 1
					};
					var garmentUnitReceiptNoteItem2 = new GarmentUnitReceiptNoteItem
					{
						DODetailId = detail.Id,

						EPOItemId = detail.EPOItemId,
						DRItemId = string.Concat("drItemId", nowTicks),
						PRId = detail.PRId,
						PRNo = detail.PRNo,
						PRItemId = detail.PRItemId,

						POId = detail.POId,
						POItemId = detail.POItemId,
						POSerialNumber = detail.POSerialNumber,

						ProductId = detail.ProductId,
						ProductCode = detail.ProductCode,
						ProductName = detail.ProductName,
						ProductRemark = detail.ProductRemark,

						RONo = detail.RONo + "S",

						ReceiptQuantity = (decimal)100,

						UomId = long.Parse(detail.UomId),
						UomUnit = detail.UomUnit,

						PricePerDealUnit = (decimal)detail.PricePerDealUnit,

						DesignColor = string.Concat("DesignColor", nowTicks),

						SmallQuantity = (decimal)detail.SmallQuantity,
						OrderQuantity = 30,
						SmallUomId = long.Parse(detail.SmallUomId),
						SmallUomUnit = detail.SmallUomUnit,
						Conversion = (decimal)1,
						CorrectionConversion = (decimal)12,

						DOCurrencyRate = 1,

						UENItemId = 1
					};

					garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem);
					garmentUnitReceiptNote.Items.Add(garmentUnitReceiptNoteItem2);
				}
			}

			return garmentUnitReceiptNote;
		}

		public void SetDataWithStorage(GarmentUnitReceiptNote garmentUnitReceiptNote, long? unitId = null)
        {
            long nowTicks = unitId ?? DateTimeOffset.Now.Ticks;

            garmentUnitReceiptNote.IsStorage = true;
            garmentUnitReceiptNote.StorageId = nowTicks;
            garmentUnitReceiptNote.StorageCode = string.Concat("StorageCode", nowTicks);
            garmentUnitReceiptNote.StorageName = string.Concat("StorageName", nowTicks);
        }


        public async Task<GarmentUnitReceiptNote> GetNewDataWithStorage(long? ticks = null)
        {
            var data = await GetNewData(ticks);
            SetDataWithStorage(data, data.UnitId);

            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetNewDataWithStorage2(long? ticks = null)
        {
            var data = await GetNewData2(ticks);
            SetDataWithStorage(data, data.UnitId);

            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetNewDataWithStorage3(long? ticks = null)
        {
            var data = await GetNewData3(ticks);
            SetDataWithStorage(data, data.UnitId);

            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetTestData(long? ticks = null)
        {
            var data = await GetNewData(ticks);
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetTestData(GarmentDeliveryOrder garmentDeliveryOrder, long? ticks = null)
        {
            var data = await GetNewData(ticks,garmentDeliveryOrder);
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetTestDataWithStorage(long? ticks = null)
        {
            var data = await GetNewDataWithStorage(ticks);
            await facade.Create(data);
            return data;
        }
        public async Task<GarmentUnitReceiptNote> GetNewDataWithStorageSubcon(long? ticks = null)
        {
            var data = await GetNewDataSubcon(ticks);
            SetDataWithStorage(data, data.UnitId);

            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetTestDataWithStorageSubcon(long? ticks = null)
        {
            var data = await GetNewDataSubcon(ticks);
            data.URNType = "SISA SUBCON";
            await facade.Create(data);
            return data;
        }
		public async Task<GarmentUnitReceiptNote> GetTestDataMonitoringFlow(long? ticks = null)
		{
			var data = await GetNewDataMonitoringFlow(ticks);
			await facade.Create(data);
			return data;
		}
		public async Task<GarmentUnitReceiptNote> GetTestDataWithStorageGudangSisaACC(long? ticks = null)
        {
            var data = await GetNewDataWithStorage(ticks);
            data.URNType = "GUDANG SISA";
            data.ExpenditureId = 1;
            data.ExpenditureNo = "no";
            data.Category = "ACCESSORIES";
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetTestDataWithStorageGudangSisaFabric(long? ticks = null)
        {
            var data = await GetNewDataWithStorage(ticks);
            data.URNType = "GUDANG SISA";
            data.ExpenditureId = 1;
            data.ExpenditureNo = "no";
            data.Category = "FABRIC";
            await facade.Create(data);
            return data;
        }

        public GarmentDOItems ReadDOItemsByURNItemId(int id)
        {
            return facade.ReadDOItemsByURNItemId(id);
        }

        public async Task<GarmentUnitReceiptNote> GetTestDataWithStorage2(long? ticks = null)
        {
            var data = await GetNewDataWithStorage2(ticks);
            await facade.Create(data);
            return data;
        }

        public async Task<GarmentUnitReceiptNote> GetTestDataWithStorage_DOCurrency(long? ticks = null)
        {
            var data = await GetNewDataWithStorage3(ticks);
            await facade.Create(data);
            return data;
        }
    }
}
