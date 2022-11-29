using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel
{
    public class GarmentPurchaseRequestItemViewModel : BaseViewModel
    {
        public string UId { get; set; }
        public string PO_SerialNumber { get; set; }

        public ProductViewModel Product { get; set; }

        public double Quantity { get; set; }
        public double BudgetPrice { get; set; }

        public UomViewModel Uom { get; set; }

        public CategoryViewModel Category { get; set; }

        public string ProductRemark { get; set; }

        public string Status { get; set; }

        public bool IsUsed { get; set; }

        public UomViewModel PriceUom { get; set; }
        public double PriceConversion { get; set; }

        /* For PR Master Validation */
        public GarmentProductViewModel Composition { get; set; }
        public GarmentProductViewModel Const { get; set; }
        public GarmentProductViewModel Yarn { get; set; }
        public GarmentProductViewModel Width { get; set; }
        /* For PR Master Validation */

        public bool IsOpenPO { get; set; }
        public string OpenPOBy { get; set; }
        public DateTimeOffset OpenPODate { get; set; }

        public bool IsApprovedOpenPOMD { get; set; }
        public string ApprovedOpenPOMDBy { get; set; }
        public DateTimeOffset ApprovedOpenPOMDDate { get; set; }

        public bool IsApprovedOpenPOPurchasing { get; set; }
        public string ApprovedOpenPOPurchasingBy { get; set; }
        public DateTimeOffset ApprovedOpenPOPurchasingDate { get; set; }

        public bool IsApprovedOpenPOKadivMd { get; set; }
        public string ApprovedOpenPOKadivMdBy { get; set; }
        public DateTimeOffset ApprovedOpenPOKadivMdDate { get; set; }
    }
}
