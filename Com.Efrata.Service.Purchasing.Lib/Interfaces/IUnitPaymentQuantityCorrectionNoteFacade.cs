using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IUnitPaymentQuantityCorrectionNoteFacade
    {
        Tuple<List<UnitPaymentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        UnitPaymentCorrectionNote ReadById(int id);
        Task<int> Create(UnitPaymentCorrectionNote model, string username, int clientTimeZoneOffset = 7);
        SupplierViewModel GetSupplier(string supplierId);

        UnitReceiptNote ReadByURNNo(string uRNno);
        //Task<int> Update(int id, UnitPaymentCorrectionNote model, string user);
        //Task<int> Delete(int id, string username);
        Tuple<List<UnitPaymentQuantityCorrectionNoteReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset);
        Task<CorrectionState> GetCorrectionStateByUnitPaymentOrderId(int unitPaymentOrderId);
    }
}
