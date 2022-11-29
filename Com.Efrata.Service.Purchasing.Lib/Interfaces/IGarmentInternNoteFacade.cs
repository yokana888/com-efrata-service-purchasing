using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentInternNoteFacade
    {
        Tuple<List<GarmentInternNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        GarmentInternNote ReadById(int id);
        Task<int> Create(GarmentInternNote m, bool isImport, string user, int clientTimeZoneOffset = 7);
        Task<int> Update(int id, GarmentInternNote m, string user, int clientTimeZoneOffset = 7);
        int Delete(int id, string username);
        Tuple<List<GarmentInternNoteReportViewModel>, int> GetReport(string no, string supplierCode, string curencyCode, string invoiceNo, string npn, string doNo, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelIn(string no, string supplierCode, string curencyCode, string invoiceNo, string npn, string doNo, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo, int offset);
        List<GarmentInternalNoteDto> BankExpenditureReadInternalNotes(string currencyCode, int supplierId);
        List<GarmentInternalNoteDto> BankExpenditureReadInternalNotesOptimized(int currencyId, int supplierId);
        Task<int> BankExpenditureUpdateIsPaidInternalNoteAndInvoiceNote(bool dppVATIsPaid, int bankExpenditureNoteId, string bankExpenditureNoteNo, string internalNoteIds = "[]", string invoiceNoteIds = "[]");
    }
}
