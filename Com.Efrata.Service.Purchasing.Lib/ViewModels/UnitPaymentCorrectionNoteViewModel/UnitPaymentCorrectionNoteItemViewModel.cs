using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel
{
    public class UnitPaymentCorrectionNoteItemViewModel : BaseViewModel
    {
        public long uPODetailId { get; set; }
        public string uRNNo { get; set; }
        public string ePONo { get; set; }
        public long pRId { get; set; }
        public string pRNo { get; set; }
        public long pRDetailId { get; set; }
        public ProductViewModel product { get; set; }
        public double quantity { get; set; }
        public double quantityCheck { get; set; }
        public UomViewModel uom { get; set; }
        public double pricePerDealUnitBefore { get; set; }
        public double pricePerDealUnitAfter { get; set; }
        public double priceTotalBefore { get; set; }
        public double priceTotalAfter { get; set; }
        public CurrencyViewModel currency { get; set; }
        public virtual long uPCId { get; set; }
    }
}
