using Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IBankExpenditureNoteFacade
    {
        ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        ReadResponse<object> GetAllByPosition(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Task<int> Update(int id, BankExpenditureNoteModel model, IdentityService identityService);
        Task<BankExpenditureNoteModel> ReadById(int Id);
        Task<int> Create(BankExpenditureNoteModel model, IdentityService identityService);
        Task<int> Delete(int Id, IdentityService identityService);
        ReadResponse<object> GetReport(int Size, int Page, string DocumentNo, string UnitPaymentOrderNo, string InvoiceNo, string SupplierCode, string DivisionCode, string PaymentMethod, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int Offset);
        List<ExpenditureInfo> GetByPeriod(int month, int year, int timeoffset);
        Task<string> Posting(List<long> ids);
        MemoryStream GeneratePdfTemplate(BankExpenditureNoteModel model, int clientTimeZoneOffset);
    }
}
