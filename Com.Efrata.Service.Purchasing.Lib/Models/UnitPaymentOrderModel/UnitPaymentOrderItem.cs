using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel
{
    public class UnitPaymentOrderItem : StandardEntity<long>
    {
        public long URNId { get; set; }
        [MaxLength(255)]
        public string URNNo { get; set; }
        public long DOId { get; set; }
        [MaxLength(255)]
        public string DONo { get; set; }

        public virtual ICollection<UnitPaymentOrderDetail> Details { get; set; }

        public virtual long UPOId { get; set; }
        [ForeignKey("UPOId")]
        public virtual UnitPaymentOrder UnitPaymentOrder { get; set; }
    }
}
