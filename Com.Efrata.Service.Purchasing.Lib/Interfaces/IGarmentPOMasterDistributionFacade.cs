using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmentPOMasterDistributionFacade
    {
        ReadResponse<dynamic> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]");
        GarmentPOMasterDistribution ReadById(long id);
        Task<int> Create(GarmentPOMasterDistribution model);
        Task<int> Update(long id , GarmentPOMasterDistribution model);
        Task<int> Delete(long id);
        Dictionary<string, decimal> GetOthersQuantity(GarmentPOMasterDistributionViewModel garmentPOMasterDistributionViewModel);
    }
}
