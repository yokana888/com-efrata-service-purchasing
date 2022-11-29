namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class SupplierDto
    {
        public SupplierDto(long? supplierId, string supplierName, string supplierCode)
        {
            Id = (int)supplierId.GetValueOrDefault();
            Name = supplierName;
            Code = supplierCode;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsImport { get; set; }
    }
}