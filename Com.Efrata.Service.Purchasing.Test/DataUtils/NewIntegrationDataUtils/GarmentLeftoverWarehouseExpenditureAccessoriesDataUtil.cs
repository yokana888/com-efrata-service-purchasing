using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil
    {
        public GarmentLeftoverWarehouseExpenditureAccessoriesViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel
            {
                Id=1,
                ExpenditureNo="exNo",
                IsUsed=false,
                Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel
                    {
                        Quantity=1,
                    }
                }
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
    }
}
