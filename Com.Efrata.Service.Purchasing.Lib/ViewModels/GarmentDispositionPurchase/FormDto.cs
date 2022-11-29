using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase
{
    public class FormDto
    {
        public int Id { get; set; }
        public string DispositionNo { get; set; }
        public string Category { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public bool SupplierIsImport { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string Bank { get; set; }
        public string ConfirmationOrderNo { get; set; }
        public string PaymentType { get; set; }
        public DateTimeOffset PaymentDueDate { get; set; }
        public string Remark { get; set; }
        public string ProformaNo { get; set; }
        public double DPP { get; set; }
        public double IncomeTaxValue { get; set; }
        public double VatValueView { get; set; }
        public double IncomeTaxValueView { get; set; }
        public double VatValue { get; set; }
        public double MiscAmount { get; set; }
        public double Amount { get; set; }
        public List<FormItemDto> Items { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public PurchasingGarmentExpeditionPosition Position { get; set; }
        public bool Active { get; set; }
        public string CreatedAgent { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedUtc { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedAgent { get; set; }

        public void FixingVatAndIncomeTax()
        {
            IncomeTaxValue = IncomeTaxValueView;
            VatValue = VatValueView;
        }
        public void FixingVatAndIncomeTaxView()
        {
            IncomeTaxValueView = IncomeTaxValue;
            VatValueView = VatValue;
        }
    }
}
