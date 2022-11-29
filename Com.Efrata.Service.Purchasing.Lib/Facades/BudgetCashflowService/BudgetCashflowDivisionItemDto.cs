namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public class BudgetCashflowDivisionItemDto
    {
        public BudgetCashflowDivisionItemDto()
        {

        }

        public BudgetCashflowDivisionItemDto(string currencyIdString, string currencyCode, double currencyRate, string divisionIdString, string unitIdString, double total, BudgetCashflowCategoryLayoutOrder layoutOrder)
        {
            int.TryParse(currencyIdString, out var currencyId);
            CurrencyId = currencyId;

            int.TryParse(unitIdString, out var unitId);
            UnitId = unitId;

            int.TryParse(divisionIdString, out var divisionId);
            DivisionId = divisionId;

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
        }

        public BudgetCashflowDivisionItemDto(int currencyId, string currencyCode, double currencyRate, string divisionIdString, string unitIdString, double total, BudgetCashflowCategoryLayoutOrder layoutOrder)
        {
            CurrencyId = currencyId;

            int.TryParse(unitIdString, out var unitId);
            UnitId = unitId;

            int.TryParse(divisionIdString, out var divisionId);
            DivisionId = divisionId;

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
        }

        public BudgetCashflowDivisionItemDto(int currencyId, int divisionId, int unitId, double nominal, double currencyNominal, double actualNominal, BudgetCashflowCategoryLayoutOrder layoutOrder)
        {
            CurrencyId = currencyId;
            CurrencyNominal = currencyNominal;
            Nominal = nominal;
            ActualNominal = actualNominal;
            LayoutOrder = layoutOrder;
            LayoutName = layoutOrder.ToDescriptionString();
            UnitId = unitId;
            DivisionId = divisionId;
        }

        public int CurrencyId { get; private set; }
        public double CurrencyNominal { get; private set; }
        public double Nominal { get; private set; }
        public double ActualNominal { get; private set; }
        public BudgetCashflowCategoryLayoutOrder LayoutOrder { get; private set; }
        public string LayoutName { get; private set; }
        public int UnitId { get; private set; }
        public int DivisionId { get; private set; }
    }
}