using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase
{
    public class FormItemDto
    {
        public int Id { get; set; }
        public string EPONo { get; set; }
        public int EPOId { get; set; }
        public bool IsUseVat { get; set; }
        public bool IsPayVat { get; set; }
        public double VatValue { get; set; }
        public bool IsUseIncomeTax { get; set; }
        public bool IsPayIncomeTax { get; set; }
        public double IncomeTaxValue { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public int IncomeTaxId { get; set; }
        public int CurrencyId { get; set; }
        public double CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsDispositionCreated { get; set; }
        public bool IsDispositionPaid { get; set; }
        public int GarmentDispositionPurchaseId { get; set; }
        public double DispositionAmountPaid { get; set; }
        public double DispositionAmountCreated { get; set; }
        public double DispositionQuantityCreated { get; set; }
        public double DispositionQuantityPaid { get; set; }
        public double VerifiedAmount { get; set; }
        public List<FormDetailDto> Details { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAgent { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedUtc { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedAgent { get; set; }
        public string VatId { get; set; }
        public string VatRate { get; set; }
    }
}
