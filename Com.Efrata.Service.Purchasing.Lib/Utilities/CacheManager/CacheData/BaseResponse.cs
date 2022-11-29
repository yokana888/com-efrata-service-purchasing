using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData
{
    public class BaseResponse<T>
    {
        public string apiVersion { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }

    public class IdCOAResult
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string COACode { get; set; }
    }

    public class BankAccountCOAResult
    {
        public int Id { get; set; }
        public string AccountCOA { get; set; }
    }

    public class CategoryCOAResult
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string PurchasingCOA { get; set; }
        public string StockCOA { get; set; }
        public string LocalDebtCOA { get; set; }
        public string ImportDebtCOA { get; set; }
    }

    public class IncomeTaxCOAResult
    {
        public int Id { get; set; }
        public string COACodeCredit { get; set; }
    }
}
