using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel
{
  public  class UnitPaymentQuantityCorrectionNoteReportViewModel : BaseViewModel
    {
        public string upcNo { get; set; }
        public DateTimeOffset correctionDate { get; set; }
        public string upoNo { get; set; }
        public string epoNo { get; set; }
        public string prNo { get; set; }
        public string notaRetur { get; set; }
        public string vatTaxCorrectionNo { get; set; }
        public DateTimeOffset? vatTaxCorrectionDate { get; set; }
        public string supplier { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string unit { get; set; }
        public string category { get; set; }
        public string user { get; set; }
        public double jumlahKoreksi { get; set; }
        public string satuanKoreksi { get; set; }
        public double hargaSatuanKoreksi { get; set; }
        public double hargaTotalKoreksi { get; set; }
        public string jenisKoreksi { get; set; }
    }
}
