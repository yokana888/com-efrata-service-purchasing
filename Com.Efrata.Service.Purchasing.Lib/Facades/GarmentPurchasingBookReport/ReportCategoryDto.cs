namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport
{
    public class ReportCategoryDto
    {
        public ReportCategoryDto(int categoryId, string categoryName, string currencyCode, double currencyAmount, double amount)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            Amount = amount;
            CurrencyCode = currencyCode;
            CurrencyAmount = currencyAmount;
        }

        public int CategoryId { get; private set; }
        public string CategoryName { get; private set; }
        public double Amount { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyAmount { get; private set; }
    }
}