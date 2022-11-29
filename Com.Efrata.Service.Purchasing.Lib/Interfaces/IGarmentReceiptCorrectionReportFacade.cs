using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionNoteViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentReceiptCorrectionReportFacade
    {
        // Task<GarmentReceiptCorrectionNoteWrapper> GetReport(string unit, string cf, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string order, int page, int size);
        MemoryStream GenerateExcel(string unit, string cf, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string order);

       // MemoryStream GenerateExcelEPOOverBudget(string epono, string unit, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        Tuple<List<GarmentReceiptCorrectionReportViewModel>, int> GetReport(string unit, string cf, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string order, int page, int size);

    }
}
