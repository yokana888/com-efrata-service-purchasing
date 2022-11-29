using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel
{
	public class GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel
	{
		public int no { get; set; }
		public string poExtNo { get; set; }
		public string poExtDate { get; set; }
		public string supplierCode { get; set; }
		public string supplierName { get; set; }
		public string prNo { get; set; }
		public string prDate { get; set; }
		public string prRefNo { get; set; }
		public string unit { get; set; }
		public string productName { get; set; }
		public string productCode { get; set; }
		public string productDesc { get; set; }
		public double quantity { get; set; }
		public string uom { get; set; }
		public double budgetPrice { get; set; }
		public double price { get; set; }
		public double totalBudgetPrice { get; set; }
		public double totalPrice { get; set; }
		public double overBudgetValue { get; set; }
		public double overBudgetValuePercentage { get; set; }
		public string status { get; set; }
		public string overBudgetRemark { get; set; }
	}
}
