using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel
{
    public class InternalPurchaseOrderItem : StandardEntity<long>
    {

        [MaxLength(255)]
        public long PRItemId { get; set; }

        /*Product*/
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(4000)]
        public string ProductName { get; set; }
        [MaxLength(255)]
        public string UomId { get; set; }
        [MaxLength(255)]
        public string UomUnit { get; set; }


        public Double Quantity { get; set; }
        [MaxLength(1000)]
        public string ProductRemark { get; set; }
        [MaxLength(255)]
        public string Status { get; set; }

        public virtual ICollection<InternalPurchaseOrderFulFillment> Fulfillments { get; set; }

        public virtual long POId { get; set; }
        [ForeignKey("POId")]
        public virtual InternalPurchaseOrder InternalPurchaseOrder { get; set; }
    }
}
