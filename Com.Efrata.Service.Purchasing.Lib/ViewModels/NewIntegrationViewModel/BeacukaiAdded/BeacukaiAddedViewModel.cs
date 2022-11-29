using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.BeacukaiAdded
{
    public class BeacukaiAddedViewModel
    {
        public DateTime BCDate { get; set; }
        public DateTime BonDate { get; set; }
        public string BCNo { get; set; }
        public string BonNo { get; set; }
        public double Quantity { get; set; }
        public string ItemCode { get; set; }
        public string BCType { get; set; }
    }

    public class BeacukaiAddedViewModelbyBCNo
    {
        public DateTime BCDate { get; set; }
        //public DateTime BonDate { get; set; }
        public string BCNo { get; set; }
        public string BonNo { get; set; }
        public double Quantity { get; set; }
        public string ItemCode { get; set; }
        public string BCType { get; set; }
    }
}
