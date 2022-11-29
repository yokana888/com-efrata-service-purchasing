using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    public class GarmentInternalNoteFilterDto
    {
        public List<int> PositionIds { get; set; }

        public List<int> IncomeTaxId { get; set; }
        public List<string> CurrencyCode { get; set; }
        public int isPPHMenu { get; set; }
    }
}
