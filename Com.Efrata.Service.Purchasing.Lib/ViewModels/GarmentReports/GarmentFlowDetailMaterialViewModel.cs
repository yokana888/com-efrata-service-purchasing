using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class GarmentFlowDetailMaterialViewModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string POSerialNumber { get; set; }
        public string ProductRemark { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
        public string BuyerCode { get; set; }
        public string RONoDO { get; set; }
        public string ArticleDO { get; set; }
        public string UnitDOType { get; set; }
        public string UENNo { get; set; }
        public DateTime? ExpenditureDate { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public double Quantity { get; set; }
        public string UomUnit { get; set; }
        public double PricePerDealUnit { get; set; }
        public string DOCurrencyRate { get; set; }
        public string UnitDestination { get; set; }
        public decimal Total { get; set; }


    }
}
