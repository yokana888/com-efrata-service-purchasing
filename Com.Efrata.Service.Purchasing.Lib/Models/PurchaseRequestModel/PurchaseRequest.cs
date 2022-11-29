using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel
{
    public class PurchaseRequest : BaseModel
    {
        [MaxLength(255)]
        public string No { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset ExpectedDeliveryDate { get; set; }

        /* Budget */
        [MaxLength(255)]
        public string BudgetId { get; set; }
        [MaxLength(255)]
        public string BudgetCode { get; set; }
        [MaxLength(1000)]
        public string BudgetName { get; set; }

        /* Unit */
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(1000)]
        public string UnitName { get; set; }

        /* Division */
        [MaxLength(255)]
        public string DivisionId { get; set; }
        [MaxLength(255)]
        public string DivisionCode { get; set; }
        [MaxLength(1000)]
        public string DivisionName { get; set; }

        /* Category */
        [MaxLength(255)]
        public string CategoryId { get; set; }
        [MaxLength(255)]
        public string CategoryCode { get; set; }
        [MaxLength(1000)]
        public string CategoryName { get; set; }

        public bool IsPosted { get; set; }
        public bool IsUsed { get; set; }
        public string Remark { get; set; }
        public bool Internal { get; set; }
        public PurchaseRequestStatus Status { get; set; }

        public virtual ICollection<PurchaseRequestItem> Items { get; set; }
    }
}
