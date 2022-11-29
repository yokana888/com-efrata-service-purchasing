using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentBC23ReportFacade
    {
        Tuple<List<GarmentBC23ReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string order, int offset);
        MemoryStream GetXLs(DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
