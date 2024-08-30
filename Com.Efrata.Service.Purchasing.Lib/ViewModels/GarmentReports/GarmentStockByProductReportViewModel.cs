using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class GarmentStockByProductReportViewModel
    {
        public string ProductCode { get; set; }
        public string RO { get; set; }
        public string PlanPo { get; set; }
        public string NoArticle { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public string Buyer { get; set; }
        //public decimal BeginningBalanceQty { get; set; }
        //public string BeginningBalanceUom { get; set; }
        public decimal ReceiptCorrectionQty { get; set; }
        public decimal ReceiptQty { get; set; }
        public string ReceiptUom { get; set; }
        public double ExpendQty { get; set; }
        public string ExpandUom { get; set; }
        public decimal EndingBalanceQty { get; set; }
        public string EndingUom { get; set; }
        public string PaymentMethod { get; set; }
        public string ProductType { get; set; }
        public string Color { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string Index { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime ExpendDate { get; set; }


    }

    public class GarmentStockByProductReportViewModelTemp
    {
        public string ProductCode { get; set; }
        public string RO { get; set; }
        public string PlanPo { get; set; }
        public string NoArticle { get; set; }
        public string ProductName { get; set; }
        public string Buyer { get; set; }
        //public decimal BeginningBalanceQty { get; set; }
        //public string BeginningBalanceUom { get; set; }
        public decimal ReceiptCorrectionQty { get; set; }
        public decimal ReceiptQty { get; set; }
        public string ReceiptUom { get; set; }
        public decimal ExpendQty { get; set; }
        public string ExpandUom { get; set; }
        public decimal EndingBalanceQty { get; set; }
        public string EndingUom { get; set; }
        public string PaymentMethod { get; set; }
        public string ProductType { get; set; }
        public string Color { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string Index { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime ExpendDate { get; set; }
        public string ProductRemark { get; set; }
    }
}
