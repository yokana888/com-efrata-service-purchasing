using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class UnitPaymentOrderViewModel
    {
        public int Id { get; set; }
        public int PurchasingDocumentExpeditionId { get; set; }
        public string No { get; set; }
        public DateTimeOffset UPODate { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentMethod { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public int URNId { get; set; }
        public string URNNo { get; set; }
        public double IncomeTax { get; set; }
        public double Vat { get; set; }
        public string IncomeTaxId { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public string IncomeTaxBy { get; set; }
        public double TotalPaid { get; set; }
        public string Currency { get; set; }
        public List<UnitPaymentOrderItemViewModel> Items { get; set; }
    }
}
