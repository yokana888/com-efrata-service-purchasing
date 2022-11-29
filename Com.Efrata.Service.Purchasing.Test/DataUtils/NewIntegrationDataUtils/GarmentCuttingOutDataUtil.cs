using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentCuttingOut;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class GarmentCuttingOutDataUtil
    {
        public GarmentCuttingOutViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentCuttingOutViewModel
            {
                Article = $"Artcle{nowTicks}",
                CuttingOutDate = DateTime.Now,
                CuttingOutType = "CutOut",
                CutOutNo = $"CutOutNo{nowTicks}",
                RONo = "RONo123",
                TotalCuttingOutQuantity = 20,

            };
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            return GetResultFormatterOk(GetNewData());
        }

        public Dictionary<string, object> GetResultFormatterOk(GarmentCuttingOutViewModel garmentExpenditureGoodViewModel)
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

        public string GetResultFormatterOkString(GarmentCuttingOutViewModel garmentExpenditureGoodViewModel)
        {
            var result = GetResultFormatterOk(garmentExpenditureGoodViewModel);

            return JsonConvert.SerializeObject(result);
        }
        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            var data = new List<GarmentCuttingOutViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

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

        public string GetNullFormatterOkString()
        {
            var result = GetNullFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}
