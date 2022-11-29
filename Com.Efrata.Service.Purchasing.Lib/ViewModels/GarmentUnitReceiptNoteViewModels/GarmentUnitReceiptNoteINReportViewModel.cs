using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels
{
    public class GarmentUnitReceiptNoteINReportViewModel
    {
        public string NoBUM { get; set; }
        public string NoPO { get; set; }
        public string NoSuratJalan { get; set; }
        public string UNit { get; set; }
        public DateTimeOffset TanggalMasuk { get; set; }
        public DateTime TanggalBuatBon { get; set; }
        public string Gudang { get; set; }
        public string Supplier { get; set; }
        public string AsalTerima { get; set; }
        public string NamaBarang { get; set; }
        public string KodeBarang { get; set; }
        public string Keterangan { get; set; }
        public string NoRO { get; set; }
        public double JumlahDiterima { get; set; }
        public string Satuan { get; set; }
        public double JumlahKecil { get; set; }
    }
}
