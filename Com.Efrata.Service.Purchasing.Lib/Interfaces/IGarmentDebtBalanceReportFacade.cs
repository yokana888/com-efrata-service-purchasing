using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentDebtBalanceReportFacade
    {
        Tuple<List<GarmentDebtBalanceViewModel>, int> GetDebtBookReport(int month, int year, bool? suppliertype, string category);
        MemoryStream GenerateExcelDebtReport(int month, int year, bool? suppliertype, string category);
    }
}
