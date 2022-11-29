using Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.BudgetCashflowWorstCaseModel
{
    public class BudgetCashflowWorstCaseItem : StandardEntity
    {
        public BudgetCashflowWorstCaseItem()
        {

        }

        public BudgetCashflowWorstCaseItem(BudgetCashflowCategoryLayoutOrder layoutOrder, int currencyId, double currencyNominal, double nominal, double actualNominal, int budgetCashflowWorstCaseId, int unitId)
        {
            LayoutOrder = layoutOrder;
            CurrencyId = currencyId;
            CurrencyNominal = currencyNominal;
            Nominal = nominal;
            ActualNominal = actualNominal;
            BudgetCashflowWorstCaseId = budgetCashflowWorstCaseId;
            UnitId = unitId;
        }

        public BudgetCashflowCategoryLayoutOrder LayoutOrder { get; private set; }
        public int CurrencyId { get; private set; }
        public double CurrencyNominal { get; private set; }
        public double Nominal { get; private set; }
        public double ActualNominal { get; private set; }
        public int BudgetCashflowWorstCaseId { get; private set; }
        public int UnitId { get; private set; }

        public void UpdateNominal(double currencyNominal, double nominal, double actualNominal)
        {
            CurrencyNominal = currencyNominal;
            Nominal = nominal;
            ActualNominal = actualNominal;
        }
    }
}
