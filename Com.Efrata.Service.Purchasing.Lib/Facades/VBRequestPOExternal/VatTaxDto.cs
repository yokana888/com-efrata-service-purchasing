using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class VatTaxDto
    {
        public VatTaxDto(string vatTaxId, double vatTaxRate)
        {
            Id = vatTaxId;


            Rate = vatTaxRate;
        }

        //public VatTaxDto(string vatTaxId, double vatTaxRate)
        //{

        //    Id = vatTaxId;

        //    double.TryParse(vatTaxRate, out var rate);
        //    Rate = Convert.ToString(vatTaxRate);
        //}

        public VatTaxDto(string vatTaxId, string vatTaxRate)
        {
            Id = Convert.ToString(vatTaxId);

            double.TryParse(vatTaxRate, out var rate);
            Rate = Convert.ToDouble(vatTaxRate);

  
        }
        public string Id { get; private set; }
        public double Rate { get; private set; }

    }
}
