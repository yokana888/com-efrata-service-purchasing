using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel
{
    public class GarmentInternalPurchaseOrderItem : StandardEntity<long>
    {
        public long GPRItemId { get; set; }
        [MaxLength(1000)]
        public string PO_SerialNumber { get; set; }

        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(1000)]
        public string ProductName { get; set; }

        public double Quantity { get; set; }
        public double BudgetPrice { get; set; }
        public double RemainingBudget { get; set; }

        [MaxLength(255)]
        public string UomId { get; set; }
        [MaxLength(1000)]
        public string UomUnit { get; set; }

        [MaxLength(255)]
        public string CategoryId { get; set; }
        [MaxLength(1000)]
        public string CategoryName { get; set; }

        public string ProductRemark { get; set; }
        [MaxLength(255)]
        public string Status { get; set; }

        public virtual long GPOId { get; set; }
        [ForeignKey("GPOId")]
        public virtual GarmentInternalPurchaseOrder GarmentInternalPurchaseOrder { get; set; }

    }
}
