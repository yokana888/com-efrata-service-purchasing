using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class RealizationBOMViewModel
    {
        public string RO { get; set; }
        public string BCNoOut { get; set; }
        public string BCTypeOut { get; set; }
        public double QtyOut { get; set; }
        public string QtyNameOut { get; set; }
        public DateTime BCDateOut { get; set; }
        public string ComodityName { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }

        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public double ItemQty { get; set; }
        public string ItemQtyName { get; set; }
        public string DONo { get; set; }
        public string SupplierName { get; set; }
        public string BCNoIn { get; set; }
        public string BCTypeIn { get; set; }
        public DateTime BCDateIn { get; set; }

    }
}
