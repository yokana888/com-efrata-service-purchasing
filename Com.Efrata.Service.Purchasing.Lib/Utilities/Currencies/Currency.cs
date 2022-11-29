
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies
{
    public class Currency
    {
        public Currency()
        {
            Rate = 1;
        }

        public string UId { get; set; }

        public string Code { get; set; }

        public DateTime Date { get; set; }

        public double? Rate { get; set; }
    }
}
