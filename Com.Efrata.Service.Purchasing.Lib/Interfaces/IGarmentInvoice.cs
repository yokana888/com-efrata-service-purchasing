using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentInvoice
    {
        Tuple<List<GarmentInvoice>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        GarmentInvoice ReadById(int id);
        Task<int> Create(GarmentInvoice m, string user, int clientTimeZoneOffset = 7);
        Task<int> Update(int id, GarmentInvoice m, string user, int clientTimeZoneOffset = 7);
        int Delete(int id, string username);
        GarmentInvoice ReadByDOId(int id);
        List<GarmentInvoice> ReadForInternNote(List<long> garmentInvoiceIds);
    }
}
