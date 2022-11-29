using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel
{
    public class UnitPaymentOrderDetailViewModel : BaseViewModel
    {
        public long URNItemId { get; set; }
        public string EPONo { get; set; }
        public long EPODetailId { get; set; }
        public long POItemId { get; set; }
        public long PRId { get; set; }
        public string PRNo { get; set; }
        public long PRItemId { get; set; }

        public ProductViewModel product { get; set; }
        public double deliveredQuantity { get; set; }
        public UomViewModel deliveredUom { get; set; }
        public double pricePerDealUnit { get; set; }

        public double PriceTotal { get; set; }
        public double PricePerDealUnitCorrection { get; set; }
        public double PriceTotalCorrection { get; set; }
        public double QuantityCorrection { get; set; }
        public string ProductRemark { get; set; }

        //public string remark { get; set; }
    }
}
