using System;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class BudgetMasterSampleDisplayViewModel
    {
        public string RO_Number { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string Type { get; set; }
        public string Article { get; set; }
        public string ProductCode { get; set; }
        public string Remark { get; set; }
        public double Quantity { get; set; }
        public string Uom { get; set; }
        public double Price { get; set; }
        public string POSerialNumber { get; set; }
    }
}
