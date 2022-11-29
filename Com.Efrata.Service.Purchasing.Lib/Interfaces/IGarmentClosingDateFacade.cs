using Com.Efrata.Service.Purchasing.Lib.Models.GarmentClosingDateModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentClosingDateFacade
    {
        Task<int> Create(GarmentClosingDate m, string user, int clientTimeZoneOffset = 7);
        Tuple<List<GarmentClosingDate>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
    }
}
