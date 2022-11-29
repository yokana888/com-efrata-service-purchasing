using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public interface IVBRequestPOExternalService
    {
        List<POExternalDto> ReadPOExternal(string keyword, string division, string currencyCode);
        List<SPBDto> ReadSPB(string keyword, string division, List<long> epoIds, string currencyCode, string typePurchasing);
        int UpdateSPB(string division, int spbId);
        Task<int> AutoJournalVBRequest(VBFormDto form);
    }
}
