using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IDebtCardReportFacade
    {
        Tuple<List<GarmentDebtCardReportViewModel>, int> GetDebtCardReport(int month, int year, string suppliercode, string suppliername, string currencyCode, string paymentMethod, int offset);
        MemoryStream GenerateExcelCardReport(int month, int year, string suppliercode, string suppliername, string currencyCode, string currencyName, string paymentMethod, int offset);
    }
}
