using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel
{
    public class GarmentCorrectionNote : BaseModel
    {
        [MaxLength(255)]
        public string CorrectionNo { get; set; }
        public DateTimeOffset CorrectionDate { get; set; }
        [MaxLength(255)]
        public string CorrectionType { get; set; }

        public long DOId { get; set; }
        [MaxLength(255)]
        public string DONo { get; set; }

        public long SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }

        public long CurrencyId { get; set; }
        [MaxLength(255)]
        public string CurrencyCode { get; set; }

        public bool UseVat { get; set; }
        public bool UseIncomeTax { get; set; }

        public long IncomeTaxId { get; set; }
        [MaxLength(1000)]
        public string IncomeTaxName { get; set; }
        public decimal IncomeTaxRate { get; set; }

        public string Remark { get; set; }

        public decimal TotalCorrection { get; set; }
        public string NKPN { get; set; }
        public string NKPH { get; set; }

        [MaxLength(255)]
        public string VatId { get; set; }
        [MaxLength(255)]
        public string VatRate { get; set; }

        public virtual ICollection<GarmentCorrectionNoteItem> Items { get; set; }
    }
}
