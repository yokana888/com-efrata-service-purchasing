using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentBeacukaiFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentBeacukaiDataUtils
{
	public class GarmentBeacukaiDataUtil
	{
		private GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil;
        private readonly GarmentBeacukaiFacade facade;

        public GarmentUnitReceiptNoteDataUtil garmentUnitReceiptNoteDataUtil;
        
        public GarmentBeacukaiDataUtil( GarmentDeliveryOrderDataUtil GarmentDeliveryOrderDataUtil, GarmentBeacukaiFacade facade)
		{
			this.garmentDeliveryOrderDataUtil = GarmentDeliveryOrderDataUtil;
			this.facade = facade;
		}
		public GarmentBeacukaiDataUtil(GarmentDeliveryOrderDataUtil GarmentDeliveryOrderDataUtil, GarmentUnitReceiptNoteDataUtil GarmentUnitReceiptNoteDataUtil, GarmentBeacukaiFacade facade)
		{
            this.garmentDeliveryOrderDataUtil = GarmentDeliveryOrderDataUtil;
            this.garmentUnitReceiptNoteDataUtil = GarmentUnitReceiptNoteDataUtil;
			this.facade = facade;
		}
		public async Task<GarmentBeacukai> GetNewData(string user, GarmentDeliveryOrder garmentDeliveryOrder = null)
		{
			long nowTicks = DateTimeOffset.Now.Ticks;
			var garmentDO = garmentDeliveryOrder ?? await Task.Run(() => garmentDeliveryOrderDataUtil.GetNewData("User"));
			
			return new GarmentBeacukai
			{
				BeacukaiNo = "BeacukaiNo",
				BeacukaiDate = DateTimeOffset.Now,
				SupplierId = It.IsAny<int>(),
				SupplierCode = "codeS",
				SupplierName = "nameS",
				BillNo = "BP181115160748000042",
				Bruto = 10,
				Netto = 10,
				Packaging = "COllY",
				PackagingQty = 2,
				CustomsType = "customsType",
				CustomsCategory = false,
				ValidationDate = DateTimeOffset.Now,
				CurrencyId = It.IsAny<int>(),
				CurrencyCode = "TEST",
                ArrivalDate= DateTimeOffset.Now,
                Items = new List<GarmentBeacukaiItem>
					{
						new GarmentBeacukaiItem
						{

						   GarmentDOId =garmentDO.Id,
						   DODate=garmentDO.DODate,
						   GarmentDONo=garmentDO.DONo,
						   ArrivalDate  =  garmentDO.ArrivalDate,
						   TotalAmount = (decimal)garmentDO.TotalAmount,
						   TotalQty=50
						
						}
				}
			};
		}
		public async Task<GarmentBeacukai> GetNewDataWithURN(string user)
		{
			long nowTicks = DateTimeOffset.Now.Ticks;
			var garmentDO = await Task.Run(() => garmentDeliveryOrderDataUtil.GetNewData("User"));
            var garmentURN = await Task.Run(() => garmentUnitReceiptNoteDataUtil.GetTestData(garmentDeliveryOrder: garmentDO));
            return new GarmentBeacukai
			{
				BeacukaiNo = "BeacukaiNo",
				BeacukaiDate = DateTimeOffset.Now,
				SupplierId = It.IsAny<int>(),
				SupplierCode = "codeS",
				SupplierName = "nameS",
				BillNo = "BP181115160748000042",
				Bruto = 10,
				Netto = 10,
				Packaging = "COllY",
				CustomsCategory = false,
				PackagingQty = 2,
				CustomsType = "customsType",
				ValidationDate = DateTimeOffset.Now,
				CurrencyId = It.IsAny<int>(),
				CurrencyCode = "TEST",
                ArrivalDate= DateTimeOffset.Now,
                Items = new List<GarmentBeacukaiItem>
					{
						new GarmentBeacukaiItem
						{

						   GarmentDOId =garmentDO.Id,
						   DODate=garmentDO.DODate,
						   GarmentDONo=garmentDO.DONo,
						   ArrivalDate  =  garmentDO.ArrivalDate,
						   TotalAmount = (decimal)garmentDO.TotalAmount,
						   TotalQty=50
						
						}
				}
			};
		}
		public async Task<GarmentBeacukaiViewModel> GetViewModel(string user)
		{
			long nowTicks = DateTimeOffset.Now.Ticks;
			var garmentDO = await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());

			return new GarmentBeacukaiViewModel
			{
				beacukaiNo = "",
				beacukaiDate = DateTimeOffset.MinValue,
				supplier = { },
				customType = null,
				packagingQty = 0,
				netto = 0,
				bruto = 0,
				packaging = "",
				currency = { },
                arrivalDate= DateTimeOffset.Now,
                items = new List<GarmentBeacukaiItemViewModel> {new  GarmentBeacukaiItemViewModel {
					selected=false,
					deliveryOrder  =  new GarmentDeliveryOrderViewModel{
						   Id=garmentDO.Id,
						   doDate=garmentDO.DODate,
						   doNo=garmentDO.DONo
					}
				}
				}
			};
		}
		public async Task<GarmentBeacukai> GetTestData(string user, GarmentDeliveryOrder garmentDeliveryOrder)
		{
			GarmentBeacukai model = await GetNewData(user, garmentDeliveryOrder);

			await facade.Create(model, user);

			return model;
		}
		public async Task<GarmentBeacukai> GetTestDataWithURN(string user)
		{
			GarmentBeacukai model = await GetNewDataWithURN(user);

			await facade.Create(model, user);

			return model;
		}

        public async Task<GarmentBeacukai> GetTestData1(string user)
        {
            var garmentDO = await Task.Run(() => garmentDeliveryOrderDataUtil.GetTestData());
            GarmentBeacukai model = await GetNewData(user,garmentDO);

            await facade.Create(model, user);

            return model;
        }
    }
}
