using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel
{
    public class UnitPaymentCorrectionNoteGenerateDataViewModel : BaseViewModel
    {
        public string UPCNo { get; set; }
        public DateTimeOffset UPCDate { get; set; }
        public string CorrectionType { get; set; }
        public string UPONo { get; set; }
        public string InvoiceCorrectionNo { get; set; }
        public DateTimeOffset InvoiceCorrectionDate { get; set; }
        public string VatTaxCorrectionNo { get; set; }
        public DateTimeOffset VatTaxCorrectionDate { get; set; }
        public string IncomeTaxCorrectionNo { get; set; }
        public DateTimeOffset IncomeTaxCorrectionDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string ReleaseOrderNoteNo { get; set; }
        public string UPCRemark { get; set; }
        public string EPONo { get; set; }
        public string PRNo { get; set; }
        public string AccountNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string UOMUnit { get; set; }
        public double PricePerDealUnit { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyRate { get; set; }
        public double PriceTotal { get; set; }
        public string URNNo { get; set; }
        public DateTimeOffset URNDate { get; set; }
        public string UserCreated { get; set; }
        public string UseVat { get; set; }
        public string UseIncomeTax { get; set; }
        public double VatRate { get; set; }
    }
}
