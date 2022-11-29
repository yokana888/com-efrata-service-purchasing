using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport
{
    public class ReportDto
    {
        public ReportDto(List<ReportIndexDto> data, List<ReportCategoryDto> categories, List<ReportCurrencyDto> currencies)
        {
            Data = data;
            Categories = categories;
            Currencies = currencies;
        }
        public ReportDto(List<ReportIndexDto> data, List<ReportCategoryDto> categories, List<ReportCurrencyDto> currencies,List<ReportCurrencyAndCategoryDto>currencyAndCategory)
        {
            Data = data;
            Categories = categories;
            Currencies = currencies;
            CurrencyAndCategory = currencyAndCategory;
        }
        public List<ReportIndexDto> Data { get; private set; }
        public List<ReportCategoryDto> Categories { get; private set; }
        public List<ReportCurrencyDto> Currencies { get; private set; }
        public List<ReportCurrencyAndCategoryDto> CurrencyAndCategory { get; private set; }
    }
}