using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel
{
    public class GarmentDeliveryOrderDetail : StandardEntity<long>
    {
        public long EPOItemId { get; set; }
        [MaxLength(255)]
        public int POId { get; set; }
        public int POItemId { get; set; }
        public long PRId { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }
        public long PRItemId { get; set; }
        public string POSerialNumber { get; set; }

        /* Unit */
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }

        /* Product */
        [MaxLength(255)]
        public long ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }

        public double DOQuantity { get; set; }
        public double ReturQuantity { get; set; }
        public double DealQuantity { get; set; }

        public double Conversion { get; set; }

        /* UOM */
        [MaxLength(255)]
        public string UomId { get; set; }
        [MaxLength(1000)]
        public string UomUnit { get; set; }

        public double SmallQuantity { get; set; }
        public string SmallUomId { get; set; }
        public string SmallUomUnit { get; set; }

        public double PricePerDealUnit { get; set; }
        public double PriceTotal { get; set; }

        public string RONo { get; set; }
        public double ReceiptQuantity { get; set; }
        public virtual long GarmentDOItemId { get; set; }

        public double? QuantityCorrection { get; set; }
        public double? PricePerDealUnitCorrection { get; set; }
        public double? PriceTotalCorrection { get; set; }
		public string Uid { get; set; }
        public string CustomsCategory { get; set; }
        public string CodeRequirment { get; set; }
        [ForeignKey("GarmentDOItemId")]
        public virtual GarmentDeliveryOrderItem GarmentDeliveryOrderItem { get; set; }
    }
}
