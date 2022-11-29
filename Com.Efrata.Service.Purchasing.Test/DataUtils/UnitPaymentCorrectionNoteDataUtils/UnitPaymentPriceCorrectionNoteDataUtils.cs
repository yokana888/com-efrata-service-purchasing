using Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils
{
    public class UnitPaymentPriceCorrectionNoteDataUtils
    {
        private UnitPaymentOrderDataUtil unitPaymentOrderDataUtil;
        private readonly UnitPaymentPriceCorrectionNoteFacade facade;

        public UnitPaymentPriceCorrectionNoteDataUtils(UnitPaymentOrderDataUtil unitPaymentOrderDataUtil, UnitPaymentPriceCorrectionNoteFacade facade)
        {
            this.unitPaymentOrderDataUtil = unitPaymentOrderDataUtil;
            this.facade = facade;
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

                UPONo = unitPaymentOrder.UPONo,

                CorrectionDate = new DateTimeOffset(),

                CorrectionType = "Harga Total",

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

        public async Task<UnitPaymentCorrectionNote> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data, true, "Unit Test", 7);
            return data;
        }
    }
}
