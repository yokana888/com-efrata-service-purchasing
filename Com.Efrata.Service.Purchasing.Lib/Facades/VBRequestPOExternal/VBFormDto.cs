using iTextSharp.text;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class VBFormDto
    {
        public DateTimeOffset? Date { get; set; }
        public string DocumentNo { get; set; }
        public List<long> EPOIds { get; set; }
        public List<UPOAndAmountDto> UPOIds { get; set; }
        public double? Amount { get; set; }
        public AccountBankViewModel Bank { get; set; }
    }

    public class AccountBankViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountCOA { get; set; }

        public CurrencyViewModel Currency { get; set; }
    }

    public class CurrencyViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
    }
}