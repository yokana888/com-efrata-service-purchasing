using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel
{
    public class SupplierViewModel
    {
        public string _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public bool import { get; set; }
        public string npwp { get; set; }
    }
}
