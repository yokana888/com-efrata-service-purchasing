using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IUnitPaymentPriceCorrectionNoteFacade
    {
        Tuple<List<UnitPaymentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        UnitPaymentCorrectionNote ReadById(int id);
        UnitReceiptNote GetUrn(string urnNo);
        SupplierViewModel GetSupplier(string supplierId);
        Task<int> Create(UnitPaymentCorrectionNote model, bool supplierImport, string username, int clientTimeZoneOffset = 7);
        IQueryable<UnitPaymentPriceCorrectionNoteReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset);
        Tuple<List<UnitPaymentPriceCorrectionNoteReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateDataExcel(DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
