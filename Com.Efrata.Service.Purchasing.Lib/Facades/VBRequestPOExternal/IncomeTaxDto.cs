namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class IncomeTaxDto
    {
        public IncomeTaxDto(string incomeTaxId, string incomeTaxName, string incomeTaxRate)
        {
            int.TryParse(incomeTaxId, out var id);
            Id = id;

            double.TryParse(incomeTaxRate, out var rate);
            Rate = rate;

            Name = incomeTaxName;
        }

        public IncomeTaxDto(string incomeTaxId, string incomeTaxName, double incomeTaxRate)
        {
            int.TryParse(incomeTaxId, out var id);
            Id = id;

            Rate = incomeTaxRate;

            Name = incomeTaxName;
        }

        public IncomeTaxDto(long incomeTaxId, string incomeTaxName, double incomeTaxRate)
        {
            Id = (int)incomeTaxId;

            Rate = incomeTaxRate;

            Name = incomeTaxName;
        }

        public int Id { get; private set; }
        public double Rate { get; private set; }
        public string Name { get; private set; }
    }
}