using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel
{
    public class UnitPaymentOrderGenerateDataViewModel : BaseViewModel
    {
        public string UPONo { get; set; }
        public DateTimeOffset? UPODate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string UPORemark { get; set; }
        public string UseVat { get; set; }
        public string VatNo { get; set; }
        public double VatRate { get; set; }
        public DateTimeOffset? VatDate { get; set; }
        public string UseIncomeTax { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public string IncomeTaxNo { get; set; }
        public DateTimeOffset? IncomeTaxDate { get; set; }
        public string EPONO { get; set; }
        public string PRNo { get; set; }
        public string AccountNo { get; set; }
        public string IncludedPPN { get; set; }
        public string Printed { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double ReceiptQty { get; set; }
        public string UOMUnit { get; set; }
        public double PricePerDealUnit { get; set; }
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        public double PriceTotal { get; set; }
        public string URNNo { get; set; }
        public DateTimeOffset? URNDate { get; set; }
        public string UserCreated { get; set; }
        public string PaymentMethod { get; set; }
    }
}
