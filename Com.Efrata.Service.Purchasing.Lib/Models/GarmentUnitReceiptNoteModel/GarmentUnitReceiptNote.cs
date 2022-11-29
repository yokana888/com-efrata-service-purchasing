using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel
{
    public class GarmentUnitReceiptNote : BaseModel
    {
        [MaxLength(255)]
        public string URNNo { get; set; }

        public long UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(1000)]
        public string UnitName { get; set; }

        public long SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }

        public long DOId { get; set; }
        [MaxLength(255)]
        public string DONo { get; set; }

        public DateTimeOffset ReceiptDate { get; set; }

        public bool IsStorage { get; set; }
        public long StorageId { get; set; }
        [MaxLength(255)]
        public string StorageCode { get; set; }
        [MaxLength(1000)]
        public string StorageName { get; set; }

        public string Remark { get; set; }

        public bool IsCorrection { get; set; }

        public bool IsUnitDO { get; set; }

        public string DeletedReason { get; set; }

        public double? DOCurrencyRate { get; set; }

        public string URNType { get; set; }
        public long UENId { get; set; }
        public string UENNo { get; set; }
        public string  DRId { get; set; }
        public string DRNo { get; set; }

        public long ExpenditureId { get; set; }
        [MaxLength(20)]
        public string ExpenditureNo { get; set; }
        [MaxLength(20)]
        public string Category { get; set; }

        public virtual ICollection<GarmentUnitReceiptNoteItem> Items { get; set; }
    }
}
