using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.Expedition
{
    public class PurchasingDocumentExpeditionReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<PurchasingDocumentExpedition> dbSet;
        public PurchasingDocumentExpeditionReportFacade(PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = this.dbContext.Set<PurchasingDocumentExpedition>();
        }

        public List<PurchasingDocumentExpeditionReportViewModel> GetReport(List<string> unitPaymentOrders)
        {
            var data = this.dbSet
                .Select(s => new PurchasingDocumentExpedition
                {
                    UnitPaymentOrderNo = s.UnitPaymentOrderNo,
                    SendToVerificationDivisionDate = s.SendToVerificationDivisionDate,
                    VerificationDivisionDate = s.VerificationDivisionDate,
                    VerifyDate = s.VerifyDate,
                    SendToCashierDivisionDate = s.SendToCashierDivisionDate,
                    SendToAccountingDivisionDate = s.SendToAccountingDivisionDate,
                    SendToPurchasingDivisionDate = s.SendToPurchasingDivisionDate,
                    CashierDivisionDate = s.CashierDivisionDate,
                    BankExpenditureNoteDate = s.BankExpenditureNoteDate,
                    BankExpenditureNoteNo = s.BankExpenditureNoteNo,
                    BankExpenditureNotePPHDate = s.BankExpenditureNotePPHDate,
                    BankExpenditureNotePPHNo = s.BankExpenditureNotePPHNo,
                    Position = s.Position,
                })
                .Where(p => unitPaymentOrders.Contains(p.UnitPaymentOrderNo));

            List<PurchasingDocumentExpeditionReportViewModel> list = new List<PurchasingDocumentExpeditionReportViewModel>();

            foreach(PurchasingDocumentExpedition d in data)
            {
                PurchasingDocumentExpeditionReportViewModel item = new PurchasingDocumentExpeditionReportViewModel()
                {
                    SendToVerificationDivisionDate = d.SendToVerificationDivisionDate,
                    VerificationDivisionDate = d.VerificationDivisionDate,
                    VerifyDate = d.VerifyDate,
                    SendDate = (d.Position == ExpeditionPosition.CASHIER_DIVISION || d.Position == ExpeditionPosition.SEND_TO_CASHIER_DIVISION) ? d.SendToCashierDivisionDate :
                    (d.Position == ExpeditionPosition.FINANCE_DIVISION || d.Position == ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION) ? d.SendToAccountingDivisionDate:
                    (d.Position == ExpeditionPosition.SEND_TO_PURCHASING_DIVISION) ? d.SendToPurchasingDivisionDate : null,
                    CashierDivisionDate = d.CashierDivisionDate,
                    UnitPaymentOrderNo = d.UnitPaymentOrderNo,
                    BankExpenditureNoteDate = d.BankExpenditureNoteDate,
                    BankExpenditureNoteNo = d.BankExpenditureNoteNo,
                    BankExpenditureNotePPHDate = d.BankExpenditureNotePPHDate,
                    BankExpenditureNotePPHNo = d.BankExpenditureNotePPHNo
                };

                list.Add(item);
            }

            return list;
        }
    }
}
