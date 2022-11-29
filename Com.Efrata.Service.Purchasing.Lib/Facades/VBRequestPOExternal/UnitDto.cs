namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class UnitDto
    {
        public UnitDto()
        {
        }

        public UnitDto(string unitId, string unitCode, string unitName, string divisionCode, string divisionId, string divisionName)
        {
            int.TryParse(unitId, out var id);
            Id = id;
            Code = unitCode;
            Name = unitName;
            Division = new DivisionDto(divisionCode, divisionId, divisionName);
        }

        public int Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public int DivisionId { get; private set; }
        public DivisionDto Division { get; private set; }
    }
}