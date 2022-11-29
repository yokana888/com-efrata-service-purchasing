using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentExpenditureGood;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class GarmentExpenditureGoodDataUtil
    {
        public GarmentExpenditureGoodViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentExpenditureGoodViewModel
            {
                Id = $"Id{nowTicks}",
                Active = false,
                CreatedBy = $"CreatedBy{nowTicks}",
                CreatedUtc = new DateTime(1970, 1, 1),
                LastModifiedBy = $"LastModifiedBy{nowTicks}",
                LastModifiedUtc = new DateTime(1970, 1, 1),
                IsDeleted = false,
                RONo = "RONo123",
                Invoice = "Invoice123",
                ExpenditureGoodNo = "ExpenditureGoodNo1",
                Article = "Article1",
                Comodity = new GarmentComodity
                {
                    Code = "ComodityCode",
                    Id = 1,
                    Name = "ComodityName"
                },
                ExpenditureDate = DateTime.Now,
                ExpenditureType = "EXPORT",
                Buyer = new Buyer
                {
                    Code = "BuyerCode",
                    Id = 1,
                    Name = "BuyerName"
                },
                TotalQuantity = 120,
            };
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            return GetResultFormatterOk(GetNewData());
        }

        public Dictionary<string, object> GetResultFormatterOk(GarmentExpenditureGoodViewModel garmentExpenditureGoodViewModel)
        {
            var data = garmentExpenditureGoodViewModel;

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            return GetResultFormatterOkString(GetNewData());
        }

        public string GetResultFormatterOkString(GarmentExpenditureGoodViewModel garmentExpenditureGoodViewModel)
        {
            var result = GetResultFormatterOk(garmentExpenditureGoodViewModel);

            return JsonConvert.SerializeObject(result);
        }
        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            List<GarmentExpenditureGoodViewModel> garmentExpenditureGoods = new List<GarmentExpenditureGoodViewModel>();
            var newData = GetNewData();
            newData.Invoice = "Invoice1234";
            garmentExpenditureGoods.Add(GetNewData());
            garmentExpenditureGoods.Add(newData);
            garmentExpenditureGoods.Add(GetNewData());
            var data = new List<GarmentExpenditureGoodViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(garmentExpenditureGoods);

            return result;
        }

        public Dictionary<string, object> GetMultipleResultFormatterOkCMT()
        {
            List<GarmentExpenditureGoodViewModel> garmentExpenditureGoods = new List<GarmentExpenditureGoodViewModel>();
            var newData = GetNewData();
            garmentExpenditureGoods.Add(GetNewData());
            garmentExpenditureGoods.Add(newData);
            var data = new List<GarmentExpenditureGoodViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(garmentExpenditureGoods);

            return result;
        }

        public Dictionary<string, object> GetNullFormatterOk()
        {
            //List<GarmentExpenditureGoodViewModel> garmentExpenditureGoods = new List<GarmentExpenditureGoodViewModel>();
            //garmentExpenditureGoods.Add(GetNewData());
            //garmentExpenditureGoods.Add(GetNewData());
            //var data = new List<GarmentExpenditureGoodViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(null);

            return result;
        }

        public string GetMultipleResultFormatterOkString()
        {
            var result = GetMultipleResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

        public string GetMultipleResultFormatterOkCMTString()
        {
            var result = GetMultipleResultFormatterOkCMT();

            return JsonConvert.SerializeObject(result);
        }

        public string GetNullFormatterOkString()
        {
            var result = GetNullFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}