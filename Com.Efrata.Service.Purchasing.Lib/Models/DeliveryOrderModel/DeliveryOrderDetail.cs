using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel
{
    public class DeliveryOrderDetail : StandardEntity<long>
    {
        public long EPODetailId { get; set; }
        public long POItemId { get; set; }
        public long PRId { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }
        public long PRItemId { get; set; }

        /* Unit */
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }

        /* Product */
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }

        public double DOQuantity { get; set; }
        public double DealQuantity { get; set; }

        /* UOM */
        [MaxLength(255)]
        public string UomId { get; set; }
        [MaxLength(1000)]
        public string UomUnit { get; set; }

        public double ReceiptQuantity { get; set; }
        public bool IsClosed { get; set; }

        public virtual long DOItemId { get; set; }
        [ForeignKey("DOItemId")]
        public virtual DeliveryOrderItem DeliveryOrderItem { get; set; }
    }
}
