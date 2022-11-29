using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    public class GarmentInternalNoteDetailsDto
    {
        public long? Id { get; set; }
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
        public string UId { get; set; }
        public string INNo { get; set; }
        public long INId { get; set; }
        public string Remark { get; set; }
        public DateTimeOffset? INDate { get; set; }

        public long? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }
        
        public double? CurrencyRate { get; set; }

        public long? SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public DateTimeOffset? INDueDate { get; set; }
        public double? TotalAmount { get; set; }
        public virtual ICollection<GarmentInternNoteItemsInvoiceDto> Items { get; set; }

        public bool IsCreatedVB { get; set; }
        public PurchasingGarmentExpeditionPosition Position { get; set; }
    }
}
