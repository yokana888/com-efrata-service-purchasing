using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.CostCalculationGarment;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class CostCalculationGarmentDataUtil
    {
        public CostCalculationGarmentViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new CostCalculationGarmentViewModel
            {
                RO_Number = $"RO_Number{nowTicks}",
                CostCalculationGarment_Materials = new List<CostCalculationGarment_MaterialViewModel>
                {
                    new CostCalculationGarment_MaterialViewModel
                    {
                        PO_SerialNumber = $"PO_SerialNumber{nowTicks}",
                        Product = new GarmentProductViewModel {
                            Id = 1,
                            Code = $"ProductCode{nowTicks}",
                        },
                        BudgetQuantity = 100,
                        UOMPrice = new UomViewModel {
                            Id = "1",
                            Unit = $"UOMPriceUnit{nowTicks}",
                        },
                        IsPRMaster = true,
                    }
                }
            };
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            return GetResultFormatterOk(GetNewData());
        }

        public Dictionary<string, object> GetResultFormatterOk(CostCalculationGarmentViewModel costCalculationGarmentViewModel)
        {
            var data = costCalculationGarmentViewModel;

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            return GetResultFormatterOkString(GetNewData());
        }

        public string GetResultFormatterOkString(CostCalculationGarmentViewModel costCalculationGarmentViewModel)
        {
            var result = GetResultFormatterOk(costCalculationGarmentViewModel);

            return JsonConvert.SerializeObject(result);
        }
    }
}
