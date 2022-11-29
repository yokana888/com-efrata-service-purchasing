namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class ProductDto
    {
        public ProductDto(string productCode, int productId, string productName, int defaultUomId, string defaultUomUnit)
        {
            Id = productId;
            Code = productCode;
            Name = productName;
            UOM = new UOMDto(defaultUomId, defaultUomUnit);
        }

        public ProductDto(string productCode, string productId, string productName, string defaultUomId, string defaultUomUnit)
        {
            int.TryParse(productId, out var id);
            Id = id;
            Code = productCode;
            Name = productName;
            UOM = new UOMDto(defaultUomId, defaultUomUnit);

        }

        public int Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public UOMDto UOM { get; private set; }
    }
}