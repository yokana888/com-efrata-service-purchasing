using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel.Reports
{
    public class TotalGarmentPurchaseBySupplierViewModel : BaseViewModel
	{
		public string SupplierName { get; set; }
		public string UnitName { get; set; }
        public string PaymentMethod { get; set; }
		public string CategoryName { get; set; }
        public decimal Quantity { get; set; }
        public string UOMUnit { get; set; }
        public decimal SmallQty { get; set; }
        public string SmallUom { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountIDR { get; set; }
    }
}
