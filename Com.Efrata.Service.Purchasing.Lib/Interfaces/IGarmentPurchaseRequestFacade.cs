using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentPurchaseRequestFacade
    {
		Tuple<List<GarmentPurchaseRequest>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        ReadResponse<dynamic> ReadDynamic(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]");

        GarmentPurchaseRequest ReadById(int id);
		GarmentPurchaseRequest ReadByRONo(string rono);
		Task<int> Create(GarmentPurchaseRequest m, string user, int clientTimeZoneOffset = 7);
		Task<int> Update(int id, GarmentPurchaseRequest m, string user, int clientTimeZoneOffset = 7);
		List<GarmentInternalPurchaseOrder> ReadByTags(string tags, DateTimeOffset shipmentDateFrom, DateTimeOffset shipmentDateTo);
		List<GarmentPurchaseRequest> ReadName(string Keyword = null, string Filter = "{}");
		Task<List<MonitoringPurchaseAllUserViewModel>> GetMonitoringPurchaseReport(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromEx, DateTime? dateToEx, int page, int size, string Order, int offset);
		Task<MemoryStream> GenerateExcelPurchase(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int page, int size, string Order, int offset);
		Task<List<MonitoringPurchaseAllUserViewModel>> GetMonitoringPurchaseByUserReport(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromEx, DateTime? dateToEx, int page, int size, string Order, int offset);
		Task<MemoryStream> GenerateExcelByUserPurchase(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int page, int size, string Order, int offset);
        Task<int> Delete(int id, string user);
        Task<int> PRPost(List<long> listId, string username);
        Task<int> PRUnpost(long id, string username);
        MemoryStream GeneratePdf(GarmentPurchaseRequestViewModel viewModel);
        GarmentPreSalesContractViewModel GetGarmentPreSalesContract(long Id);
        Task<int> PRApprove(long id, string username);
        Task<int> PRUnApprove(long id, string username);
        Task<int> Patch(long id, JsonPatchDocument<GarmentPurchaseRequest> jsonPatch, string user);
        List<GarmentInternalPurchaseOrder> ReadByTagsOptimized(string tags, DateTimeOffset shipmentDateFrom, DateTimeOffset shipmentDateTo);
    }
}
