using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade
{
    public interface IUnitReceiptNoteFacade
    {
        ReadResponse<UnitReceiptNote> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        ReadResponse<UnitReceiptNote> ReadByNoFiltered(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        UnitReceiptNote ReadById(int id);
        Task<int> Create(UnitReceiptNote m, string user);
        Task<int> Update(int id, UnitReceiptNote unitReceiptNote, string user);
        Task<string> Delete(int id, string user);
        ReadResponse<UnitReceiptNote> ReadBySupplierUnit(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        ReadResponse<UnitReceiptNoteReportViewModel> GetReport(string urnNo, string prNo, string unitId, string categoryId, string supplierId, string divisioId, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(string urnNo, string prNo, string unitId, string categoryId, string supplierId, string divisionId, DateTime? dateFrom, DateTime? dateTo, int offset);
        string GetPurchaseRequestCategoryCode(long prId);
        List<UnitReceiptNote> GetByListOfNo(List<string> urnNoList);
        Task<List<SubLedgerUnitReceiptNoteViewModel>> GetUnitReceiptNoteForSubledger(List<string> urnNoes);
        ReadResponse<UnitNoteSpbViewModel> GetSpbReport(string urnNo, string supplierName, string doNo, DateTime? dateFrom, DateTime? dateTo, int size, int page, string Order, int offset);
        MemoryStream GenerateExcelSpb(string urnNo, string supplierName, string doNo, DateTime? dateFrom, DateTime? dateTo, int offset);
        Task<dynamic> GetCreditorAccountDataByURNNo(string urnNo);
    }
}
