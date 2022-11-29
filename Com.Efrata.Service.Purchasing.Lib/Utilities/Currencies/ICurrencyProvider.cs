using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies
{
    public interface ICurrencyProvider
    {
        Task<Currency> GetCurrencyByCurrencyCode(string currencyCode);
        Task<Currency> GetCurrencyByCurrencyCodeDate(string currencyCode, DateTimeOffset date);
        Task<List<Currency>> GetCurrencyByCurrencyCodeDateList(IEnumerable<Tuple<string, DateTimeOffset>> currencyTuples);
        Task<List<AccountingCategory>> GetAccountingCategoriesByCategoryIds(List<int> categoryIds);
        Task<List<AccountingUnit>> GetAccountingUnitsByUnitIds(List<int> unitIds);
        Task<List<Unit>> GetUnitsByUnitIds(List<int> unitIds);
        Task<List<Category>> GetCategoriesByCategoryIds(List<int> categoryIds);
        Task<List<string>> GetCategoryIdsByAccountingCategoryId(int accountingCategoryId);
        Task<List<string>> GetUnitsIdsByAccountingUnitId(int accountingUnitId);
    }
}
