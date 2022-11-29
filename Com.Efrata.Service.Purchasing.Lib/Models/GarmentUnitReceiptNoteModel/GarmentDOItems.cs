using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel
{
	public class GarmentDOItems : BaseModel
	{
		[MaxLength(255)]
		public string DOItemNo { get; set; }
		public string UId { get; set; }
		public long UnitId { get; set; }
		[MaxLength(255)]
		public string UnitCode { get; set; }
		[MaxLength(1000)]
		public string UnitName { get; set; }
		public long StorageId { get; set; }
		[MaxLength(255)]
		public string StorageCode { get; set; }
		[MaxLength(100)]
		public string StorageName { get; set; }
		public long POId { get; set; }
		public long POItemId { get; set; }
		public long PRItemId { get; set; }
		public long EPOItemId { get; set; }
		[MaxLength(100)]
		public string POSerialNumber { get; set; }
		public long ProductId { get; set; }
		[MaxLength(255)]
		public string ProductCode { get; set; }
		[MaxLength(1000)]
		public string ProductName { get; set; }
		[MaxLength(255)]
		public string DesignColor { get; set; }
		public decimal SmallQuantity { get; set; }
		public decimal RemainingQuantity { get; set; }
		public long SmallUomId { get; set; }
		[MaxLength(100)]
		public string SmallUomUnit { get; set; }
		public double DOCurrencyRate { get; set; }
		public long DetailReferenceId { get; set; }
		public long URNItemId { get; set; }
		[MaxLength(255)]
		public string RO { get; set; }
        public string CustomsCategory { get; set; }
    }
}
