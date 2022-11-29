namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class UOMDto
    {
        public UOMDto(int uomId, string uomUnit)
        {
            Id = uomId;
            Unit = uomUnit;
        }

        public UOMDto(string dealUomId, string dealUomUnit)
        {
            int.TryParse(dealUomId, out var id);
            Id = id;
            Unit = dealUomUnit;
        }

        public int Id { get; private set; }
        public string Unit { get; private set; }
    }
}