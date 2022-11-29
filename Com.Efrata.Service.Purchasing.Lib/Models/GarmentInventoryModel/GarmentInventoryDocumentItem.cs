using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel
{
    public class GarmentInventoryDocumentItem : StandardEntity<long>
    {
        public long GarmentInventoryDocumentId { get; set; }
        [ForeignKey("GarmentInventoryDocumentId")]
        public virtual GarmentInventoryDocument GarmentInventoryDocument { get; set; }

        public long ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public long UomId { get; set; }
        [MaxLength(1000)]
        public string UomUnit { get; set; }

        public string ProductRemark { get; set; }

        public int? MongoIndexItem { get; set; }
    }
}
