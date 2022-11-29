using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel
{
    public class DeliveryOrderItem : StandardEntity<long>
    {
        public long EPOId { get; set; }
        [MaxLength(255)]
        public string EPONo { get; set; }

        public virtual ICollection<DeliveryOrderDetail> Details { get; set; }

        public virtual long DOId { get; set; }
        [ForeignKey("DOId")]
        public virtual DeliveryOrder DeliveryOrder { get; set; }
    }
}
