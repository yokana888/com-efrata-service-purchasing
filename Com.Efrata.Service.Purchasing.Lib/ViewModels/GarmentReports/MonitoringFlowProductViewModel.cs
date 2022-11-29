using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class MonitoringFlowProductViewModel
    {
        public int count { get; set; }
        public string DONo { get; set; }
        public DateTime DODate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierType { get; set; }
        public string PO { get; set; }
        public string BCNo { get; set; }
        public string BCType { get; set; }
        public DateTime BCDate { get; set; }
        public double DOQty { get; set; }
        public string DOUom { get; set; }
        public string Urnno { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double ReceiptQty { get; set; }
        public string URNType { get; set; }
        public string ReceiptUom { get; set; }
        public string UENno { get; set; }
        public string ExpenditureType { get; set; }
        public DateTime ExpenditureDate { get; set; }
        public double ExpendQty { get; set; }
        public string ExpendUom { get; set; }
    }
}
