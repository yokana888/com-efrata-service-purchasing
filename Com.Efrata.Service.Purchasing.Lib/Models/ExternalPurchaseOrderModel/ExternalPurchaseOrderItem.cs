using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel
{
    public class ExternalPurchaseOrderItem : StandardEntity<long>
    {
        public long POId { get; set; }
        [MaxLength(255)]
        public string PONo { get; set; }
        public long PRId { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }

        //Unit
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(1000)]
        public string UnitName { get; set; }

        public virtual long EPOId { get; set; }
        [ForeignKey("EPOId")]
        public virtual ExternalPurchaseOrder ExternalPurchaseOrder { get; set; }

        public virtual ICollection<ExternalPurchaseOrderDetail> Details { get; set; }
    }
}
