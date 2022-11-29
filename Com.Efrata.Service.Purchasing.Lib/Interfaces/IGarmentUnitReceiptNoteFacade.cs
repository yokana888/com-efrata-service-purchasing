using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentUnitReceiptNoteFacade
    {
        ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        GarmentUnitReceiptNoteViewModel ReadById(int id);
        MemoryStream GeneratePdf(GarmentUnitReceiptNoteViewModel garmentUnitReceiptNote);
        Task<int> Create(GarmentUnitReceiptNote garmentUnitReceiptNote);
        Task<int> Update(int id, GarmentUnitReceiptNote garmentUnitReceiptNote);
        Task<int> Delete(int id, string deletedReason);
        List<object> ReadForUnitDO(string Keyword = null, string Filter = "{}");
        List<object> ReadForUnitDOHeader(string Keyword = null, string Filter = "{}");
        List<object> ReadURNItem(string Keyword = null, string Filter = "{}");
        Tuple<List<FlowDetailPenerimaanViewModels>, int> GetReportFlow(DateTime? dateFrom, DateTime? dateTo, string unit, string category, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelLow(DateTime? dateFrom, DateTime? dateTo, string unit, string category, string categoryname, int offset, string unitname);
        MemoryStream GenerateExcelFlowForUnit(DateTime? dateFrom, DateTime? dateTo, string unit, string category, string categoryname, int offset, string unitname);
        List<object> ReadItemByRO(string Keyword = null, string Filter = "{}");
        Tuple<List<GarmentUnitReceiptNoteINReportViewModel>, int> GetReportIN(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelMonIN(DateTime? dateFrom, DateTime? dateTo, string category, int offset);

        GarmentDOItems ReadDOItemsByURNItemId(int id);
        int UrnDateRevise(List<long> ids, string user, DateTime reviseDate);
        List<object> ReadDataByDO(string Keyword = null, string Filter = "{}");
    }
}
