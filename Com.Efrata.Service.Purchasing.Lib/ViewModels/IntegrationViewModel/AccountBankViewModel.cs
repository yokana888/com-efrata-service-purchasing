namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel
{
    public class AccountBankViewModel
    {
        public string _id { get; set; }
        public string accountCOA { get; set; }
        public string code { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string bankName { get; set; }
        public string bankCode { get; set; }
        public string accountCurrencyId { get; set; }
        public CurrencyViewModel currency { get; set; }
    }
}
