using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel
{
    public class CurrencyViewModel
    {
        public string _id { get; set; }
        public string code { get; set; }
        public string symbol { get; set; }
        public double rate { get; set; }
        public string description { get; set; }
    }
}