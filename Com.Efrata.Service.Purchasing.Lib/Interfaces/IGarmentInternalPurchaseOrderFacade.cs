using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentInternalPurchaseOrderFacade
    {
        Tuple<List<GarmentInternalPurchaseOrder>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        GarmentInternalPurchaseOrder ReadById(int id);
        bool CheckDuplicate(GarmentInternalPurchaseOrder model);
        Task<int> CreateMultiple(List<GarmentInternalPurchaseOrder> listModel, string user, int clientTimeZoneOffset = 7);
        Task<int> Split(int id, GarmentInternalPurchaseOrder model, string user, int clientTimeZoneOffset = 7);
        Task<int> Delete(int id, string username);
		List<GarmentInternalPurchaseOrder> ReadName(string Keyword = null, string Filter = "{}");

		List<GarmentInternalPurchaseOrder> ReadByTags(string category, string tags, DateTimeOffset shipmentDateFrom, DateTimeOffset shipmentDateTo, string username);

        Tuple<List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>, int> GetIPOEPODurationReport(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);

        MemoryStream GenerateExcelIPOEPODuration(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
