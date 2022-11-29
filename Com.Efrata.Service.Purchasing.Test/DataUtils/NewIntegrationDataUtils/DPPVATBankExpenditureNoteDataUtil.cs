using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils
{
    public class DPPVATBankExpenditureNoteDataUtil
    {
        public DPPVATBankExpenditureNoteViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new DPPVATBankExpenditureNoteViewModel
            {
                Amount = 10,
                AmountDetail = 100,
                ExpenditureDate = DateTimeOffset.Now,
                DPP =100,
                Difference = 10,
                DeliveryOrdersNo = "Dono123",
                CurrencyRate = 10,
                ExpenditureNoteNo = "ExpenditureNoteNo123",
                CategoryName = "CategoryName",
            };

            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            return GetResultFormatterOk(GetNewData());
        }

        public Dictionary<string, object> GetResultFormatterOk(DPPVATBankExpenditureNoteViewModel dPPVATBankExpenditureNoteViewModel)
        {
            var data = dPPVATBankExpenditureNoteViewModel;

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            return GetResultFormatterOkString(GetNewData());
        }

        public string GetResultFormatterOkString(DPPVATBankExpenditureNoteViewModel dPPVATBankExpenditureNoteViewModel)
        {
            var result = GetResultFormatterOk(dPPVATBankExpenditureNoteViewModel);

            return JsonConvert.SerializeObject(result);
        }
        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            List<DPPVATBankExpenditureNoteViewModel> garmentInvoices = new List<DPPVATBankExpenditureNoteViewModel>();
            var newData = GetNewData();
            garmentInvoices.Add(GetNewData());
            garmentInvoices.Add(newData);
            var data = new List<DPPVATBankExpenditureNoteViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(garmentInvoices);

            return result;
        }

        public string GetMultipleResultFormatterOkString()
        {
            var result = GetMultipleResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

        //public string GetNullFormatterOkString()
        //{
        //    var result = GetNullFormatterOk();

        //    return JsonConvert.SerializeObject(result);
        //}
    }
}
