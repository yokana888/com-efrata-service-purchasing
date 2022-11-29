using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel
{
    public class GarmentInventorySummary : BaseModel
    {
        [MaxLength(255)]
        public string No { get; set; }

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

        public decimal Quantity { get; set; }

        public long UomId { get; set; }
        [MaxLength(255)]
        public string UomUnit { get; set; }

        public decimal StockPlanning { get; set; }
    }
}
