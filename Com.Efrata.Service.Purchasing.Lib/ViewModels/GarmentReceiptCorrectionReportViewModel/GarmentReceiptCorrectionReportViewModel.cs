using Com.Efrata.Service.Purchasing.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionNoteViewModel
{
    public class GarmentReceiptCorrectionReportViewModel
    {
        public string CorrectionNo { get; set; }
        public DateTimeOffset? CorrectionDate { get; set; }
        public string CorrectionNote { get; set; }
        public string URNNo { get; set; }
        public string BillNo { get; set; }
        public string Supplier { get; set; }
        public long SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
        public string DONo { get; set; }
        public string POSerialNumber{ get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public string UomUnit { get; set; }
        public double Conversion { get; set; }
        public double SmallQuantity { get; set; }
        public string SmallUomUnit { get; set; }
    }
}
