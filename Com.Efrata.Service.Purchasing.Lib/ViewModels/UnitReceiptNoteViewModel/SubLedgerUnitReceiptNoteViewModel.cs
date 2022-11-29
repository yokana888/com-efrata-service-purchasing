using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel
{
    public class SubLedgerUnitReceiptNoteViewModel
    {
        public DateTimeOffset? URNDate { get; set; }
        public string URNNo { get; set; }
        public string Supplier { get; set; }
        public string UPONo { get; set; }
    }
}
