using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.Expedition
{
    public class PPHBankExpenditureNoteReportFacade : IPPHBankExpenditureNoteReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<PPHBankExpenditureNote> dbSet;
        public PPHBankExpenditureNoteReportFacade(PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = this.dbContext.Set<PPHBankExpenditureNote>();
        }

        public ReadResponse<object> GetReport(int Size, int Page, string No, string UnitPaymentOrderNo, string InvoiceNo, string SupplierCode, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int Offset)
        {
            IQueryable<PPHBankExpenditureNoteReportViewModel> Query;

            if (DateFrom == null || DateTo == null)
            {
                Query = (from a in dbContext.PPHBankExpenditureNotes
                         join b in dbContext.PPHBankExpenditureNoteItems on a.Id equals b.PPHBankExpenditureNoteId
                         join c in dbContext.PurchasingDocumentExpeditions on b.PurchasingDocumentExpeditionId equals c.Id
                         where a.IsDeleted == false && c.InvoiceNo == (InvoiceNo == null ? c.InvoiceNo : InvoiceNo)
                            && c.SupplierCode == (SupplierCode == null ? c.SupplierCode : SupplierCode)
                            && c.UnitPaymentOrderNo == (UnitPaymentOrderNo == null ? c.UnitPaymentOrderNo : UnitPaymentOrderNo)
                         where a.No == (No == null ? a.No : No)
                         orderby a.No
                         select new PPHBankExpenditureNoteReportViewModel
                         {
                             No = a.No,
                             Currency = a.Currency,
                             Date = a.Date,
                             Supplier = c.SupplierName,
                             Category = c.CategoryName,
                             SPB = b.UnitPaymentOrderNo,
                             Bank = string.Concat(a.BankAccountName, " - ", a.BankName, " - ", a.BankAccountNumber, " - ", a.Currency),
                             DPP = c.TotalPaid - c.Vat,
                             IncomeTax = c.IncomeTax,
                             InvoiceNo = c.InvoiceNo
                         }
                      );
            }
            else
            {
                Query = (from a in dbContext.PPHBankExpenditureNotes
                         join b in dbContext.PPHBankExpenditureNoteItems on a.Id equals b.PPHBankExpenditureNoteId
                         join c in dbContext.PurchasingDocumentExpeditions on b.PurchasingDocumentExpeditionId equals c.Id
                         where c.InvoiceNo == (InvoiceNo == null ? c.InvoiceNo : InvoiceNo)
                            && c.SupplierCode == (SupplierCode == null ? c.SupplierCode : SupplierCode)
                            && c.UnitPaymentOrderNo == (UnitPaymentOrderNo == null ? c.UnitPaymentOrderNo : UnitPaymentOrderNo)
                         where a.IsDeleted == false && a.No == (No == null ? a.No : No) && a.Date.AddHours(Offset).Date >= DateFrom.Value.Date && a.Date.AddHours(Offset).Date <= DateTo.Value.Date
                         orderby a.No
                         select new PPHBankExpenditureNoteReportViewModel
                         {
                             No = a.No,
                             Currency = a.Currency,
                             Date = a.Date,
                             Supplier = c.SupplierName,
                             Category = c.CategoryName,
                             SPB = b.UnitPaymentOrderNo,
                             Bank = string.Concat(a.BankAccountName, " - ", a.BankName, " - ", a.BankAccountNumber, " - ", a.Currency),
                             DPP = c.TotalPaid - c.Vat,
                             IncomeTax = c.IncomeTax,
                             InvoiceNo = c.InvoiceNo
                         }
                      );
            }

            Pageable<PPHBankExpenditureNoteReportViewModel> pageable = new Pageable<PPHBankExpenditureNoteReportViewModel>(Query, Page - 1, Size);
            List<object> data = pageable.Data.ToList<object>();
            
            return new ReadResponse<object>(data, pageable.TotalCount, new Dictionary<string, string>());
        }
    }
}
