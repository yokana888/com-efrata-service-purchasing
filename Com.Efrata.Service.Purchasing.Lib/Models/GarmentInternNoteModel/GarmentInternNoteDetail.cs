using Com.Moonlay.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel
{
    public class GarmentInternNoteDetail : StandardEntity<long>
    {
        public long DOId { get; set; }
        [MaxLength(255)]
        public string DONo { get; set; }

        public long EPOId { get; set; }
        [MaxLength(255)]
        public string EPONo { get; set; }

        [MaxLength(255)]
        public string POSerialNumber { get; set; }
        [MaxLength(255)]
        public string RONo { get; set; }

        [MaxLength(255)]
        public string PaymentMethod { get; set; }
        [MaxLength(255)]
        public string PaymentType { get; set; }
        public int PaymentDueDays { get; set; }
        public DateTimeOffset PaymentDueDate { get; set; }
        public DateTimeOffset DODate { get; set; }
        public int InvoiceDetailId { get; set; }

        /*Product*/
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(255)]
        public long? ProductId { get; set; }
        [MaxLength(255)]
        public string ProductName { get; set; }

        public double Quantity { get; set; }

        /* Unit */
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(255)]
        public string UnitName { get; set; }

        /*UOM*/
        public long? UOMId { get; set; }
        [MaxLength(255)]
        public string UOMUnit { get; set; }
        
        public double PricePerDealUnit { get; set; }
        public double PriceTotal { get; set; }

        public virtual long GarmentItemINId { get; set; }
        [ForeignKey("GarmentItemINId")]
        public virtual GarmentInternNoteItem InternNoteItem { get; set; }
    }
}