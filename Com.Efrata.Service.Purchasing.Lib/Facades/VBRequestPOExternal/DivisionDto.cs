namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class DivisionDto
    {
        public DivisionDto(string divisionCode, string divisionId, string divisionName)
        {
            int.TryParse(divisionId, out var id);
            Id = id;
            Code = divisionCode;
            Name = divisionName;
        }

        public int Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
    }
}