using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel
{
    public class GarmentUENViewModel
    {
        public long UENId { get; set; }
        public string UENNo { get; set; }
        public DateTimeOffset UENDate { get; set; }
        public string UnitRequestName { get; set; }
        public string UnitSenderName { get; set; }
        public string FabricType { get; set; }
        public string RONo { get; set; }
        public double Quntity { get; set; }
        public string UOMUnit { get; set; } 
    }
}
