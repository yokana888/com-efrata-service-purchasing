using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class ROFeatureViewModel
    {
        public string KodeBarang { get; set; }
        public string NamaBarang { get; set; }
        public string PO { get; set; }
        public string Article { get; set; }
        public double QtyTerima { get; set; }
        public double QtyKeluar { get; set; }
        public string UomMasuk { get; set; }
        public string UomKeluar { get; set; }
        public string Unitcode { get; set; }
        public string RONo { get; set; }
        public ROItemViewModel items { get; set; }
    }

    public class ROItemViewModel
    {
        public List<RODetailMasukViewModel> Masuk { get; set; }
        public List<RODetailViewModel> Keluar { get; set; }
    }

    public class RODetailMasukViewModel
    {
        public DateTime ReceiptDate { get; set; }
        public string KodeBarang { get; set; }
        public string NamaBarang { get; set; }
        public string PO { get; set; }
        public string RONo { get; set; }
        public string NoBukti { get; set; }
        public string Unitcode { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
    }

    public class RODetailViewModel
    {
        public string Tipe { get; set; }
        public string UnitDONo { get; set; }
        public string RO { get; set; }
        public string RONo { get; set; }
        public double JumlahDO { get; set; }
        public string UomDO { get; set; }
        public string KodeBarang { get; set; }
        public string NamaBarang { get; set; }
        public string PO { get; set; }
        public string NoBukti { get; set; }
        public DateTime TanggalKeluar { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
    }

    public class ROFeatureTemp
    {
        public string KodeBarang { get; set; }
        public string NamaBarang { get; set; }
        public string PO { get; set; }
        public string Article { get; set; }
        public double QtyTerima { get; set; }
        public double QtyKeluar { get; set; }
        public string UomMasuk { get; set; }
        public string Unitcode { get; set; }
        public string UomKeluar { get; set; }
        public string RONo { get; set; }
    }
}
