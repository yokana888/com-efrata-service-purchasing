using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel
{
    public class MonitoringOutViewModel
    {
        public string UENNo { get; set; }
        public string PONo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string ExTo { get; set; }
        public string Storage { get; set; }
        public double Quantity { get; set; }
        public string UnitQtyName { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset ExDate { get; set; }
    }
}
