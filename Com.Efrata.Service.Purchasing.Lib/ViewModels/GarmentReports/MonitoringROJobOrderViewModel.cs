using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class MonitoringROJobOrderViewModel
    {
        public string POSerialNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double BudgetQuantity { get; set; }
        public string UomPriceUnit { get; set; }
        public string Status { get; set; }
        public string POMasterNumber { get; set; }
        public List<MonitoringROJobOrderItemViewModel> Items { get; set; }
    }

    public class MonitoringROJobOrderItemViewModel
    {
        public string ROMaster { get; set; }
        public string POMaster { get; set; }
        public decimal DistributionQuantity { get; set; }
        public double Conversion { get; set; }
        public string UomCCUnit { get; set; }
        public string DONo { get; set; }
        public string SupplierName { get; set; }
        public string OverUsageReason { get; set; }
    }
}
