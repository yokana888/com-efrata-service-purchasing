using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class PurchasingDocumentExpeditionReportViewModel
    {
        public string UnitPaymentOrderNo { get; set; }
        public DateTimeOffset? SendToVerificationDivisionDate { get; set; }
        public DateTimeOffset? VerificationDivisionDate { get; set; }
        public DateTimeOffset? VerifyDate { get; set; }
        public DateTimeOffset? SendDate { get; set; }
        public DateTimeOffset? CashierDivisionDate { get; set; }
        public DateTimeOffset? BankExpenditureNoteDate { get; set; }
        public string BankExpenditureNoteNo { get; set; }
        public DateTimeOffset? BankExpenditureNotePPHDate { get; set; }
        public string BankExpenditureNotePPHNo { get; set; }
    }
}
