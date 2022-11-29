using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentTopTenPurchaseSupplier
    {
        List<TopTenGarmenPurchasebySupplierViewModel> GetTopTenGarmentPurchaseSupplierReport(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateExcelTopTenGarmentPurchaseSupplier(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
