using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel
{
   public class UnitPaymentOrderTaxReportViewModel : BaseViewModel
    {
        public DateTimeOffset? tglppn { get; set; }
        public string noppn { get; set; }
        public string supplier { get; set; }
        public string nospb{ get; set; }
        public DateTimeOffset? tglspb { get; set; }
        public double amountspb { get; set; }
        public double amountppn { get; set; }
        public double amountpph { get; set; }        
    }
}
