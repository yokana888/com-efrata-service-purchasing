using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel
{
    public class GarmentInternNoteItem : StandardEntity<long>
    {
        public long InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset InvoiceDate { get; set; }
        public double TotalAmount { get; set; }
        public virtual ICollection<GarmentInternNoteDetail> Details { get; set; }

        public virtual long GarmentINId { get; set; }
        [ForeignKey("GarmentINId")]
        public virtual GarmentInternNote InternNote { get; set; }
    }
}
