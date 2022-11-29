using Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary;
using Com.Efrata.Service.Purchasing.Lib.Utilities;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public class BudgetCashflowItemDto
    {
        public BudgetCashflowItemDto()
        {

        }

        public BudgetCashflowItemDto(int id, int currencyId, double currencyNominal, double nominal, double actualNominal, BudgetCashflowCategoryLayoutOrder layoutOrder)
        {
            Id = id;
            CurrencyId = currencyId;
            CurrencyNominal = currencyNominal;
            Nominal = nominal;
            ActualNominal = actualNominal;
            LayoutOrder = layoutOrder;
            LayoutName = layoutOrder.ToDescriptionString();
        }

        public BudgetCashflowItemDto(string currencyIdString, string currencyCode, double currencyRate, double total, BudgetCashflowCategoryLayoutOrder layoutOrder, string unitId, string divisionId)
        {
            int.TryParse(currencyIdString, out var currencyId);
            CurrencyId = currencyId;
            if (currencyCode != "IDR")
            {
                CurrencyNominal = total;
                ActualNominal = total * currencyRate;
            }
            else
            {
                Nominal = total;
                ActualNominal = total;
            }

            LayoutOrder = layoutOrder;
            LayoutName = layoutOrder.ToDescriptionString();
            UnitId = int.Parse(unitId);
            DivisionId = int.Parse(divisionId);
        }

        public BudgetCashflowItemDto(int currencyId, double currencyNominal, double nominal, double actualNominal, double bestCaseCurrencyNominal, double bestCaseNominal, double bestCaseActualNominal, BudgetCashflowCategoryLayoutOrder layoutOrder)
        {
            CurrencyId = currencyId;
            CurrencyNominal = currencyNominal;
            Nominal = nominal;
            ActualNominal = actualNominal;
            BestCaseCurrencyNominal = bestCaseCurrencyNominal;
            BestCaseNominal = bestCaseNominal;
            BestCaseActualNominal = bestCaseActualNominal;
            LayoutOrder = layoutOrder;
        }

        public int Id { get; private set; }
        public int CurrencyId { get; private set; }
        public double CurrencyNominal { get; private set; }
        public double ActualNominal { get; private set; }
        public double BestCaseCurrencyNominal { get; private set; }
        public double BestCaseNominal { get; private set; }
        public double BestCaseActualNominal { get; private set; }
        public double Nominal { get; private set; }
        public BudgetCashflowCategoryLayoutOrder LayoutOrder { get; private set; }
        public string LayoutName { get; private set; }
        public int UnitId { get; private set; }
        public int DivisionId { get; }
    }
}