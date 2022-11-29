namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class CurrencyDto
    {
        public CurrencyDto(long? currencyId, string currencyCode, double currencyRate)
        {
            Id = (int)currencyId.GetValueOrDefault();
            Code = currencyCode;
            Rate = currencyRate;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public double Rate { get; set; }
    }
}