using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels
{
    public class GarmentPOMasterDistribution : BaseModel
    {
        public long DOId { get; set; }
        [MaxLength(25)]
        public string DONo { get; set; }

        public DateTimeOffset DODate { get; set; }

        public long SupplierId { get; set; }
        [MaxLength(100)]
        public string SupplierName { get; set; }

        public virtual ICollection<GarmentPOMasterDistributionItem> Items { get; set; }
    }
}
