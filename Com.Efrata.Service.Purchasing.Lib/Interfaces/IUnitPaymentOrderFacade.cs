using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IUnitPaymentOrderFacade
    {
        Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> Read(List<long> poExtIds, int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        UnitPaymentOrder ReadById(int id);
        Task<int> Create(UnitPaymentOrder model, string username, bool isImport, int clientTimeZoneOffset = 7);
        Task<int> Update(int id, UnitPaymentOrder model, string user);
        Task<int> Delete(int id, string username);
        Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> ReadSpb(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> ReadSpbForVerification(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> ReadPositionFiltered(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        UnitReceiptNote GetUnitReceiptNote(long URNId);
        ExternalPurchaseOrder GetExternalPurchaseOrder(string EPONo);
        Tuple<List<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel>, int> GetReportAll(string unitId, string supplierId,string noSPB, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(string unitId, string supplierId,string noSPB, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateDataExcel(DateTime? dateFrom, DateTime? dateTo, int offset);
        List<UnitPaymentOrder> ReadByEPONo(string no);

        Tuple<List<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel>, int> GetReportTax(string supplierId, string taxno, DateTime? dateFrom, DateTime? dateTo, DateTime? taxdateFrom, DateTime? taxdateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelTax(string supplierId, string taxno, DateTime? dateFrom, DateTime? dateTo, DateTime? taxdateFrom, DateTime? taxdateTo, int offset);

    }
}
