using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel
{
    public class GarmentCorrectionNoteItem : StandardEntity<long>
    {
        public long DODetailId { get; set; }

        public long EPOId { get; set; }
        [MaxLength(255)]
        public string EPONo { get; set; }

        public long PRId { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }

        public long POId { get; set; }
        [MaxLength(1000)]
        public string POSerialNumber { get; set; }
        [MaxLength(1000)]
        public string RONo { get; set; }

        public long ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public long UomId { get; set; }
        [MaxLength(1000)]
        public string UomIUnit { get; set; }

        [Column(TypeName = "decimal(38, 20)")]
        public decimal PricePerDealUnitBefore { get; set; }
        [Column(TypeName = "decimal(38, 20)")]
        public decimal PricePerDealUnitAfter { get; set; }
        public decimal PriceTotalBefore { get; set; }
        public decimal PriceTotalAfter { get; set; }

        public long GCorrectionId { get; set; }
        [ForeignKey("GCorrectionId")]
        public virtual GarmentCorrectionNote GarmentCorrectionNote { get; set; }
    }
}
