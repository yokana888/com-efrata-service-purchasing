using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils
{
    public class UnitReceiptNoteDataUtil
    {
        private UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil;
        private DeliveryOrderDataUtil deliveryOrderDataUtil;
        private readonly UnitReceiptNoteFacade facade;

        public UnitReceiptNoteDataUtil(UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil, UnitReceiptNoteFacade facade, DeliveryOrderDataUtil deliveryOrderDataUtil)
        {
            this.unitReceiptNoteItemDataUtil = unitReceiptNoteItemDataUtil;
            this.deliveryOrderDataUtil = deliveryOrderDataUtil;
            this.facade = facade;
        }

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetNewData(string user)
        {
            DeliveryOrder deliveryOrder = await deliveryOrderDataUtil.GetTestData(user);
            List<DeliveryOrderItem> doItem = new List<DeliveryOrderItem> (deliveryOrder.Items);
            List<DeliveryOrderDetail> doDetail = new List<DeliveryOrderDetail>(doItem[0].Details);
            

            doItem[0].Details = doDetail;
            return new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote
            {
                UnitId = doDetail[0].UnitId,
                UnitCode = doDetail[0].UnitCode,
                UnitName = "UnitName",
                Remark = "Test URN",
                SupplierCode = deliveryOrder.SupplierCode,
                SupplierId = deliveryOrder.SupplierId,
                SupplierName = deliveryOrder.SupplierName,
                ReceiptDate = DateTimeOffset.UtcNow,
                DOId=deliveryOrder.Id,
                DONo=deliveryOrder.DONo,
                Items = new List<Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem> { unitReceiptNoteItemDataUtil.GetNewData(doItem) }
            };
        }

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetNewHavingStockData(string user)
        {
            DeliveryOrder deliveryOrder = await deliveryOrderDataUtil.GetTestHavingStockData(user);
            List<DeliveryOrderItem> doItem = new List<DeliveryOrderItem>(deliveryOrder.Items);
            List<DeliveryOrderDetail> doDetail = new List<DeliveryOrderDetail>(doItem[0].Details);


            doItem[0].Details = doDetail;
            return new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote
            {
                UnitId = doDetail[0].UnitId,
                UnitCode = doDetail[0].UnitCode,
                UnitName = "UnitName",
                Remark = "Test URN",
                SupplierCode = deliveryOrder.SupplierCode,
                SupplierId = deliveryOrder.SupplierId,
                SupplierName = deliveryOrder.SupplierName,
                ReceiptDate = DateTimeOffset.UtcNow,
                DOId = deliveryOrder.Id,
                DONo = deliveryOrder.DONo,
                Items = new List<Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem> { unitReceiptNoteItemDataUtil.GetNewData(doItem) }
            };
        }

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetNewDatas(string user)
		{
			DeliveryOrder deliveryOrder = await deliveryOrderDataUtil.GetTestData(user);
			List<DeliveryOrderItem> doItem = new List<DeliveryOrderItem>(deliveryOrder.Items);
			List<DeliveryOrderDetail> doDetail = new List<DeliveryOrderDetail>(doItem[0].Details);
            var dt = DateTime.Now;
            dt = dt.Date;

			doItem[0].Details = doDetail;
			return new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote
			{
				UnitId = doDetail[0].UnitId,
				UnitCode = doDetail[0].UnitCode,
				UnitName = "UnitName",
				Remark = "Test URN",
				URNNo= "BPI-001",
				SupplierCode = deliveryOrder.SupplierCode,
				SupplierId = deliveryOrder.SupplierId,
				SupplierName = deliveryOrder.SupplierName,
                ReceiptDate = new DateTimeOffset(new DateTime(dt.Ticks), TimeSpan.Zero),
                DOId = deliveryOrder.Id,
				DONo = deliveryOrder.DONo,
				Items = new List<Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem> { unitReceiptNoteItemDataUtil.GetNewData(doItem) }
			};
		}

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetNewDataLocalSupplier(string user)
        {
            DeliveryOrder deliveryOrder = await deliveryOrderDataUtil.GetTestData(user);
            List<DeliveryOrderItem> doItem = new List<DeliveryOrderItem>(deliveryOrder.Items);
            List<DeliveryOrderDetail> doDetail = new List<DeliveryOrderDetail>(doItem[0].Details);
            var dt = DateTime.Now;
            dt = dt.Date;

            doItem[0].Details = doDetail;
            return new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote
            {
                UnitId = doDetail[0].UnitId,
                UnitCode = doDetail[0].UnitCode,
                UnitName = "UnitName",
                Remark = "Test URN",
                URNNo = "BPL-001",
                SupplierCode = deliveryOrder.SupplierCode,
                SupplierId = deliveryOrder.SupplierId,
                SupplierName = deliveryOrder.SupplierName,
                ReceiptDate = new DateTimeOffset(new DateTime(dt.Ticks), TimeSpan.Zero),
                DOId = deliveryOrder.Id,
                DONo = deliveryOrder.DONo,
                Items = new List<Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem> { unitReceiptNoteItemDataUtil.GetNewData(doItem) }
            };
        }

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetNewDataLocalSupplierValas(string user)
        {
            DeliveryOrder deliveryOrder = await deliveryOrderDataUtil.GetTestDataValas(user);
            List<DeliveryOrderItem> doItem = new List<DeliveryOrderItem>(deliveryOrder.Items);
            List<DeliveryOrderDetail> doDetail = new List<DeliveryOrderDetail>(doItem[0].Details);
            var dt = DateTime.Now;
            dt = dt.Date;

            doItem[0].Details = doDetail;
            return new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote
            {
                UnitId = doDetail[0].UnitId,
                UnitCode = doDetail[0].UnitCode,
                UnitName = "UnitName",
                Remark = "Test URN",
                URNNo = "BPL-001",
                SupplierCode = deliveryOrder.SupplierCode,
                SupplierId = deliveryOrder.SupplierId,
                SupplierName = deliveryOrder.SupplierName,
                ReceiptDate = new DateTimeOffset(new DateTime(dt.Ticks), TimeSpan.Zero),
                DOId = deliveryOrder.Id,
                DONo = deliveryOrder.DONo,
                Items = new List<Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem> { unitReceiptNoteItemDataUtil.GetNewData(doItem) }
            };
        }

        public async Task<UnitReceiptNoteViewModel> GetNewDataViewModel(string user)
        {
            DeliveryOrder deliveryOrder = await deliveryOrderDataUtil.GetTestData(user);
            List<DeliveryOrderItem> doItem = new List<DeliveryOrderItem>(deliveryOrder.Items);
            return new UnitReceiptNoteViewModel
            {
                date=DateTimeOffset.Now,
                supplier = new SupplierViewModel
                {
                    _id = deliveryOrder.SupplierId,
                    code = deliveryOrder.SupplierCode,
                    name = deliveryOrder.SupplierName,
                    import=true
                },
                unit = new UnitViewModel
                {
                    _id = "UnitId",
                    code = "UnitCode",
                    name = "UnitName",
                    division = new DivisionViewModel
                    {
                        _id = "DivisionId",
                        code = "DivisionCode",
                        name = "DivisionName",
                    }
                },
                doNo=deliveryOrder.DONo,
                remark = "test",
                items = new List<UnitReceiptNoteItemViewModel> { unitReceiptNoteItemDataUtil.GetNewDataViewModel(doItem) }
            };
        }
        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetTestData(string user)
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await GetNewData(user);

            await facade.Create(unitReceiptNote, user);

            return unitReceiptNote;
        }
       

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetTestDataLocalSupplier(string user)
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await GetNewDataLocalSupplier(user);

            unitReceiptNote.SupplierIsImport = false;
            await facade.Create(unitReceiptNote, user);

            return unitReceiptNote;
        }

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetTestDataImportSupplier(string user)
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await GetNewDataLocalSupplier(user);

            unitReceiptNote.SupplierIsImport = true;
            await facade.Create(unitReceiptNote, user);

            return unitReceiptNote;
        }

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetTestDataImportSupplierValas(string user)
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await GetNewDataLocalSupplierValas(user);

            unitReceiptNote.SupplierIsImport = true;
            await facade.Create(unitReceiptNote, user);

            return unitReceiptNote;
        }
    }
}
