using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public interface IBudgetCashflowService
    {
        List<BudgetCashflowItemDto> GetBudgetCashflowWorstCase(DateTimeOffset dueDate, int unitId);
        List<BudgetCashflowItemDto> GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder layoutOrder, int unitId, DateTimeOffset dueDate);
        BudgetCashflowDivisionDto GetBudgetCashflowDivision(BudgetCashflowCategoryLayoutOrder layoutOrder, int divisionId, DateTimeOffset dueDate);
        Task<int> UpsertWorstCaseBudgetCashflowUnit(WorstCaseBudgetCashflowFormDto form);
        Task<int> UpdateWorstCaseBudgetCashflowUnit(int year, int month, int unitId, WorstCaseBudgetCashflowFormDto form);
        List<BudgetCashflowItemDto> GetCashInOperatingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetCashOutOperatingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetDiffOperatingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetCashInInvestingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetCashOutInvestingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetDiffInvestingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetCashInFinancingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetCashOutFinancingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetDiffFinancingActivitiesByUnit(int unitId, DateTimeOffset dueDate);
        List<BudgetCashflowItemDto> GetBudgetCashflowByCategoryAndUnitId(List<int> categoryIds, int unitId, DateTimeOffset dueDate, int divisionId, bool isImport);
    }
}
