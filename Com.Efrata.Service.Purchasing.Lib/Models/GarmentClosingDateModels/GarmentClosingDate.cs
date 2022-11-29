using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentClosingDateModels
{
    public class GarmentClosingDate : BaseModel
    {
        public DateTimeOffset CloseDate { get; set; }
    }
}
