using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel
{
    public class ProductViewModel
    {
        public string _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public double price { get; set; }

        public UomViewModel uom { get; set; }
    }
}
