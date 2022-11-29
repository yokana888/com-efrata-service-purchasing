using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class GarmentRealizationCMTReportViewModel
    {
        public string InvoiceNo { get; set; }
        public string ExpenditureGoodNo { get; set; }
        public string Article { get; set; }
        public double UnitQty { get; set; }
        public decimal EGAmountIDR { get; set; }
        public string NoKasBank { get; set; }
        public string UENNo { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public decimal EAmountVLS { get; set; }
        public decimal EAmountIDR { get; set; }
        public string RONo { get; set; }
        public string URNNo { get; set; }
        public decimal UAmountVLS { get; set; }
        public decimal UAmountIDR { get; set; }
        public string ProductRemark2 { get; set; }
        public decimal ReceiptQuantity { get; set; }
        public string SupplierName { get; set; }
        public string BillNo { get; set; }
        public string PaymentBill { get; set; }
        public long InvoiceId { get; set; }
        public string DONo { get; set; }
        public decimal UENPrice { get; set; }
        public decimal DORate { get; set; }
        public int Count { get; set; }
    }
}
