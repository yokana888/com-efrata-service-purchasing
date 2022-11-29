using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentStockOpnameModel
{
    public class GarmentStockOpname : StandardEntity<int>
    {
        public DateTimeOffset Date { get; set; }

        public int UnitId { get; set; }
        [MaxLength(25)]
        public string UnitCode { get; set; }
        [MaxLength(255)]
        public string UnitName { get; set; }

        public int StorageId { get; set; }
        [MaxLength(25)]
        public string StorageCode { get; set; }
        [MaxLength(255)]
        public string StorageName { get; set; }

        public virtual ICollection<GarmentStockOpnameItem> Items { get; set; }
    }
}
