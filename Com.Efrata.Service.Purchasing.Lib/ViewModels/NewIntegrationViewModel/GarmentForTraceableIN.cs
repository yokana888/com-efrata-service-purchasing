using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel
{
    public class GarmentForTraceableIN
    {
        public string RoJob { get;  set; }
        public string CutOutType { get;  set; }
        public double CutOutQuantity { get;  set; }
        public string FinishingInType { get;  set; }
        public string FinishingTo { get;  set; }
        public double FinishingOutQuantity { get;  set; }
        public string ExpenditureType { get;  set; }
        public string Invoice { get; set; }
        public double ExpenditureQuantity { get;  set; }
    }
}
