using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade
{
    public class CreditorAccountDto
    {
        public decimal DPP { get; set; }

        public decimal PPN { get; set; }

        public string InvoiceNo { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public bool SupplierIsImport { get; set; }

        public DateTimeOffset Date { get; set; }

        public string Code { get; set; }

        public string Currency { get; set; }

        public decimal DPPCurrency { get; set; }

        public decimal CurrencyRate { get; set; }

        public string PaymentDuration { get; set; }

        public string Products { get; set; }

        public int DivisionId { get; set; }

        public string DivisionCode { get; set; }

        public string DivisionName { get; set; }

        public int UnitId { get; set; }

        public string UnitCode { get; set; }

        public string UnitName { get; set; }

        public string ExternalPurchaseOrderNo { get; set; }

        public decimal VATAmount { get; set; }

        public decimal IncomeTaxAmount { get; set; }

        public string IncomeTaxNo { get; set; }
    }
}
