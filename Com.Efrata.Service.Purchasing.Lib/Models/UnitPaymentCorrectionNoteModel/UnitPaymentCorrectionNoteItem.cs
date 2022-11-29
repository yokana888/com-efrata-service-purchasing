using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel
{
    public class UnitPaymentCorrectionNoteItem : StandardEntity<long>
    {
        
        public long UPODetailId { get; set; }
        public string URNNo { get; set; }
        public string EPONo { get; set; }
        public long PRId { get; set; }
        public string PRNo { get; set; }
        public long PRDetailId { get; set; }
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string UomId { get; set; }
        public string UomUnit { get; set; }
        public double PricePerDealUnitBefore { get; set; }
        public double PricePerDealUnitAfter { get; set; }
        public double PriceTotalBefore { get; set; }
        public double PriceTotalAfter { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyRate { get; set; }
        public virtual long UPCId { get; set; }
        [ForeignKey("UPCId")]
        public virtual UnitPaymentCorrectionNote UnitPaymentCorrectionNote { get; set; }

    }
}
