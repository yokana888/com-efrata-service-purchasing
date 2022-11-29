using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public class DivisionDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int BudgetCashflowColumnOrder { get; set; }
    }
}
