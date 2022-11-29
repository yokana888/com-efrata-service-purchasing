using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel
{
    public class GarmentInternNote : BaseModel
    {
        [MaxLength(255)]
        public string INNo { get; set; }
        public string Remark { get; set; }
        public DateTimeOffset INDate { get; set; }

        /*Currency*/
        [MaxLength(255)]
        public long? CurrencyId { get; set; }
        [MaxLength(255)]
        public string CurrencyCode { get; set; }
        [MaxLength(1000)]
        public double CurrencyRate { get; set; }

        /*Supplier*/
        [MaxLength(255)]
        public long? SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }
        public virtual ICollection<GarmentInternNoteItem> Items { get; set; }

        public bool IsCreatedVB { get; set; }
        public bool IsPphPaid { get; set; }
        public bool DPPVATIsPaid { get; set; }
        public PurchasingGarmentExpeditionPosition Position { get; set; }

    }
}
