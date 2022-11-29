using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Moonlay.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.Models.Expedition
{
    public class PurchasingDocumentExpedition : StandardEntity, IValidatableObject
    {
        public string UnitPaymentOrderNo { get; set; }
        public DateTimeOffset UPODate { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentMethod { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public int URNId { get; set; }
        public string URNNo { get; set; }
        public double IncomeTax { get; set; }
        public double Vat { get; set; }
        public string IncomeTaxId { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public string IncomeTaxBy { get; set; }
        public double TotalPaid { get; set; }
        public string Currency { get; set; }
        public ExpeditionPosition Position { get; set; }
        public string SendToVerificationDivisionBy { get; set; }
        public DateTimeOffset? SendToVerificationDivisionDate { get; set; }
        public string VerificationDivisionBy { get; set; }
        public DateTimeOffset? VerificationDivisionDate { get; set; }
        public string SendToCashierDivisionBy { get; set; }
        public DateTimeOffset? SendToCashierDivisionDate { get; set; }
        public string SendToAccountingDivisionBy { get; set; }
        public DateTimeOffset? SendToAccountingDivisionDate { get; set; }
        public string SendToPurchasingDivisionBy { get; set; }
        public DateTimeOffset? SendToPurchasingDivisionDate { get; set; }
        public string CashierDivisionBy { get; set; }
        public DateTimeOffset? CashierDivisionDate { get; set; }
        public string AccountingDivisionBy { get; set; }
        public DateTimeOffset? AccountingDivisionDate { get; set; }
        public string NotVerifiedReason { get; set; }
        public DateTimeOffset? VerifyDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsPaidPPH { get; set; }
        public string BankExpenditureNoteNo { get; set; }
        public DateTimeOffset? BankExpenditureNoteDate { get; set; }
        public string BankExpenditureNotePPHNo { get; set; }
        public DateTimeOffset? BankExpenditureNotePPHDate { get; set; }
        public virtual ICollection<PurchasingDocumentExpeditionItem> Items { get; set; }
        public double SupplierPayment { get; set; }
        public double AmountPaid { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
            //PurchasingDbContext dbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));

            //if (dbContext.PurchasingDocumentExpeditions.Count(p => p.IsDeleted.Equals(false) && p.Id != this.Id && p.UnitPaymentOrderNo.Equals(this.UnitPaymentOrderNo)) > 0) /* Unique */
            //{
            //    yield return new ValidationResult($"Unit Payment Order No {this.UnitPaymentOrderNo} is already exists", new List<string> { "UnitPaymentOrdersCollection" });
            //}
        }

        public object toViewModel(PurchasingDocumentExpedition model)
        {
            PurchasingDocumentExpedition data = new PurchasingDocumentExpedition();
            return data;
        }

    }
}
