using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentStockOpnameModel
{
    public class GarmentStockOpnameItem : StandardEntity<int>
    {
        public int GarmentStockOpnameId { get; set; }
        [ForeignKey("GarmentStockOpnameId")]
        public virtual GarmentStockOpname GarmentStockOpname { get; set; }

        public long DOItemId { get; set; }

        [MaxLength(255)]
        public string DOItemNo { get; set; }
        [MaxLength(100)]
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
        public long SmallUomId { get; set; }
        [MaxLength(100)]
        public string SmallUomUnit { get; set; }
        public double DOCurrencyRate { get; set; }
        public long DetailReferenceId { get; set; }
        public long URNItemId { get; set; }
        [MaxLength(255)]
        public string RO { get; set; }

        public decimal BeforeQuantity { get; set; }
        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
