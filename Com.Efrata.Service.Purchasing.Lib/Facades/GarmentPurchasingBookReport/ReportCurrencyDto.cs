namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport
{
    public class ReportCurrencyDto
    {
        public ReportCurrencyDto(int currencyId, string currencyCode, double currencyRate, double amount, double currencyAmount)
        {
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            Amount = amount;
            CurrencyAmount = currencyAmount;
        }

        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public double Amount { get; private set; }
        public double CurrencyAmount { get; private set; }
    }
}