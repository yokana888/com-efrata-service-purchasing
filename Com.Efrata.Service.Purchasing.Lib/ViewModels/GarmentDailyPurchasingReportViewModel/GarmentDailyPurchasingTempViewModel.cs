using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDailyPurchasingReportViewModel
{
	public class GarmentDailyPurchasingTempViewModel
    {
        public string SuplName { get; set; }
        public string UnitName { get; set; }
        public string BCNo { get; set; }
        public string BCType { get; set; }
        public string NoteNo { get; set; }
        public string BonKecil { get; set; }
        public string DONo { get; set; }
        public string INNo { get; set; }
        public string ProductName { get; set; }
        public string JnsBrg { get; set; }
        public decimal Quantity { get; set; }
        public string Satuan { get; set; }
        public double Kurs { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
        public double AmountIDR { get; set; }
    }
}
