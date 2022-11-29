using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote
{
    public class UnitReceiptNoteViewModel 
    {
        public bool _deleted { get; set; }
        public SupplierViewModel supplier { get; set; }
        public string no { get; set; }
        public DateTimeOffset date { get; set; }
        public UnitViewModel unit { get; set; }
        public string pibNo { get; set; }
        public string incomeTaxNo { get; set; }
        public string doNo { get; set; }
        public List<UnitReceiptNoteItemViewModel> items { get; set; }

        public bool isStorage { get; set; }
        public string remark { get; set; }

        
    }
}