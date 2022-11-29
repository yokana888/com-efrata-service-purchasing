using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel
{
    public class UnitPaymentPriceCorrectionNoteReportViewModel : BaseViewModel
    {
        public string upcNo { get; set; }
        public DateTimeOffset correctionDate { get; set; }
        public string upoNo { get; set; }
        public string epoNo { get; set; }
        public string prNo { get; set; }
        public string vatTaxCorrectionNo { get; set; }
        public DateTimeOffset? vatTaxCorrectionDate { get; set; }
        public string supplier { get; set; }
        public string correctionType { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public double quantity { get; set; }
        public string uom { get; set; }
        public double pricePerDealUnitAfter { get; set; }
        public double priceTotalAfter { get; set; }
        public string user { get; set; }
    }
}
