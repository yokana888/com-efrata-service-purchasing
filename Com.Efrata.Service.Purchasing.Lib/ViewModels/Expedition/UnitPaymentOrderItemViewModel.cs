using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class UnitPaymentOrderItemViewModel
    {
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string Uom { get; set; }
        public double Price { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string URNNo { get; set; }
        public int? URNId { get; set; }
    }
}
