using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel
{
    public class GarmentInternalPurchaseOrderItemViewModel : BaseViewModel
    {
        public long GPRItemId { get; set; }
        public string PO_SerialNumber { get; set; }

        public ProductViewModel Product { get; set; }

        public double Quantity { get; set; }
        public double BudgetPrice { get; set; }
        public double RemainingBudget { get; set; }

        public UomViewModel Uom { get; set; }

        public CategoryViewModel Category { get; set; }

        public string ProductRemark { get; set; }
        public string Status { get; set; }
    }
}
