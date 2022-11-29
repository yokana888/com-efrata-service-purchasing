using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel
{
    public class PurchasingDispositionItem : BaseModel
    {
        [MaxLength(255)]
        public string EPONo { get; set; }
        public string EPOId { get; set; }
        public bool UseVat { get; set; }
        public bool UseIncomeTax { get; set; }
        public string IncomeTaxId { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }

        public string VatId { get; set; }
        public string VatRate { get; set; }

        public virtual long PurchasingDispositionId { get; set; }
        [ForeignKey("PurchasingDispositionId")]
        public virtual PurchasingDisposition PurchasingDisposition { get; set; }

        public virtual ICollection<PurchasingDispositionDetail> Details { get; set; }
    }
}
