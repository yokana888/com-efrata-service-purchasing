using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase
{
    public class DispositionPurchaseReportTableDto
    {
        public string StaffName { get; set; }
        public string DispositionNo { get; set; }
        public DateTimeOffset DispositionDate { get; set; }
        public string SupplierCode { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Category { get; set; }
        public string PaymentType { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string CurrencyCode { get; set; }
        public double Nominal { get; set; }
    }
}
