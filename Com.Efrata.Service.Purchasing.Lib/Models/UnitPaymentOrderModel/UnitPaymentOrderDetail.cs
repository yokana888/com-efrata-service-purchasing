using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel
{
    public class UnitPaymentOrderDetail : StandardEntity<long>
    {
        public long URNItemId { get; set; }
        [MaxLength(255)]
        public string EPONo { get; set; }
        public long EPODetailId { get; set; }
        public long POItemId { get; set; }
        public long PRId { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }
        public long PRItemId { get; set; }

        /*Product*/
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }

        public double ReceiptQuantity { get; set; }

        /*Uom*/
        [MaxLength(255)]
        public string UomId { get; set; }
        [MaxLength(1000)]
        public string UomUnit { get; set; }

        public double PricePerDealUnit { get; set; }
        public double PriceTotal { get; set; }
        public double PricePerDealUnitCorrection { get; set; }
        public double PriceTotalCorrection { get; set; }
        public string ProductRemark { get; set; }
        public double QuantityCorrection { get; set; }

        public virtual long UPOItemId { get; set; }
        [ForeignKey("UPOItemId")]
        public virtual UnitPaymentOrderItem UnitPaymentOrderItem { get; set; }
    }
}
