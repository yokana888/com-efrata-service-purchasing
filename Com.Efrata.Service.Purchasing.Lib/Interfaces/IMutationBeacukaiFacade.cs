using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IMutationBeacukaiFacade
    {
        Tuple<List<MutationBBCentralViewModel>, int> GetReportBBCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateExcelBBCentral (DateTime? dateFrom, DateTime? dateTo, int offset);
        Tuple<List<MutationBPCentralViewModel>, int> GetReportBPCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateExcelBPCentral (DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
