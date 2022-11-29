using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.BeacukaiAdded;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class BeacukaiAddedDataUtil
    {
        public BeacukaiAddedViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new BeacukaiAddedViewModel
            {
                BCDate = DateTime.Now,
                BCNo = "BCNo123",
                BonNo = "Invoice123",
                ItemCode = "ItemCode",
                BCType = "BC 30",
                Quantity = 1
            };
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            return GetResultFormatterOk(GetNewData());
        }

        public Dictionary<string, object> GetResultFormatterOk(BeacukaiAddedViewModel garmentExpenditureGoodViewModel)
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

        public string GetResultFormatterOkString(BeacukaiAddedViewModel garmentExpenditureGoodViewModel)
        {
            var result = GetResultFormatterOk(garmentExpenditureGoodViewModel);

            return JsonConvert.SerializeObject(result);
        }
        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            var data = new List<BeacukaiAddedViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public Dictionary<string, object> GetNullOk()
        {
            //var data = new List<BeacukaiAddedViewModel> { GetNewData() };

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
            var result = GetNullOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}
