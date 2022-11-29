using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel
{
   public class UnitPaymentOrderReportViewModel : BaseViewModel
    {
        public string no { get; set; }
        public DateTimeOffset? tglspb { get; set; }
        public string nospb{ get; set; }
        public string namabrg { get; set; }
        public string satuan { get; set; }
        public double jumlah { get; set; }
        public double hrgsat { get; set; }
        public double jumlahhrg { get; set; }
        public double ppn{ get; set; }
        public double total{ get; set; }
        public double pph { get; set; }
        public DateTimeOffset? tglpr { get; set; }
        public string nopr { get; set; }
        public DateTimeOffset? tglbon { get; set; }
        public string nobon { get; set; }
        public DateTimeOffset? tglinv{ get; set; }
        public string noinv { get; set; }
        public DateTimeOffset? jt { get; set; }
        public string kodesupplier { get; set; }
        public string supplier { get; set; }
        public string unit { get; set; }
        public string div{ get; set; }
        public string adm { get; set; }
        public string term { get; set; }
        public string matauang { get; set; }
        public string kategori { get; set; }
        public double qtycorrection { get; set; }
        public double pricecorrection { get; set; }
        public double totalpricecorrection { get; set; }
    }
}
