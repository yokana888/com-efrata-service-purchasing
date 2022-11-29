using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentReturnCorrectionNoteFacade
    {
        Tuple<List<GarmentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        GarmentCorrectionNote ReadById(int id);
        Task<int> Create(GarmentCorrectionNote garmentCorrectionNote, bool isImport, string user, int clientTimeZoneOffset = 7);
        SupplierViewModel GetSupplier(long supplierId);
    }
}
