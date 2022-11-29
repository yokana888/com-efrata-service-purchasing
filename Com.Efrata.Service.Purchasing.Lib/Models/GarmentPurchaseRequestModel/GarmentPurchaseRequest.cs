using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel
{
    public class GarmentPurchaseRequest : BaseModel
    {
        [MaxLength(255)]
        public string PRNo { get; set; }
        [MaxLength(255)]
        public string PRType { get; set; }
        [MaxLength(255)]
        public string RONo { get; set; }
        [MaxLength(255)]
        public string MDStaff { get; set; }

        public long SCId { get; set; }
        [MaxLength(255)]
        public string SCNo { get; set; }

        [MaxLength(100)]
        public string SectionName { get; set; }
        [MaxLength(100)]
        public string ApprovalPR { get; set; }

        [MaxLength(255)]
        public string BuyerId { get; set; }
        [MaxLength(255)]
        public string BuyerCode { get; set; }
        [MaxLength(1000)]
        public string BuyerName { get; set; }

        [MaxLength(255)]
        public string Article { get; set; }

        public DateTimeOffset Date { get; set; }
        public DateTimeOffset? ExpectedDeliveryDate { get; set; }
        public DateTimeOffset ShipmentDate { get; set; }

        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(1000)]
        public string UnitName { get; set; }

        public bool IsPosted { get; set; }
        public bool IsUsed { get; set; }
        public string Remark { get; set; }

        public bool IsValidated { get; set; }
        [MaxLength(50)]
        public string ValidatedBy { get; set; }
        public DateTimeOffset? ValidatedDate { get; set; }

        public bool IsValidatedMD1 { get; set; }
        [MaxLength(50)]
        public string ValidatedMD1By { get; set; }
        public DateTimeOffset ValidatedMD1Date { get; set; }

        public bool IsValidatedMD2 { get; set; }
        [MaxLength(50)]
        public string ValidatedMD2By { get; set; }
        public DateTimeOffset ValidatedMD2Date { get; set; }

        public bool IsValidatedPurchasing { get; set; }
        [MaxLength(50)]
        public string ValidatedPurchasingBy { get; set; }
        public DateTimeOffset ValidatedPurchasingDate { get; set; }

        public virtual ICollection<GarmentPurchaseRequestItem> Items { get; set; }
    }
}
