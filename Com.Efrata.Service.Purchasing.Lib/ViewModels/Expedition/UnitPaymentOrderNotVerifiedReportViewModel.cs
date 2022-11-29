using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class UnitPaymentOrderNotVerifiedReportViewModel : BaseViewModel
    {
        public string UnitPaymentOrderNo { get; set; }
        public DateTimeOffset? VerifyDate { get; set; }
        public string SupplierName { get; set; }
        public DateTimeOffset UPODate { get; set; }
        public string DivisionName { get; set; }
        public double TotalPaid { get; set; }
        public string Currency { get; set; }
        public string NotVerifiedReason { get; set; }
    }
}
