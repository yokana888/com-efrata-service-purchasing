

using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports.GarmentRealizationCMTReportFacade;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentRealizationCMTReportFacade
    {
        Tuple<List<GarmentRealizationCMTReportViewModel>, int> GetReport( DateTime? dateFrom, DateTime? dateTo, string unit, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string unit, int offset, string unitname);
    }
}
