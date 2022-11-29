using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel
{
    public class GarmentInventoryDocument : BaseModel
    {
        [MaxLength(255)]
        public string No { get; set; }
        public DateTimeOffset Date { get; set; }

        [MaxLength(255)]
        public string ReferenceNo { get; set; }
        [MaxLength(255)]
        public string ReferenceType { get; set; }

        [MaxLength(255)]
        public string Type { get; set; }

        public long StorageId { get; set; }
        [MaxLength(255)]
        public string StorageCode { get; set; }
        [MaxLength(1000)]
        public string StorageName { get; set; }

        public string Remark { get; set; }

        public virtual ICollection<GarmentInventoryDocumentItem> Items { get; set; }
    }
}
