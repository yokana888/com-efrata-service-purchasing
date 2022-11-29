using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils
{
    public class GarmentCorrectionNoteQuantityDataUtil
    {
        private readonly GarmentCorrectionNoteQuantityFacade garmentCorrectionNoteQuantityFacade;
        private readonly GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil;

        public GarmentCorrectionNoteQuantityDataUtil(GarmentCorrectionNoteQuantityFacade garmentCorrectionNoteFacade, GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil)
        {
            this.garmentCorrectionNoteQuantityFacade = garmentCorrectionNoteFacade;
            this.garmentDeliveryOrderDataUtil = garmentDeliveryOrderDataUtil;
        }

        public async Task<GarmentCorrectionNote> GetNewData(GarmentDeliveryOrder garmentDO = null)
        {
            var garmentDeliveryOrder = garmentDO ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());

            GarmentCorrectionNote garmentCorrectionNote = new GarmentCorrectionNote
            {
                CorrectionNo = "NK1234L",
                CorrectionType = "Jumlah",
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
		public async Task<List<GarmentCorrectionNote>> GetNewDoubleCorrectionData(string user)
		{
			var garmentDeliveryOrder = await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());

			GarmentCorrectionNote garmentCorrectionNote = new GarmentCorrectionNote
			{
				CorrectionNo = "NK1234L",
				CorrectionType = "Jumlah",
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
			GarmentCorrectionNote garmentCorrectionNotes = new GarmentCorrectionNote
			{
				CorrectionNo = "NK1234L",
				CorrectionType = "HARGa total",
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
					garmentCorrectionNotes.Items.Add(
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
			var data1=await garmentCorrectionNoteQuantityFacade.Create(garmentCorrectionNote, false, user,7);
			var data2=await garmentCorrectionNoteQuantityFacade.Create(garmentCorrectionNotes, false, user, 7);
			List<GarmentCorrectionNote> lisdata = new List<GarmentCorrectionNote>();
			lisdata.Add(garmentCorrectionNote);
			lisdata.Add(garmentCorrectionNotes);
			return lisdata;

			//return garmentCorrectionNote;
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
            await garmentCorrectionNoteQuantityFacade.Create(data,false, user);
            return data;
        }
    }
}
