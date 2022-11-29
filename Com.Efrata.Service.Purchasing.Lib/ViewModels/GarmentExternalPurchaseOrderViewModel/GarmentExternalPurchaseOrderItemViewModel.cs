using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel
{
    public class GarmentExternalPurchaseOrderItemViewModel: BaseViewModel
    {
        public string UId { get; set; }
        public long GarmentEPOId { get; set; }
        public string PRNo { get; set; }
        public int PRId { get; set; }
        public string PONo { get; set; }
        public int POId { get; set; }
        public string PO_SerialNumber { get; set; }
        public string Article { get; set; }
        public string RONo { get; set; }
        public GarmentProductViewModel Product { get; set; }
        public double DefaultQuantity { get; set; }
        public UomViewModel DefaultUom { get; set; }
        public double DealQuantity { get; set; }
        public UomViewModel DealUom { get; set; }
        public double SmallQuantity { get; set; }
        public UomViewModel SmallUom { get; set; }
        public double Conversion { get; set; }
        public double PricePerDealUnit { get; set; }
        public double BudgetPrice { get; set; }
        public double UsedBudget { get; set; }
        public double DOQuantity { get; set; }
        public string Remark { get; set; }
        public string OverBudgetRemark { get; set; }

        public DateTimeOffset ShipmentDate { get; set; }
        public bool IsOverBudget { get; set; }

        public long? UENItemId { get; set; }
    }
}
