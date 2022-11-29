using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class DPPVATBankExpenditureNoteViewModel
    {
        public int ExpenditureId { get; set; }
        public string ExpenditureNoteNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string CategoryName { get; set; }
        public string PaymentMethod { get; set; }
        public double DPP { get; set; }
        public double VAT { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        public string BankName { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int InternalNoteId { get; set; }
        public string InternalNoteNo { get; set; }
        public double InternalNoteAmount { get; set; }
        public double OutstandingAmount { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public double InvoiceAmount { get; set; }
        public double PaidAmount { get; set; }
        public double Difference { get; set; }
        public string BillsNo { get; set; }
        public string PaymentBills { get; set; }
        public string DeliveryOrdersNo { get; set; }
        public object SupplierCode { get; set; }
        public double AmountDetail { get; set; }
    }
}
