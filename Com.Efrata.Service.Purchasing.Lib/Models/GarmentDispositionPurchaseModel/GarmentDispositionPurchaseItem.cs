using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel
{
    public class GarmentDispositionPurchaseItem : StandardEntity
    {
        public string EPONo { get; set; }
        public int EPOId { get; set; }
        public bool IsVAT { get; set; }

        public string VatId { get; set; }
        public string VatRate { get; set; }

        public double VATAmount { get; set; }
        public bool IsIncomeTax { get; set; }
        public double IncomeTaxAmount { get; set; }
        public int IncomeTaxId { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public bool IsDispositionCreated { get; set; }
        public bool IsDispositionPaid { get; set; }
        public int GarmentDispositionPurchaseId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        public double DispositionAmountPaid { get; set; }
        public double DispositionAmountCreated { get; set; }
        public double DispositionQuantityCreated { get; set; }
        public double DispositionQuantityPaid { get; set; }
        public double VerifiedAmount { get; set; }
        [ForeignKey("GarmentDispositionPurchaseId")]
        public virtual GarmentDispositionPurchase GarmentDispositionPurchase { get; set; }
        public virtual List<GarmentDispositionPurchaseDetail> GarmentDispositionPurchaseDetails { get; set; }

        /// <summary>
        /// use for update garment diposition only
        /// </summary>
        /// <param name="modelReplace"></param>
        public void UpdateModel(GarmentDispositionPurchaseItem modelReplace)
        {
            EPONo = modelReplace.EPONo;
            EPOId = modelReplace.EPOId;
            IsVAT = modelReplace.IsVAT;
            VATAmount = modelReplace.VATAmount;
            IsIncomeTax = modelReplace.IsIncomeTax;
            IncomeTaxAmount = modelReplace.IncomeTaxAmount;
            IncomeTaxId = modelReplace.IncomeTaxId;
            IncomeTaxName = modelReplace.IncomeTaxName;
            IncomeTaxRate = modelReplace.IncomeTaxRate;
            IsDispositionCreated = modelReplace.IsDispositionCreated;
            IsDispositionPaid = modelReplace.IsDispositionPaid;
            GarmentDispositionPurchaseId = modelReplace.GarmentDispositionPurchaseId;
            CurrencyId = modelReplace.CurrencyId;
            CurrencyCode = modelReplace.CurrencyCode;
            CurrencyRate = modelReplace.CurrencyRate;
            DispositionAmountPaid = modelReplace.DispositionAmountPaid;
            DispositionAmountCreated = modelReplace.DispositionAmountCreated;
            DispositionQuantityCreated = modelReplace.DispositionQuantityCreated;
            DispositionQuantityPaid = modelReplace.DispositionQuantityPaid;
        }
        public void SetAudit(GarmentDispositionPurchase modelReplace)
        {
            Active = modelReplace.Active;
            CreatedUtc = modelReplace.CreatedUtc;
            CreatedBy = modelReplace.CreatedBy;
            CreatedAgent = modelReplace.CreatedAgent;
            LastModifiedUtc = modelReplace.LastModifiedUtc;
            LastModifiedBy = modelReplace.LastModifiedBy;
            LastModifiedAgent = modelReplace.LastModifiedAgent;
            IsDeleted = modelReplace.IsDeleted;
            DeletedUtc = modelReplace.DeletedUtc;
            DeletedBy = modelReplace.DeletedBy;
            DeletedAgent = modelReplace.DeletedAgent;
        }
    }
}
