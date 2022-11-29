using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseOrder;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel
{
    public class UnitReceiptNoteItemViewModel : BaseViewModel
    {
        public long epoDetailId { get; set; }
        public long epoId { get; set; }
        public string epoNo { get; set; }
        public string incomeTaxBy { get; set; }
        public long doDetailId { get; set; }
        public long poId { get; set; }
        public string prNo { get; set; }
        public long prId { get; set; }
        public long prItemId { get; set; }

        public string categoryCode { get; set; }
        public string uom { get; set; }
        public string uomId { get; set; }
        public bool isCorrection { get; set; }
        public string productRemark { get; set; }
        public double deliveredQuantity { get; set; }
        public double pricePerDealUnit { get; set; }
        public double currencyRate { get; set; }
        public ProductViewModel product { get; set; }
        public PurchaseOrderViewModel purchaseOrder { get; set; }
        public bool isPaid { get; set; }
    }
}