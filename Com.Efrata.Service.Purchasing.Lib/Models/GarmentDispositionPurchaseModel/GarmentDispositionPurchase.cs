using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel
{
    public class GarmentDispositionPurchase : StandardEntity
    {
        public string DispositionNo { get; set; }
        public string Category { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public bool SupplierIsImport { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public DateTimeOffset CurrencyDate { get; set; }
        public string Bank { get; set; }
        public string ConfirmationOrderNo { get; set; }
        public string PaymentType { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string Description { get; set; }
        public string InvoiceProformaNo { get; set; }
        public double Dpp { get; set; }
        public double IncomeTax { get; set; }
        public double VAT { get; set; }
        public double OtherCost { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset VerifiedDateReceive { get; set; }
        public DateTimeOffset VerifiedDateSend { get; set; }
        public virtual List<GarmentDispositionPurchaseItem> GarmentDispositionPurchaseItems { get; set; }
        public PurchasingGarmentExpeditionPosition Position { get; set; }
        public bool IsPaymentPaid { get; set; }

        /// <summary>
        /// only use for update GarmentDisposiion
        /// </summary>
        /// <param name="modelReplace"></param>
        public void Update(GarmentDispositionPurchase modelReplace)
        {
            DispositionNo = modelReplace.DispositionNo;
            Category = modelReplace.Category;
            SupplierId = modelReplace.SupplierId;
            SupplierName = modelReplace.SupplierName;
            SupplierCode = modelReplace.SupplierCode;
            SupplierIsImport = modelReplace.SupplierIsImport;
            CurrencyId = modelReplace.CurrencyId;
            CurrencyName = modelReplace.CurrencyName;
            CurrencyDate = modelReplace.CurrencyDate;
            Bank = modelReplace.Bank;
            ConfirmationOrderNo = modelReplace.ConfirmationOrderNo;
            PaymentType = modelReplace.PaymentType;
            DueDate = modelReplace.DueDate;
            Description = modelReplace.Description;
            InvoiceProformaNo = modelReplace.InvoiceProformaNo;
            Dpp = modelReplace.Dpp;
            IncomeTax = modelReplace.IncomeTax;
            VAT = modelReplace.VAT;
            OtherCost = modelReplace.OtherCost;
            Amount = modelReplace.Amount;
            //VerifiedDateReceive = modelReplace.VerifiedDateReceive;
            //VerifiedDateSend = modelReplace.VerifiedDateSend;
            //GarmentDispositionPurchaseItems = modelReplace.GarmentDispositionPurchaseItems;
            //modelReplace.GarmentDispositionPurchaseItems.ForEach(t =>
            //{
            //    t.upda
            //});
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
