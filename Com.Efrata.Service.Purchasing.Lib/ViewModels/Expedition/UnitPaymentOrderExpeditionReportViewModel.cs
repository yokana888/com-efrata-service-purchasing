using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class UnitPaymentOrderExpeditionReportViewModel
    {
        public string No { get; set; }
        public DateTimeOffset? Date { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string InvoiceNo { get; set; }
        public NewSupplierViewModel Supplier { get; set; }
        public string Currency { get; set; }
        public double TotalDay { get; set; }
        public CategoryViewModel Category { get; set; }
        public UnitViewModel Unit { get; set; }
        public DivisionViewModel Division { get; set; }
        public ExpeditionPosition Position { get; set; }
        public DateTimeOffset? SendToVerificationDivisionDate { get; set; }
        public double DPP { get; set; }
        public double PPn { get; set; }
        public double PPh { get; set; }
        public double TotalTax { get; set; }
        public DateTimeOffset? VerificationDivisionDate { get; set; }
        public DateTimeOffset? VerifyDate { get; set; }
        public DateTimeOffset? SendDate { get; set; }
        public DateTimeOffset? CashierDivisionDate { get; set; }
        public string BankExpenditureNoteNo { get; set; }
        public DateTime? LastModifiedUtc { get; set; }
        public string VerifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string PaymentDueDays { get; set; }
        public string PaymentMethod { get; set; }
        public double PaymentNominal { get; set; }
        public double PaymentDifference { get; set; }
    }

    public class UnitPaymentOrderExpeditionReportWrapper
    {
        public int Total { get; set; }
        public List<UnitPaymentOrderExpeditionReportViewModel> Data { get; set; }
    }
}
