using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class GarmentPreSalesContractDataUtil
    {
        public GarmentPreSalesContractViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentPreSalesContractViewModel
            {
                SCNo = $"SCNo{nowTicks}",
                SCDate = DateTimeOffset.Now,
                SCType = null,
                SectionId = 1,
                SectionCode = $"SectionCode{nowTicks}",
                BuyerAgentId = "1",
                BuyerAgentName = $"BuyerAgentName{nowTicks}",
                BuyerAgentCode = $"BuyerAgentCode{nowTicks}",
                BuyerBrandId = "1",
                BuyerBrandName = $"BuyerBrandName{nowTicks}",
                BuyerBrandCode = $"BuyerBrandCode{nowTicks}",
                OrderQuantity = 10,
                IsCC = false,
                IsPR = false,
            };
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            var data = GetNewData();

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            var result = GetResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            var data = new List<GarmentPreSalesContractViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetMultipleResultFormatterOkString()
        {
            var result = GetMultipleResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}

