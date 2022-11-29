using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    public class GarmentInternNoteItemsInvoiceDto
    {
        public long Id { get; set; }
        public bool Active { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAgent { get; set; }
        public DateTime? LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedUtc { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedAgent { get; set; }
        public long InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset InvoiceDate { get; set; }
        public double TotalAmount { get; set; }
        public List<GarmentNoteItemsInvoiceDetailsDto> Details { get; set; }
        public double TotalIncomeTax { get; set; }
        public Models.GarmentInvoiceModel.GarmentInvoice GarmentInvoice { get; set; }
    }
}
