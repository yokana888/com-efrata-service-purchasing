using Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils
{
    public class UnitPaymentCorrectionNoteDataUtil
    {
        private UnitPaymentOrderDataUtil unitPaymentOrderDataUtil;
        private readonly UnitPaymentQuantityCorrectionNoteFacade facade;
        private readonly UnitReceiptNoteFacade UrnFacade;

        public UnitPaymentCorrectionNoteDataUtil(UnitPaymentOrderDataUtil unitPaymentOrderDataUtil, UnitPaymentQuantityCorrectionNoteFacade facade, UnitReceiptNoteFacade UrnFacade)
        {
            this.unitPaymentOrderDataUtil = unitPaymentOrderDataUtil;
            this.facade = facade;
            this.UrnFacade = UrnFacade;
        }

        public async Task<UnitPaymentCorrectionNote> GetNewData()
        {
            Lib.Models.UnitPaymentOrderModel.UnitPaymentOrder unitPaymentOrder = await Task.Run(() => this.unitPaymentOrderDataUtil.GetTestData());

            List<UnitPaymentCorrectionNoteItem> unitPaymentCorrectionNoteItem = new List<UnitPaymentCorrectionNoteItem>();
            foreach (var item in unitPaymentOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    unitPaymentCorrectionNoteItem.Add(new UnitPaymentCorrectionNoteItem
                    {
                        UPODetailId = detail.Id,
                        URNNo = item.URNNo,
                        EPONo = detail.EPONo,
                        PRId = detail.PRId,
                        PRNo = detail.PRNo,
                        PRDetailId = detail.PRItemId,
                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        UomId = detail.UomId,
                        UomUnit = detail.UomUnit,
                        PricePerDealUnitBefore = (double)detail.PricePerDealUnitCorrection,
                        PriceTotalBefore = (double)detail.PriceTotalCorrection
                    });
                }
            }

            UnitPaymentCorrectionNote unitPaymentCorrectionNote = new UnitPaymentCorrectionNote
            {
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",

                SupplierId = "1",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",

                UPCNo = "18-06-G-NKI-001",
                UPOId = unitPaymentOrder.Id,

                UPONo = "18-08-BPL-P1A-003",

                CorrectionDate = new DateTimeOffset(),

                CorrectionType = "Jumlah",

                InvoiceCorrectionDate = new DateTimeOffset(),
                InvoiceCorrectionNo = "123456",

                useVat = true,
                VatTaxCorrectionDate = new DateTimeOffset(),
                VatTaxCorrectionNo = null,

                useIncomeTax = true,
                IncomeTaxCorrectionDate = new DateTimeOffset(),
                IncomeTaxCorrectionNo = null,

                ReleaseOrderNoteNo = "123456",
                ReturNoteNo = "",

                CategoryId = "CategoryId ",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",

                Remark = null,

                DueDate = new DateTimeOffset(), // ???

                Items = unitPaymentCorrectionNoteItem
            };
            return unitPaymentCorrectionNote;
        }

        public Lib.Models.UnitReceiptNoteModel.UnitReceiptNote GetNewDataUrn()
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote
            {
                URNNo = "18-08-BPL-P1A-003",
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                UnitId = "UnitId",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                SupplierId = "SupplierId",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",
                DOId = 1,
                DONo = "SJTURI-2",
                ReceiptDate = new DateTimeOffset(),
                IsStorage = false,
                StorageId = "SotrageId",
                StorageCode = "StorageCode",
                StorageName = "StorageName",
                Remark = "Remark",
            };
            return unitReceiptNote;
        }
        public Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem GetNewDataUrnItem() => new Lib.Models.UnitReceiptNoteModel.UnitReceiptNoteItem
        {
            EPODetailId = 1,
            DODetailId = 1,
            PRNo = "PRNo",
            PRId = 1,
            PRItemId = 1,
            ProductId = "ProductId",
            ProductCode = "ProductCode",
            ProductName = "ProductName",
            ReceiptQuantity = 10,
            PricePerDealUnit = 10,
            IsCorrection = true,
            UomId = "UomId",
            Uom = "Uom",
            URNId = 1,
            ProductRemark = "Remark"
        };



        public async Task<UnitPaymentCorrectionNote> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data, "Unit Test", 7);
            return data;
        }

        public async Task<Lib.Models.UnitReceiptNoteModel.UnitReceiptNote> GetTestDataUrn()
        {
            var data = GetNewDataUrn();
            await UrnFacade.Create(data, "Unit Test");
            return data;
        }
    }
}
