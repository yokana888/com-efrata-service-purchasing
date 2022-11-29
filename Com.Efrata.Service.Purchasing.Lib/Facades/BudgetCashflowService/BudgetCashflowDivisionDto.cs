using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public class BudgetCashflowDivisionDto
    {
        public BudgetCashflowDivisionDto(List<int> unitIds, List<BudgetCashflowDivisionItemDto> items)
        {
            UnitIds = unitIds;
            Items = items;
        }

        public List<int> UnitIds { get; private set; }
        public List<BudgetCashflowDivisionItemDto> Items { get; private set; }
    }
}
