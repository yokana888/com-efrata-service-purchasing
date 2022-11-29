using System;
using System.IO;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService.ExcelGenerator
{
    public interface IBudgetCashflowUnitExcelGenerator
    {
        MemoryStream Generate(int unitId, DateTimeOffset dueDate);
    }
}
