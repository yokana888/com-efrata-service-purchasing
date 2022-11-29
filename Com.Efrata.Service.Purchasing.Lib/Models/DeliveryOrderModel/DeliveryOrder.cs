using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel
{
    public class DeliveryOrder : BaseModel
    {
        [MaxLength(255)]
        public string DONo { get; set; }
        public DateTimeOffset DODate { get; set; }
        public DateTimeOffset ArrivalDate { get; set; }

        /* Supplier */
        [MaxLength(255)]
        public string SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }

        public string Remark { get; set; }
        public bool IsClosed { get; set; }

        public virtual ICollection<DeliveryOrderItem> Items { get; set; }
    }
}
