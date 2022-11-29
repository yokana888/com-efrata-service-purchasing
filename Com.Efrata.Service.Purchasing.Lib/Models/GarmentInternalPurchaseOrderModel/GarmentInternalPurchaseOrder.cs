using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel
{
    public class GarmentInternalPurchaseOrder : BaseModel
    {
        [MaxLength(255)]
        public string PONo { get; set; }
        public long PRId { get; set; }
        public DateTimeOffset PRDate { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }
        [MaxLength(255)]
        public string RONo { get; set; }

        [MaxLength(255)]
        public string BuyerId { get; set; }
        [MaxLength(255)]
        public string BuyerCode { get; set; }
        [MaxLength(1000)]
        public string BuyerName { get; set; }

        [MaxLength(1000)]
        public string Article { get; set; }

        public DateTimeOffset? ExpectedDeliveryDate { get; set; }
        public DateTimeOffset ShipmentDate { get; set; }

        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(1000)]
        public string UnitName { get; set; }

        [MaxLength(255)]
        public string DivisionId { get; set; }
        [MaxLength(255)]
        public string DivisionCode { get; set; }
        [MaxLength(1000)]
        public string DivisionName { get; set; }

        public bool IsPosted { get; set; }
        public bool IsClosed { get; set; }
        public string Remark { get; set; }

        public virtual ICollection<GarmentInternalPurchaseOrderItem> Items { get; set; }
    }
}
