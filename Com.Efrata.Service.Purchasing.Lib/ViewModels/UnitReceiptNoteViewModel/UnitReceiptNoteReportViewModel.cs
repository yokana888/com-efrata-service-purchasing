using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel
{
    public class UnitReceiptNoteReportViewModel : BaseViewModel
    {
        public string unit { get; set; }
        public string category { get; set; }
        public string prNo { get; set; }
        public string urnNo { get; set; }
        public string productName { get; set; }
        public string productCode { get; set; }
        public string supplier { get; set; }
        public DateTimeOffset receiptDate { get; set; }
        public double dealQuantity { get; set; }
        public string DealUom { get; set; }
        public double receiptQuantity { get; set; }
        public string receiptUom { get; set; }
        public double quantity { get; set; }
        public long epoDetailId { get; set; }

        public double pricePerDealUnit { get; set; }
        public double totalPrice  { get; set; }
    }
}
