using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels
{
    public class GarmentPOMasterDistributionItem : BaseModel
    {
        public long POMasterDistributionId { get; set; }
        [ForeignKey("POMasterDistributionId")]
        public virtual GarmentPOMasterDistribution GarmentPOMasterDistribution { get; set; }

        public long DOItemId { get; set; }
        public long DODetailId { get; set; }

        public long SCId { get; set; }

        public virtual ICollection<GarmentPOMasterDistributionDetail> Details { get; set; }
    }
}
