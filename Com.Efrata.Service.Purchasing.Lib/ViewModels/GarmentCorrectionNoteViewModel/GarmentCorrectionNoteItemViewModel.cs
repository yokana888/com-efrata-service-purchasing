using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel
{
    public class GarmentCorrectionNoteItemViewModel : BaseViewModel
    {
        public long DODetailId { get; set; }

        public long EPOId { get; set; }
        public string EPONo { get; set; }

        public long PRId { get; set; }
        public string PRNo { get; set; }

        public long POId { get; set; }
        public string POSerialNumber { get; set; }
        public string RONo { get; set; }

        public ProductViewModel Product { get; set; }

        public decimal Quantity { get; set; }
        public decimal QuantityCheck { get; set; }

        public UomViewModel Uom { get; set; }

        public decimal PricePerDealUnitBefore { get; set; }
        public decimal PricePerDealUnitAfter { get; set; }
        public decimal PriceTotalBefore { get; set; }
        public decimal PriceTotalAfter { get; set; }
    }
}
