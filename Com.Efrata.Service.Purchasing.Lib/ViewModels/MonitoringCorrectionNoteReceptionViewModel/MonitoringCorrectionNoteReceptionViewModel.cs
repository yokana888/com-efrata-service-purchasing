using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.MonitoringCorrectionNoteReceptionViewModel
{
	public class MonitoringCorrectionNoteReceptionViewModel
    {
        public int index { get; set; }
        public string CorrectionNo { get; set; }
        public DateTimeOffset CorrectionDate { get; set; }
        public string CorrectionType { get; set; }
        public decimal CorrectionQuantity { get; set; }
        public string CorrectionUOMUnit { get; set; }
        public decimal CorrectionAmount { get; set; }
        public string BillNo { get; set; }
        public string PaymentBill { get; set; }
        public DateTimeOffset BillDate { get; set; }
        public string CustomsType { get; set; }
        public string BeaCukaiNo { get; set; }
        public DateTimeOffset BeaCukaiDate { get; set; }
        public string CodeRequirement { get; set; }
        public string PaymentType { get; set; }
        public string BuyerName { get; set; }
        public string ProductType { get; set; }
        public string ProductFrom { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Article { get; set; }
        public string RONo { get; set; }
        public string DONo { get; set; }
        public DateTimeOffset ArrivalDate { get; set; }
        public string InvoiceNo { get; set; }
        public string IncomeTaxNo { get; set; }
        public DateTimeOffset IncomeTaxDate { get; set; }
        public string EPONo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public double DOQuantity { get; set; }
        public string UOMUnit { get; set; }
        public double PricePerDealUnit { get; set; }
        public double PriceTotal { get; set; }
        public double Conversion { get; set; }
        public double SmallQuantity { get; set; }
        public string SmallUOMUnit { get; set; }
        public string InternNo { get; set; }
        public DateTimeOffset INDate { get; set; }
        public string URNNo { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public string UnitName { get; set; }
        public decimal ReceiptQuantity { get; set; }
        public string URNUOMUnit { get; set; }
        public decimal URNPricePerDealUnit { get; set; }
        public decimal URNPriceTotal { get; set; }
        public decimal URNConversion { get; set; }
        public decimal URNSmallQuantity { get; set; }
        public string URNSmallUOMUnit { get; set; }
    }
}
