using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel.Reports
{
    public class TotalPurchaseBySupplierViewModel : BaseViewModel
	{
		public string supplierName { get; set; }
		public string unitName { get; set; }
		public string categoryId { get; set; }
		public string categoryName { get; set; }
		public string divisionId { get; set; }
		public string divisionName { get; set; }
		public decimal amount { get; set; }
		public decimal amountIDR { get; set; }
		public decimal total { get; set; }
		public string currencyCode { get; set; }
	
	}
}
