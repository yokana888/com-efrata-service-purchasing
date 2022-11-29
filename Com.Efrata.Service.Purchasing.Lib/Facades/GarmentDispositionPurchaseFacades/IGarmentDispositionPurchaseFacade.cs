using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPurchaseFacades
{
    public interface IGarmentDispositionPurchaseFacade
    {
        Task<int> Post(FormDto model);
        Task<int> Delete(int id);
        Task<int> Update(FormEditDto model);
        Task<FormDto> GetFormById(int id, bool isVerifiedAmountCalculated = false);
        Task<DispositionPurchaseIndexDto> GetAll(string keyword, int page, int size,string filter, string order);
        Task<List<FormDto>> ReadByDispositionNo(string dispositionNo, int page, int size);
        GarmentExternalPurchaseOrderViewModel ReadByEPOWithDisposition(int EPOid, int supplierId, string currencyCode);
        Task<int> SetIsPaidTrue(string dispositionNo, string user);
        Tuple<List<FormDto>, int, Dictionary<string, string>> Read(PurchasingGarmentExpeditionPosition position,int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}",int supplierId=0);
        DispositionPurchaseReportIndexDto GetReport(int supplierId, string username, DateTimeOffset? dateForm, DateTimeOffset? dateTo, int size = 10, int page = 1);
        Task<DispositionUserIndexDto> GetListUsers(string keyword);
        List<GarmentDispositionPurchase> GetGarmentDispositionPurchase();
    }
}
