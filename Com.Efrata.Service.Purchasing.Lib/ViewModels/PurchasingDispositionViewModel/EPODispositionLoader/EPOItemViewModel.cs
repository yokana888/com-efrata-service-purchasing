using System;
using System.Collections.Generic;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel.EPODispositionLoader
{
    public class EPOItemViewModel : BaseViewModel
    {
        public string poNo { get; set; }
        public string prNo { get; set; }
        public long poId { get; set; }
        public long prId { get; set; }
        public CategoryViewModel category {get;set; }
        public bool ipoIsdeleted { get; set; }
        public UnitViewModel unit { get; set; }
        public List<EPODetailViewModel> details { get; set; }
    }
}
