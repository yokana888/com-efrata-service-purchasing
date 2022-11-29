using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport
{
    public class ReportCurrencyAndCategoryDto
    {
        public ReportCurrencyAndCategoryDto(int currencyId, string currencyCode, double currencyRate, double amount, double currencyAmount, int categoryId, string categoryName)
        {
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            Amount = amount;
            CurrencyAmount = currencyAmount;
            CategoryId = categoryId;
            CategoryName = categoryName;
        }
        public ReportCurrencyAndCategoryDto(int currencyId, string currencyCode, double currencyRate, double amount, double currencyAmount, string categoryName)
        {
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            Amount = amount;
            CurrencyAmount = currencyAmount;
            CategoryName = categoryName;
        }

        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public double Amount { get; private set; }
        public double CurrencyAmount { get; private set; }
        public int CategoryId { get; private set; }
        public string CategoryName { get; private set; }
    }
}
