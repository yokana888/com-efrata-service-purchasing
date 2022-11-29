using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels
{
    public class GarmentUnitReceiptNoteItemViewModel : BaseViewModel
    {
        public long URNId { get; set; }

        public long DODetailId { get; set; }

        public long EPOItemId { get; set; }
        public string DRItemId { get; set; }

        public long PRId { get; set; }
        public string PRNo { get; set; }
        public long PRItemId { get; set; }

        public long POId { get; set; }
        public long POItemId { get; set; }
        public string POSerialNumber { get; set; }

        public GarmentProductViewModel Product { get; set; }

        public string RONo { get; set; }

        public decimal ReceiptQuantity { get; set; }

        public UomViewModel Uom { get; set; }

        public decimal PricePerDealUnit { get; set; }

        public string DesignColor { get; set; }

        public bool IsCorrection { get; set; }

        public decimal Conversion { get; set; }

        public decimal SmallQuantity { get; set; }

        public decimal ReceiptCorrection { get; set; }

        public decimal OrderQuantity { get; set; }

        public UomViewModel SmallUom { get; set; }

        public BuyerViewModel Buyer { get; set; }

        public string Article { get; set; }
        public decimal CorrectionConversion { get; set; }
        public double DOCurrencyRate { get; set; }
        public long ExpenditureItemId { get; set; }

        public long UENItemId { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public string CustomsCategory { get; set; }
    }
}
