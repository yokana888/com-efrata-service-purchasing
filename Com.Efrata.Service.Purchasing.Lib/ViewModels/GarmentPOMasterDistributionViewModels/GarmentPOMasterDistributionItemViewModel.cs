using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels
{
    public class GarmentPOMasterDistributionItemViewModel : BaseViewModel
    {
        public long DOItemId { get; set; }
        public long DODetailId { get; set; }

        public long SCId { get; set; }

        public double DOQuantity { get; set; }

        public List<GarmentPOMasterDistributionDetailViewModel> Details { get; set; }
    }
}
