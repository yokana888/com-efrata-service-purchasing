using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IBeacukaiNoFeature
    {
        List<BeacukaiNoFeatureViewModel> GetBeacukaiNo(string filter, string keyword);
    }
}
