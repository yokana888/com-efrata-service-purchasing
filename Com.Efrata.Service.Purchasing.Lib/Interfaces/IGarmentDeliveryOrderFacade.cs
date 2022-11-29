using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentDeliveryOrderFacade
    {
        Tuple<List<GarmentDeliveryOrder>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        ReadResponse<dynamic> ReadLoader(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]");
        GarmentDeliveryOrder ReadById(int id);
        Task<int> Create(GarmentDeliveryOrder m, string user, int clientTimeZoneOffset = 7);
        Task<int> Update(int id, GarmentDeliveryOrderViewModel vm, GarmentDeliveryOrder m, string user, int clientTimeZoneOffset = 7);

        Task<int> Delete(int id, string username);
        IQueryable<GarmentDeliveryOrder> ReadBySupplier(string Keyword = null, string Filter = "{}");
        IQueryable<GarmentDeliveryOrder> DOForCustoms(string Keyword = null, string Filter = "{}", string BillNo = null);
        int IsReceived(List<int> Id);
        ReadResponse<object> ReadForUnitReceiptNote(int Page = 1, int Size = 10, string Order = "{}", string Keyword = null, string Filter = "{}");

        ReadResponse<object> ReadForCorrectionNoteQuantity(int Page = 1, int Size = 10, string Order = "{}", string Keyword = null, string Filter = "{}");

        IQueryable<AccuracyOfArrivalReportViewModel> GetReportQuery(string category, DateTime? dateFrom, DateTime? dateTo, int offset);
        Tuple<List<AccuracyOfArrivalReportViewModel>, int> GetReportHeaderAccuracyofArrival(string category, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateExcelArrivalHeader(string category, DateTime? dateFrom, DateTime? dateTo, int offset);

        Tuple<List<AccuracyOfArrivalReportViewModel>, int> GetReportDetailAccuracyofArrival(string supplier, string category, DateTime? dateFrom, DateTime? dateTo, int offset);
        MemoryStream GenerateExcelArrivalDetail(string supplier, string category, DateTime? dateFrom, DateTime? dateTo, int offset);

        IQueryable<AccuracyOfArrivalReportViewModel> GetReportQuery2(DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod, int offset);
        Tuple<List<AccuracyOfArrivalReportViewModel>, int> GetReportHeaderAccuracyofDelivery(DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod, int offset);
        MemoryStream GenerateExcelDeliveryHeader(DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod, int offset);
        Tuple<List<AccuracyOfArrivalReportViewModel>, int> GetReportDetailAccuracyofDelivery(string supplier, DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod, int offset);
        MemoryStream GenerateExcelDeliveryDetail(string supplier, DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod, int offset);
        Tuple<List<GarmentDeliveryOrderReportViewModel>, int> GetReportDO(string no, string poEksNo, long supplierId, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelDO(string no, string poEksNo, long supplierId, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo, int offset);

        AccuracyOfArrivalReportHeaderResult GetAccuracyOfArrivalHeader(string category, DateTime? dateFrom, DateTime? dateTo, int offset);
        List<AccuracyOfArrivalReportDetail> GetAccuracyOfArrivalDetail(string supplierCode, string category, DateTime? dateFrom, DateTime? dateTo, int offset);
        List<GarmentDeliveryOrder> ReadForInternNote(List<long> deliveryOrderIds);
        List<GarmentDOUrnViewModel> GetDataDO(int id);
    }
}
