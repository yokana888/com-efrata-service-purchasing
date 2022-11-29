using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel
{
    public class GarmentPurchaseRequestItem : BaseModel
    {
        [MaxLength(255)]
        public string PO_SerialNumber { get; set; }

        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }

        public double Quantity { get; set; }
        public double BudgetPrice { get; set; }

        [MaxLength(255)]
        public string UomId { get; set; }
        [MaxLength(255)]
        public string UomUnit { get; set; }

        [MaxLength(255)]
        public string CategoryId { get; set; }
        [MaxLength(1000)]
        public string CategoryName { get; set; }

        public string ProductRemark { get; set; }

        public string Status { get; set; }

        public bool IsUsed { get; set; }

        public long PriceUomId { get; set; }
        [MaxLength(255)]
        public string PriceUomUnit { get; set; }
        public double PriceConversion { get; set; }

        public bool IsOpenPO { get; set; }
        [MaxLength(100)]
        public string OpenPOBy { get; set; }
        public DateTimeOffset OpenPODate { get; set; }

        public bool IsApprovedOpenPOMD { get; set; }
        [MaxLength(100)]
        public string ApprovedOpenPOMDBy { get; set; }
        public DateTimeOffset ApprovedOpenPOMDDate { get; set; }

        public bool IsApprovedOpenPOPurchasing { get; set; }
        [MaxLength(100)]
        public string ApprovedOpenPOPurchasingBy { get; set; }
        public DateTimeOffset ApprovedOpenPOPurchasingDate { get; set; }

        public bool IsApprovedOpenPOKadivMd { get; set; }
        [MaxLength(100)]
        public string ApprovedOpenPOKadivMdBy { get; set; }
        public DateTimeOffset ApprovedOpenPOKadivMdDate { get; set; }

        public virtual long GarmentPRId { get; set; }
        [ForeignKey("GarmentPRId")]
        public virtual GarmentPurchaseRequest GarmentPurchaseRequest { get; set; }
    }
}
