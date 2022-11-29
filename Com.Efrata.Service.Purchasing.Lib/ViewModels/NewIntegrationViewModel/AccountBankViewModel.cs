namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel
{
    public class AccountBankViewModel
    {
        public int Id { get; set; }
        public string AccountCOA { get; set; }
        public string Code { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string AccountCurrencyId { get; set; }
        public CurrencyViewModel Currency { get; set; }
    }
}
