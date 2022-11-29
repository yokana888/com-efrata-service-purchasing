using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel
{
    public class UnitPaymentCorrectionNote : BaseModel
    {
        public string UPCNo { get; set; }
        public DateTimeOffset CorrectionDate { get; set; }
        public string CorrectionType { get; set; }
        public long UPOId { get; set; }
        public string UPONo { get; set; }
        [MaxLength(255)]
        public string SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }
        [StringLength(100)]
        public string SupplierNpwp { get; set; }
        [MaxLength(255)]
        public string DivisionId { get; set; }
        [MaxLength(255)]
        public string DivisionCode { get; set; }
        [MaxLength(1000)]
        public string DivisionName { get; set; }
        [MaxLength(255)]
        public string CategoryId { get; set; }
        [MaxLength(255)]
        public string CategoryCode { get; set; }
        [MaxLength(1000)]
        public string CategoryName { get; set; }
        public string InvoiceCorrectionNo { get; set; }
        public DateTimeOffset? InvoiceCorrectionDate { get; set; }
        public bool useVat { get; set; }
        public string VatTaxCorrectionNo { get; set; }
        public DateTimeOffset? VatTaxCorrectionDate { get; set; }
        public bool useIncomeTax { get; set; }
        public string IncomeTaxCorrectionNo { get; set; }
        public DateTimeOffset? IncomeTaxCorrectionDate { get; set; }
        public string ReleaseOrderNoteNo { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string Remark { get; set; }
        public string ReturNoteNo { get; set; }
        public virtual ICollection<UnitPaymentCorrectionNoteItem> Items { get; set; }
    }
}
