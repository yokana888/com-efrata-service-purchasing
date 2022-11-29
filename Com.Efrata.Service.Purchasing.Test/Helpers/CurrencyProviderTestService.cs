using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.Helpers
{
    public class CurrencyProviderTestService : ICurrencyProvider
    {
        public Task<Currency> GetCurrencyByCurrencyCode(string currencyCode)
        {
            return Task.Run(() => new Currency());
        }
        public Task<Currency> GetCurrencyByCurrencyCodeDate(string currencyCode, DateTimeOffset date)
        {
            return Task.Run(() => new Currency());
        }

        public Task<List<Currency>> GetCurrencyByCurrencyCodeDateList(IEnumerable<Tuple<string, DateTimeOffset>> currencyTuples)
        {
            return Task.Run(() => new List<Currency>());
        }

        public Task<List<AccountingCategory>> GetAccountingCategoriesByCategoryIds(List<int> categoryIds)
        {
            return Task.Run(() => new List<AccountingCategory>());
        }

        public Task<List<AccountingUnit>> GetAccountingUnitsByUnitIds(List<int> unitIds)
        {
            return Task.Run(() => new List<AccountingUnit>());
        }

        public Task<List<Unit>> GetUnitsByUnitIds(List<int> unitIds)
        {
            return Task.Run(() => new List<Unit>());
        }

        public Task<List<Category>> GetCategoriesByCategoryIds(List<int> categoryIds)
        {
            return Task.Run(() => new List<Category>());
        }

        public Task<List<string>> GetCategoryIdsByAccountingCategoryId(int accountingCategoryId)
        {
            return Task.Run(() => new List<string>());
        }

        public Task<List<string>> GetUnitsIdsByAccountingUnitId(int accountingUnitId)
        {
            return Task.Run(() => new List<string>());
        }
    }
}
