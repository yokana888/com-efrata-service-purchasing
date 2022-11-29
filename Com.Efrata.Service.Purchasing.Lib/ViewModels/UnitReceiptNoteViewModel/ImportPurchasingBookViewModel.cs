using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel
{
	public class ImportPurchasingBookViewModel : BaseViewModel
	{
		public string urnNo { get; set; }
		public DateTime receiptDate { get; set; }
		public string productName { get; set; }
		public string unitName { get; set; }
		public string categoryName { get; set; }
		public string PIBNo { get; set; }
		public decimal amount { get; set; }
		public decimal amountIDR { get; set; }
		public decimal rate { get; set; }
    }
}
