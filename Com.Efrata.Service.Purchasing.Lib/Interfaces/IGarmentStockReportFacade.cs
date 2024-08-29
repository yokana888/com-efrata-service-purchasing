using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentStockReportFacade
    {
        Tuple<List<GarmentStockReportViewModel>, int> GetStockReport(int offset, string unitcode, string planPo, string tipebarang, int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo);
        MemoryStream GenerateExcelStockReport(string ctg, string categoryname, string unitname, string unitcode, string planPo, DateTime? datefrom, DateTime? dateto, int offset);
        Tuple<List<GarmentStockByProductReportViewModel>, int> GetStockByProduct(int offset, string productCode, int page, int size, string Order);
    }
}
