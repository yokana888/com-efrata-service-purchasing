using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils
{
    public class GarmentReturnCorrectionNoteDataUtil
    {
        private readonly GarmentReturnCorrectionNoteFacade garmentReturnCorrectionNoteFacade;
        private readonly GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil;

        public GarmentReturnCorrectionNoteDataUtil(GarmentReturnCorrectionNoteFacade garmentReturnCorrectionNoteFacade, GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil)
        {
            this.garmentReturnCorrectionNoteFacade = garmentReturnCorrectionNoteFacade;
            this.garmentDeliveryOrderDataUtil = garmentDeliveryOrderDataUtil;
        }

        public async Task<GarmentCorrectionNote> GetNewData()
        {
            var garmentDeliveryOrder = await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());

            GarmentCorrectionNote garmentCorrectionNote = new GarmentCorrectionNote
            {
                CorrectionNo = "NK1234L",
                CorrectionType = "Retur",
                CorrectionDate = DateTimeOffset.Now,
                DOId = garmentDeliveryOrder.Id,
                DONo = garmentDeliveryOrder.DONo,
                SupplierId = garmentDeliveryOrder.SupplierId,
                SupplierCode = garmentDeliveryOrder.SupplierCode,
                SupplierName = garmentDeliveryOrder.SupplierName,
                Remark = "Remark",
                NKPH = "NKPH1234L",
                NKPN = "NKPN1234L",
                Items = new List<GarmentCorrectionNoteItem>()
            };

            foreach (var item in garmentDeliveryOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    garmentCorrectionNote.Items.Add(
                        new GarmentCorrectionNoteItem
                        {
                            DODetailId = detail.Id,
                            EPOId = item.EPOId,
                            EPONo = item.EPONo,
                            PRId = detail.PRId,
                            PRNo = detail.PRNo,
                            POId = detail.POId,
                            POSerialNumber = detail.POSerialNumber,
                            RONo = detail.RONo,
                            ProductId = detail.ProductId,
                            ProductCode = detail.ProductCode,
                            ProductName = detail.ProductName,
                            Quantity = (decimal)detail.QuantityCorrection,
                            UomId = Convert.ToInt32(detail.UomId),
                            UomIUnit = detail.UomUnit,
                        });
                }
            }

            return garmentCorrectionNote;
        }
        public async Task<GarmentCorrectionNote> GetNewDataWithTax()
        {
            var data = await GetNewData();

            data.UseVat = true;
            data.UseIncomeTax = true;
            data.IncomeTaxId = data.IncomeTaxId;
            data.IncomeTaxName = data.IncomeTaxName;
            data.IncomeTaxRate = data.IncomeTaxRate;

            return data;
        }

        public async Task<GarmentCorrectionNote> GetTestData(string user)
        {
            var data = await GetNewData();
            await garmentReturnCorrectionNoteFacade.Create(data, false, user);
            return data;
        }
    }
}
