using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel
{
    public class PurchasingDispositionDetailViewModel : BaseViewModel
    {
        public string UId { get; set; }
        public string EPODetailId { get; set; }
        public string PRId { get; set; }
        public string PRNo { get; set; }
        public ProductViewModel Product { get; set; }
        public double DealQuantity { get; set; }
        public UomViewModel DealUom{ get; set; }
        public double PaidQuantity { get; set; }
        public double PricePerDealUnit { get; set; }
        public double PriceTotal { get; set; }
        public double PaidPrice { get; set; }
        public double TotalPaidPrice { get; set; }
        public UnitViewModel Unit { get; set; }
    }
}
