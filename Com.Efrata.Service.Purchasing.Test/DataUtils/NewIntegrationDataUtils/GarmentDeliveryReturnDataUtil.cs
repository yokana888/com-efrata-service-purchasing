using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class GarmentDeliveryReturnDataUtil
    {
        public GarmentDeliveryReturnViewModel GetNewData(GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = null)
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentDeliveryReturnViewModel
            {
                UnitDOId = garmentUnitDeliveryOrder != null? garmentUnitDeliveryOrder.Id : 0,
                DRNo = $"DRNO{nowTicks}",
                RONo = $"RONO{nowTicks}",
                Items= new List<GarmentDeliveryReturnItemViewModel>
                {
                    new GarmentDeliveryReturnItemViewModel
                    {
                        Quantity=1,
                        IsSave=true,
                    }
                }
            };
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk(GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = null)
        {
            var data = GetNewData(garmentUnitDeliveryOrder);

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString(GarmentUnitDeliveryOrder data = null)
        {
            var result = GetResultFormatterOk(data);

            return JsonConvert.SerializeObject(result);
        }
    }
}
