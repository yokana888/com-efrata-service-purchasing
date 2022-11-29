using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class GarmentInternNoteItemViewModel : BaseViewModel
    {
        public GarmentInvoiceViewModel garmentInvoice { get; set; }
        public List<GarmentInternNoteDetailViewModel> details { get; set; }
    }
}
