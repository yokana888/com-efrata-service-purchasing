

using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports.GarmentReportCMTFacade;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentReportCMTFacade
    {
        Tuple<List<GarmentReportCMTViewModel>, int> GetReport( DateTime? dateFrom, DateTime? dateTo, int unit, int page, int size, string Order, int offset);
        ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int unitcode, int offset, string unitname);
    }
}
