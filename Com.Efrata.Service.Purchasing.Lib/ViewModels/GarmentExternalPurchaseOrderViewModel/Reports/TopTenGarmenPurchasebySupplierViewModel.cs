using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel.Reports
{
    public class TopTenGarmenPurchasebySupplierViewModel
    {

        public string SupplierName {get; set;}
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountIDR { get; set; }
        public string ProductName { get; set; }

    }
}
