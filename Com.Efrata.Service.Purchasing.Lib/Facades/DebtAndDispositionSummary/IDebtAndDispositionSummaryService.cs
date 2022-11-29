using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary
{
    public interface IDebtAndDispositionSummaryService
    {
        ReadResponse<DebtAndDispositionSummaryDto> GetReport(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency);
        ReadResponse<DebtAndDispositionSummaryDto> GetReportDebt(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency);
        ReadResponse<DebtAndDispositionSummaryDto> GetReportDisposition(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency);
        List<DebtAndDispositionSummaryDto> GetSummary(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency);
        List<DebtAndDispositionSummaryDto> GetDebtSummary(int unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]");
        List<DebtAndDispositionSummaryDto> GetDebtSummary(List<int> unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]");
        List<DebtAndDispositionSummaryDto> GetDebtSummary(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency);
        List<DebtAndDispositionSummaryDto> GetSummary(int unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]");
        List<DebtAndDispositionSummaryDto> GetSummary(List<int> unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]");
        List<DebtAndDispositionSummaryDto> GetDispositionSummary(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency);
    }
}
