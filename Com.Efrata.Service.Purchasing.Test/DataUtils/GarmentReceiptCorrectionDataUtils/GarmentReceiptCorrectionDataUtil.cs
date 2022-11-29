using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReceiptCorrectionFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentReceiptCorrectionDataUtils
{
    public class GarmentReceiptCorrectionDataUtil
    {
        private readonly GarmentReceiptCorrectionFacade garmentReceiptCorrectionFacade;
        private readonly GarmentUnitReceiptNoteDataUtil garmentUnitReceiptNoteDataUtil;

        public GarmentReceiptCorrectionDataUtil(GarmentReceiptCorrectionFacade garmentReceiptCorrectionFacade, GarmentUnitReceiptNoteDataUtil garmentUnitReceiptNoteDataUtil)
        {
            this.garmentReceiptCorrectionFacade = garmentReceiptCorrectionFacade;
            this.garmentUnitReceiptNoteDataUtil = garmentUnitReceiptNoteDataUtil;
        }

        public async Task<(GarmentReceiptCorrection GarmentReceiptCorrection, GarmentUnitReceiptNote GarmentUnitReceiptNote)> GetNewData(GarmentUnitReceiptNote unitReceiptNote = null)
        {
            var garmentUnitReceiptNote = unitReceiptNote ?? await Task.Run(() => garmentUnitReceiptNoteDataUtil.GetTestData());

            GarmentReceiptCorrection garmentReceiptCorrection = new GarmentReceiptCorrection
            {
                CorrectionDate = DateTimeOffset.Now,
                UnitCode=garmentUnitReceiptNote.UnitCode,
                UnitId= garmentUnitReceiptNote.UnitId,
                UnitName= garmentUnitReceiptNote.UnitName,
                URNId= garmentUnitReceiptNote.Id,
                URNNo= garmentUnitReceiptNote.URNNo,
                StorageCode= garmentUnitReceiptNote.StorageCode,
                StorageId= garmentUnitReceiptNote.StorageId,
                StorageName= garmentUnitReceiptNote.StorageName,
                Remark = "Remark",
                Items = new List<GarmentReceiptCorrectionItem>()
            };

            foreach (var detail in garmentUnitReceiptNote.Items)
            {
                
                    garmentReceiptCorrection.Items.Add(
                        new GarmentReceiptCorrectionItem
                        {
                            DODetailId = detail.Id,
                            EPOItemId=detail.EPOItemId,
                            POItemId=detail.POItemId,
                            PricePerDealUnit= (double)detail.PricePerDealUnit,
                            PRItemId=detail.PRItemId,
                            Conversion =(double)detail.Conversion,
                            POSerialNumber = detail.POSerialNumber,
                            RONo = detail.RONo,
                            ProductId = detail.ProductId,
                            ProductCode = detail.ProductCode,
                            ProductName = detail.ProductName,
                            Quantity = (double)detail.ReceiptCorrection,
                            UomId = Convert.ToInt32(detail.UomId),
                            UomUnit = detail.UomUnit,
                            DesignColor=detail.DesignColor,
                            ProductRemark= detail.ProductRemark,
                            SmallQuantity= (double)detail.SmallQuantity,
                            SmallUomId= detail.SmallUomId,
                            SmallUomUnit=detail.SmallUomUnit,
                            URNItemId= detail.Id
                        });
                
            }

            return (garmentReceiptCorrection, garmentUnitReceiptNote);
        }

        public async Task<GarmentReceiptCorrection> GetNewDataKoreksiJumlahPlus()
        {
            var data = await GetNewData();

            data.GarmentReceiptCorrection.CorrectionType = "Jumlah";

            foreach (var detail in data.GarmentUnitReceiptNote.Items)
            {
                var garmentReceiptCorrectionItem = data.GarmentReceiptCorrection.Items.First(i => i.URNItemId == detail.Id);
                garmentReceiptCorrectionItem.CorrectionConversion = 0;
                garmentReceiptCorrectionItem.CorrectionQuantity = (double)detail.ReceiptQuantity + 1;
            }

            return data.GarmentReceiptCorrection;
        }

        public async Task<GarmentReceiptCorrection> GetNewDataKoreksiJumlahMinus()
        {
            var data = await GetNewData();

            data.GarmentReceiptCorrection.CorrectionType = "Jumlah";

            foreach (var detail in data.GarmentUnitReceiptNote.Items)
            {
                var garmentReceiptCorrectionItem = data.GarmentReceiptCorrection.Items.First(i => i.URNItemId == detail.Id);
                garmentReceiptCorrectionItem.CorrectionConversion = 0;
                garmentReceiptCorrectionItem.CorrectionQuantity = (double)detail.ReceiptQuantity-((double)detail.ReceiptQuantity + 2);
            }

            return data.GarmentReceiptCorrection;
        }

        public async Task<GarmentReceiptCorrection> GetNewDataKoreksiKonversi()
        {
            var data = await GetNewData();

            data.GarmentReceiptCorrection.CorrectionType = "Konversi";

            foreach (var detail in data.GarmentUnitReceiptNote.Items)
            {
                var garmentReceiptCorrectionItem = data.GarmentReceiptCorrection.Items.First(i => i.URNItemId == detail.Id);
                garmentReceiptCorrectionItem.CorrectionConversion =(double) detail.Conversion+1;
                garmentReceiptCorrectionItem.CorrectionQuantity = 0;
            }

            return data.GarmentReceiptCorrection;
        }

        public async Task<GarmentReceiptCorrection> GetTestDataKoreksiJumlahPlus()
        {
            var data = await GetNewDataKoreksiJumlahPlus();
            await garmentReceiptCorrectionFacade.Create(data,"unit-test");
            return data;
        }

        public async Task<GarmentReceiptCorrection> GetTestDataKoreksiJumlahMinus()
        {
            var data = await GetNewDataKoreksiJumlahMinus();
            await garmentReceiptCorrectionFacade.Create(data, "unit-test");
            return data;
        }

        public async Task<GarmentReceiptCorrection> GetTestDataKoreksiKonversi()
        {
            var data = await GetNewDataKoreksiKonversi();
            await garmentReceiptCorrectionFacade.Create(data, "unit-test");
            return data;
        }

        public async Task<GarmentReceiptCorrection> GetTestData(GarmentUnitReceiptNote unit)
        {
            var data = await GetNewData(unit);
            await garmentReceiptCorrectionFacade.Create(data.GarmentReceiptCorrection, "unit-test");
            return data.GarmentReceiptCorrection;
        }
    }
}
