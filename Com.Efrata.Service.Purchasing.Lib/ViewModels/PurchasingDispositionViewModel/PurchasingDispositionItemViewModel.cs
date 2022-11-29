using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel
{
    public class PurchasingDispositionItemViewModel : BaseViewModel
    {
        public string UId { get; set; }
        public string EPONo { get; set; }
        public string EPOId { get; set; }
        public bool UseVat { get; set; }
        public bool UseIncomeTax { get; set; }
        public IncomeTaxViewModel IncomeTax { get; set; }
        public VatTaxViewModel vatTax { get; set; }

        public virtual List<PurchasingDispositionDetailViewModel> Details { get; set; }

    }
}
