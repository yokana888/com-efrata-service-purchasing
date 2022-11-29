using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels
{
    public class GarmentPOMasterDistributionDetail : BaseModel
    {
        public long POMasterDistributionItemId { get; set; }
        [ForeignKey("POMasterDistributionItemId")]
        public virtual GarmentPOMasterDistributionItem GarmentPOMasterDistributionItem { get; set; }

        public long CostCalculationId { get; set; }
        [MaxLength(25)]
        public string RONo { get; set; }
        [MaxLength(25)]
        public string POSerialNumber { get; set; }

        public long ProductId { get; set; }
        [MaxLength(25)]
        public string ProductCode { get; set; }

        public decimal QuantityCC { get; set; }
        public long UomCCId { get; set; }
        [MaxLength(50)]
        public string UomCCUnit { get; set; }

        [MaxLength(1000)]
        public string OverUsageReason { get; set; }

        public double Conversion { get; set; }

        public decimal Quantity { get; set; }
        public long UomId { get; set; }
        [MaxLength(50)]
        public string UomUnit { get; set; }
    }
}
