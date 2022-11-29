using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel.EPODispositionLoader
{
    public class EPODetailViewModel : BaseViewModel
    {

        public ProductViewModel product { get; set; }
        public long poItemId { get; set; }
        public long prItemId { get; set; }

        public double defaultQuantity { get; set; }
        public double dealQuantity { get; set; }

        public UomViewModel defaultUom { get; set; }
        public UomViewModel dealUom { get; set; }

        public double pricePerDealUnit { get; set; }
        public double productPrice { get; set; }
        public double priceBeforeTax { get; set; }
        public double conversion { get; set; }
        public bool includePpn { get; set; }
        public string productRemark { get; set; }
        public double? doQuantity { get; set; }
        public double dispositionQuantity { get; set; }
    }
}
