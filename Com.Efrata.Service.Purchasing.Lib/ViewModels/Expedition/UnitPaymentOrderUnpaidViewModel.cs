using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class UnitPaymentOrderUnpaidViewModel
    {
        public string UnitPaymentOrderNo { get; set; }
        public DateTimeOffset UPODate { get; set; }
        public string InvoiceNo { get; set; }
        public string SupplierName { get; set; }
        public string Currency { get; set; }
        public double IncomeTax { get; set; }
        public double DPPVat { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string UnitName { get; set; }
        public double TotalPaid { get; set; }
    }
}
