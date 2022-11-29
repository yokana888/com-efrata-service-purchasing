using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IRealizationBOMFacade
    {
        List<TraceableOutBeacukaiViewModel> GetQuery(string unitcode, DateTime? dateFrom, DateTime? dateTo);
        MemoryStream GetExcel(string unitcode, string unitname, DateTime? dateFrom, DateTime? dateTo);
        //List<RealizationBOMViewModel> GetQuery(string unitcode, DateTime? dateFrom, DateTime? dateTo);

    }
}
