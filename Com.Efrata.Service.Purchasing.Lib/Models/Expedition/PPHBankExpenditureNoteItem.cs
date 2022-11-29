using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.Expedition
{
    public class PPHBankExpenditureNoteItem : StandardEntity
    {
        public int PurchasingDocumentExpeditionId { get; set; }
        public string UnitPaymentOrderNo { get; set; }
        public virtual int PPHBankExpenditureNoteId { get; set; }
        public virtual PPHBankExpenditureNote PPHBankExpenditureNote { get; set; }
        public virtual PurchasingDocumentExpedition PurchasingDocumentExpedition { get; set; }
    }
}
