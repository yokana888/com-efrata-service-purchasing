using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel
{
    public class GarmentDeliveryOrderItem : StandardEntity<long>
    {
        public long EPOId { get; set; }
        [MaxLength(255)]
        public string EPONo { get; set; }
        public int PaymentDueDays { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
		public string UId { get; set; }
        public virtual IEnumerable<GarmentDeliveryOrderDetail> Details { get; set; }

        public virtual long GarmentDOId { get; set; }
        [ForeignKey("GarmentDOId")]
        public virtual GarmentDeliveryOrder GarmentDeliveryOrder { get; set; }
    }
}
