using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDailyPurchasingReportViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentDailyPurchasingReportFacade
    {
        Tuple<List<GarmentDailyPurchasingReportViewModel>, int> GetGDailyPurchasingReport(string unitName, bool supplierType, string supplierName, DateTime? dateFrom, DateTime? dateTo, string jnsbc, int offset);
        MemoryStream GenerateExcelGDailyPurchasingReport(string unitName, bool supplierType, string supplierName, DateTime? dateFrom, DateTime? dateTo, string jnsbc, int offset);
    }
}
