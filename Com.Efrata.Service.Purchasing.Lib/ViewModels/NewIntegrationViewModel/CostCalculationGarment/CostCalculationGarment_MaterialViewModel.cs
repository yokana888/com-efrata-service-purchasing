using Com.Efrata.Service.Purchasing.Lib.Utilities;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.CostCalculationGarment
{
    public class CostCalculationGarment_MaterialViewModel : BaseViewModel
    {
        public string PO_SerialNumber { get; set; }
        public string POMaster { get; set; }
        public string Description { get; set; }
        public GarmentProductViewModel Product { get; set; }
        public double BudgetQuantity { get; set; }
        public UomViewModel UOMPrice { get; set; }
        public bool? IsPRMaster { get; set; }
    }
}
