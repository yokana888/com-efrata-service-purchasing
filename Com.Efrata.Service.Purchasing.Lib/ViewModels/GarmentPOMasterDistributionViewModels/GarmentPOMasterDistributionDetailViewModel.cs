using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels
{
    public class GarmentPOMasterDistributionDetailViewModel : BaseViewModel
    {
        public long CostCalculationId { get; set; }
        public string RONo { get; set; }
        public string POSerialNumber { get; set; }

        public GarmentProductViewModel Product { get; set; }

        public decimal QuantityCC { get; set; }
        public UomViewModel UomCC { get; set; }

        public string OverUsageReason { get; set; }

        public double Conversion { get; set; }

        public decimal Quantity { get; set; }
        public UomViewModel Uom { get; set; }
    }
}
