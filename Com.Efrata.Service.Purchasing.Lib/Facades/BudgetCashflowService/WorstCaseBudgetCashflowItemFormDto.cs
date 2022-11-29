namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public class WorstCaseBudgetCashflowItemFormDto
    {
        public BudgetCashflowCategoryLayoutOrder LayoutOrder { get; set; }
        public CurrencyDto Currency { get; set; }
        public double CurrencyNominal { get; set; }
        public double Nominal { get; set; }
        public double ActualNominal { get; set; }
    }
}