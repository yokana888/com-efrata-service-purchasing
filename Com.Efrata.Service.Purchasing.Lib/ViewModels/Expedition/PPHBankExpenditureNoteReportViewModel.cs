using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class PPHBankExpenditureNoteReportViewModel
    {
        public string No { get; set; }
        public DateTimeOffset Date { get; set; }
        public double DPP { get; set; }
        public double IncomeTax { get; set; }
        public string Currency { get; set; }
        public string Bank { get; set; }
        public string Supplier { get; set; }
        public string Category { get; set; }
        public string SPB { get; set; }
        public string InvoiceNo { get; set; }
    }
}
