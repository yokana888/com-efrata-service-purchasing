using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel
{
    public class ExternalPurchaseOrderDetail : StandardEntity<long>
    {
        public long POItemId { get; set; }
        public long PRItemId { get; set; }

        /* Product */
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(4000)]
        public string ProductName { get; set; }

        public double DefaultQuantity { get; set; }
        public double DealQuantity { get; set; }

        [MaxLength(4000)]
        public string DefaultUomId { get; set; }

        [MaxLength(4000)]
        public string DefaultUomUnit { get; set; }

        [MaxLength(4000)]
        public string DealUomId { get; set; }

        [MaxLength(4000)]
        public string DealUomUnit { get; set; }

        public double PricePerDealUnit { get; set; }
        public double PriceBeforeTax { get; set; }
        public double Conversion { get; set; }
        public bool IncludePpn { get; set; }

        [MaxLength(4000)]
        public string ProductRemark { get; set; }
        public double DOQuantity { get; set; }
        public double ReceiptQuantity { get; set; }
        public double DispositionQuantity { get; set; }

        public virtual long EPOItemId { get; set; }
        [ForeignKey("EPOItemId")]
        public virtual ExternalPurchaseOrderItem ExternalPurchaseOrderItem { get; set; }
    }
}
