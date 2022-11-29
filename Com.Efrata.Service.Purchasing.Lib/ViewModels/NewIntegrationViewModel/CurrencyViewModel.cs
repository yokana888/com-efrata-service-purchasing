using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel
{
    public class CurrencyViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
