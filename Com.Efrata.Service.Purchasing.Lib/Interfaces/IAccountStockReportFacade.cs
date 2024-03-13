using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IAccountingStockReportFacade
    {
        Tuple<List<AccountingStockReportViewModel>, int> GetStockReport(int offset, string unitcode, string planPo, string tipebarang, int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo);
        MemoryStream GenerateExcelAStockReport(string ctg, string categoryname, string planPo, string unitcode, string unitname, DateTime? datefrom, DateTime? dateto, int offset);
    }
}
