using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class GarmentCategoryDataUtil
    {

        public GarmentCategoryViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentCategoryViewModel
            {
                Code = "Code123",
                CodeRequirement = "BB",
                Name = "Name123",
            };
            return data;
        }

        public GarmentCategoryViewModel GetNewDataBP()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentCategoryViewModel
            {
                Code = "Code123BP",
                CodeRequirement = "BP",
                Name = "Name123BP",
            };
            return data;
        }

        public GarmentCategoryViewModel GetNewDataBE()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentCategoryViewModel
            {
                Code = "Code123BE",
                CodeRequirement = "BE",
                Name = "Name123BE",
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

        public Dictionary<string, object> GetResultFormatterOkNull()
        {
            var data = GetNewData();

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(null);

            return result;
        }


        public string GetResultFormatterOkString()
        {
            var result = GetResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

        public string GetResultFormatterOkNullString()
        {
            var result = GetResultFormatterOkNull();

            return JsonConvert.SerializeObject(result);
        }

        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            var data = new List<GarmentCategoryViewModel>();
            data.Add(GetNewData());
            data.Add(GetNewDataBP());
            data.Add(GetNewDataBE());
            //var data = new List<GarmentCategoryViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }


        //public string GetResultFormatterOkString()
        //{
        //    var result = GetResultFormatterOk();

        //    return JsonConvert.SerializeObject(result);
        //}

        public string GetMultipleResultFormatterOkString()
        {
            var result = GetMultipleResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}
