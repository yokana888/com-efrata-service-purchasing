using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel
{
    public class PurchasingDisposition : BaseModel
    {
        public string DispositionNo { get; set; }
        public string SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Bank { get; set; }
        public string ConfirmationOrderNo { get; set; }
        //public string InvoiceNo { get; set; }
        public string PaymentMethod { get; set; }
        public DateTimeOffset PaymentDueDate { get; set; }
        public string Calculation { get; set; }
        public string Remark { get; set; }
        public string ProformaNo { get; set; }
        //public string Investation { get; set; }
        public double Amount { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        public string CurrencyDescription { get; set; }
        public int Position { get; set; }
        public string IncomeTaxBy { get; set; }
        public double PaymentCorrection { get; set; }
        public double IncomeTaxValue { get; set; }
        public double VatValue { get; set; }
        public double DPP { get; set; }
        public bool IsPaid { get; set; }

        public virtual ICollection<PurchasingDispositionItem> Items { get; set; }
    }
}
