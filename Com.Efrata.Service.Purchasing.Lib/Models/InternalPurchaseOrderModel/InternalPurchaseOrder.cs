using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel
{
    public class InternalPurchaseOrder : BaseModel
    {
        [MaxLength(255)]
        public string PONo { get; set; }
        [MaxLength(255)]
        public string IsoNo { get; set; }
        [MaxLength(255)]
        public string PRId { get; set; }
        [MaxLength(255)]
        public string PRNo { get; set; }
        public DateTimeOffset PRDate { get; set; }
        public DateTimeOffset ExpectedDeliveryDate { get; set; }

        /*Budget*/
        [MaxLength(255)]
        public string BudgetId { get; set; }
        [MaxLength(255)]
        public string BudgetCode { get; set; }
        [MaxLength(1000)]
        public string BudgetName { get; set; }

        /*Division*/
        [MaxLength(255)]
        public string DivisionId { get; set; }
        [MaxLength(255)]
        public string DivisionCode { get; set; }
        [MaxLength(1000)]
        public string DivisionName { get; set; }

        /*Unit*/
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(1000)]
        public string UnitName { get; set; }

        /*Category*/
        [MaxLength(255)]
        public string CategoryId { get; set; }
        [MaxLength(255)]
        public string CategoryCode { get; set; }
        [MaxLength(1000)]
        public string CategoryName { get; set; }

        public string Remark { get; set; }
        public bool IsPosted { get; set; }
        public bool IsClosed { get; set; }
        [MaxLength(255)]
        public string Status { get; set; }
        public virtual ICollection<InternalPurchaseOrderItem> Items { get; set; }
    }
}
