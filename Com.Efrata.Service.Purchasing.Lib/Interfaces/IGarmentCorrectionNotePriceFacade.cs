using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentCorrectionNotePriceFacade
    {
        Tuple<List<GarmentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        GarmentCorrectionNote ReadById(int id);
        Task<int> Create(GarmentCorrectionNote garmentCorrectionNote);
    }
}
