using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PRMasterValidationReportViewModel
{
	public class PRMasterValidationReportViewModel
    {
        public int index { get; set; }
        public string RO_Number { get; set; }
        public string UnitName { get; set; }
        public DateTimeOffset PRDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public string SectionName { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string Article { get; set; }
        public string StaffName { get; set; }
        public string ValidatedKadiv { get; set; }
        public DateTimeOffset ValidatedDate { get; set; }
    }
}
