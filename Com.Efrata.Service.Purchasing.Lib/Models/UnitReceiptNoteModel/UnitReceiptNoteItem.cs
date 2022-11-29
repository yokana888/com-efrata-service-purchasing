using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel
{
    public class UnitReceiptNoteItem : StandardEntity<long>
    {
        public long EPODetailId { get; set; }
        public long EPOId { get; set; }
        [MaxLength(255)]
        public string EPONo { get; set; }
        [MaxLength(255)]
        public string IncomeTaxBy { get; set; }

        public long DODetailId { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }
        public long PRId { get; set; }
        public long PRItemId { get; set; }

        /* Product */
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(4000)]
        public string ProductName { get; set; }

        public string Uom { get; set; }
        public string UomId { get; set; }

        public double ReceiptQuantity { get; set; }
        public double PricePerDealUnit { get; set; }
        public bool IsCorrection { get; set; }
        public string ProductRemark { get; set; }
        public bool IsPaid { get; set; }

        public long URNId { get; set; }
        [ForeignKey("URNId")]
        public virtual UnitReceiptNote UnitReceiptNote { get; set; }
    }
}
