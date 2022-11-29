using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel
{
    public class GarmentUnitExpenditureNoteItemViewModel : BaseViewModel
    {
        public long UENId { get; set; }
        public long UnitDOItemId { get; set; }
        public long URNItemId { get; set; }
        public long DODetailId { get; set; }
        public long EPOItemId { get; set; }
        public long POItemId { get; set; }
        public long PRItemId { get; set; }
        public string POSerialNumber { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public string RONo { get; set; }
        public decimal Quantity { get; set; }
        public long UomId { get; set; }
        public string UomUnit { get; set; }
        public double PricePerDealUnit { get; set; }
        public string FabricType { get; set; }
        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string DesignColor { get; set; }
        public bool IsSave { get; set; }
        public CurrencyViewModel DOCurrency { get; set; }
        public decimal Conversion { get; set; }
        public decimal BasicPrice { get; set; }
        public decimal ReturQuantity { get; set; }

        public string ItemStatus { get; set; }
        public string CustomsCategory { get; set; }
    }
}
