using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseOrder
{
    public class PurchaseOrderViewModel
    {
        public CategoryViewModel category { get; set; }
        public bool useIncomeTax { get; set; }
    }
}