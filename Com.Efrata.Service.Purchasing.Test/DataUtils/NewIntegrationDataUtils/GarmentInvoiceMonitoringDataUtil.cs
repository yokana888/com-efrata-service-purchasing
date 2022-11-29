using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentInvoice;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class GarmentInvoiceMonitoringDataUtil
    {
        public GarmentInvoiceMonitoringViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new GarmentInvoiceMonitoringViewModel
            {
                InvoiceNo = "Invoice123",
                InvoiceDate = DateTimeOffset.Now,
                BuyerAgentName = "BuyerAgentName",
                PEBDate = DateTimeOffset.Now,
                FOB = 20,
                FOBIdr = 200,
                FAB = 200,
                FABIdr = 200,
                ToBePaid = 200,
                ToBePaidIdr = 200,
                CurrencyCode = "USD",
                Rate = 200
            };
          
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            return GetResultFormatterOk(GetNewData());
        }

        public Dictionary<string, object> GetResultFormatterOk(GarmentInvoiceMonitoringViewModel garmentInvoiceMonitoringViewModel)
        {
            var data = garmentInvoiceMonitoringViewModel;

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            return GetResultFormatterOkString(GetNewData());
        }

        public string GetResultFormatterOkString(GarmentInvoiceMonitoringViewModel garmentInvoiceMonitoringViewModel)
        {
            var result = GetResultFormatterOk(garmentInvoiceMonitoringViewModel);

            return JsonConvert.SerializeObject(result);
        }
        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            List<GarmentInvoiceMonitoringViewModel> garmentInvoices = new List<GarmentInvoiceMonitoringViewModel>();
            var newData = GetNewData();
            garmentInvoices.Add(GetNewData());
            garmentInvoices.Add(newData);
            var data = new List<GarmentInvoiceMonitoringViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(garmentInvoices);

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
