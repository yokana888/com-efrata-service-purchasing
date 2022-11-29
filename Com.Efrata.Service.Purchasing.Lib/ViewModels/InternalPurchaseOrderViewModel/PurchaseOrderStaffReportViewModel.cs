using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel
{
    public class PurchaseOrderStaffReportViewModel : BaseViewModel
    {
        public string user { get; set; }
        public string divisi { get; set; }
        public string unit { get; set; }
        public double selisih { get; set; }
        public double selisih2 { get; set; }
        public string nmbarang { get; set; }
        public string nmsupp { get; set; }
        public string nopr { get; set; }
        public double jumpr { get; set; }
        public DateTimeOffset tgltarget { get; set; }
        public DateTimeOffset tgldatang { get; set; }
        public DateTimeOffset tglpoint { get; set; }
        public DateTimeOffset tglpoeks { get; set; }
        public DateTimeOffset tgpr { get; set; }
      
    }
}
