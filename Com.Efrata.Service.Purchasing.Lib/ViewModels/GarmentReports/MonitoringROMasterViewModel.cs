using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class MonitoringROMasterViewModel
    {
        public string POMaster { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string UomUnit { get; set; }
        public double DealQuantity { get; set; }
        public string DealUomUnit { get; set; }
        public List<MonitoringROMasterDeliveryOrderViewModel> DeliveryOrders { get; set; }
    }

    public class MonitoringROMasterDeliveryOrderViewModel
    {
        public string DONo { get; set; }
        public string SupplierName { get; set; }
        public double DOQuantity { get; set; }
        public List<MonitoringROMasterDistributionViewModel> Distributions { get; set; }
    }

    public class MonitoringROMasterDistributionViewModel
    {
        public string RONo { get; set; }
        public string POSerialNumber { get; set; }
        public decimal DistributionQuantity { get; set; }
        public string UomUnit { get; set; }
    }
}
