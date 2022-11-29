using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionViewModels
{
    public class GarmentReceiptCorrectionItemViewModel : BaseViewModel
    {
        public bool IsSave { get; set; }
        public long URNItemId { get; set; }
        public long DODetailId { get; set; }
        public long EPOItemId { get; set; }
        public long POItemId { get; set; }
        public long PRItemId { get; set; }
        public string POSerialNumber { get; set; }
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }
        public string RONo { get; set; }
        public double Conversion { get; set; }
        public double CorrectionConversion { get; set; }
        public double Quantity { get; set; }
        public long UomId { get; set; }
        public string UomUnit { get; set; }
        public double CorrectionQuantity { get; set; }
        public double SmallQuantity { get; set; }
        public long SmallUomId { get; set; }
        public string SmallUomUnit { get; set; }
        public double PricePerDealUnit { get; set; }
        public string FabricType { get; set; }
        public string DesignColor { get; set; }
        public double QuantityCheck { get; set; }
        public double OrderQuantity { get; set; }
    }
}
