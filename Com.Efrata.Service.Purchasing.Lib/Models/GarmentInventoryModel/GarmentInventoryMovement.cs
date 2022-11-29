using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel
{
    public class GarmentInventoryMovement : BaseModel
    {
        [MaxLength(255)]
        public string No { get; set; }

        public DateTimeOffset Date { get; set; }

        [MaxLength(255)]
        public string ReferenceNo { get; set; }
        [MaxLength(255)]
        public string ReferenceType { get; set; }

        public long ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }

        public long StorageId { get; set; }
        [MaxLength(255)]
        public string StorageCode { get; set; }
        [MaxLength(1000)]
        public string StorageName { get; set; }

        public decimal StockPlanning { get; set; }

        public decimal Before { get; set; }
        public decimal Quantity { get; set; }
        public decimal After { get; set; }

        public long UomId { get; set; }
        [MaxLength(1000)]
        public string UomUnit { get; set; }

        public string Remark { get; set; }

        [MaxLength(255)]
        public string Type { get; set; }

    }
}
