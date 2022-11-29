using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.BudgetCashflowWorstCaseModel
{
    public class BudgetCashflowWorstCase : StandardEntity
    {
        public BudgetCashflowWorstCase()
        {

        }

        public BudgetCashflowWorstCase(DateTimeOffset date, int unitId)
        {

            Year = date.AddMonths(1).Year;
            Month = date.AddMonths(1).Month;
            UnitId = unitId;
        }

        public int Year { get; private set; }
        public int Month { get; private set; }
        public int UnitId { get; private set; }
    }
}
