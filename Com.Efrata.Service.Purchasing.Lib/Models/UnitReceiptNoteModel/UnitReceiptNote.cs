using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel
{
    public class UnitReceiptNote : BaseModel
    {
        [MaxLength(255)]
        public string URNNo { get; set; }

        //Division
        [MaxLength(255)]
        public string DivisionId { get; set; }
        [MaxLength(255)]
        public string DivisionCode { get; set; }
        [MaxLength(1000)]
        public string DivisionName { get; set; }

        //Unit
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(1000)]
        public string UnitName { get; set; }

        //Supplier
        [MaxLength(255)]
        public string SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }
        public bool SupplierIsImport { get; set; }

        public long DOId { get; set; }
        public string DONo { get; set; }

        public DateTimeOffset ReceiptDate { get; set; }
        public bool IsStorage { get; set; }

        //Storage
        [MaxLength(255)]
        public string StorageId { get; set; }
        [MaxLength(255)]
        public string StorageCode { get; set; }
        [MaxLength(1000)]
        public string StorageName { get; set; }

        public bool IsPaid { get; set; }
        public string Remark { get; set; }

        public virtual ICollection<UnitReceiptNoteItem> Items { get; set; }
    }
}
