using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.BankExpenditureNote
{
    public class BankExpenditureNoteReportViewModel
    {
        public string DocumentNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
        public string DivisionName { get; set; }
        public string PaymentMethod { get; set; }
        public double DPP { get; set; }
        public double VAT { get; set; }
        public string Currency { get; set; }
        public string BankName { get; set; }
        public string UnitPaymentOrderNo { get; set; }
        public string InvoiceNumber { get; set; }
        public double TotalPaid { get; set; }
        public string SupplierCode { get; internal set; }
        public string DivisionCode { get; internal set; }
        public double TotalDPP { get; internal set; }
        public double TotalPPN { get; internal set; }
        public long Id { get; internal set; }
        public double DifferenceNominal { get; set; }
        public double AmountPaid { get; set; }
    }
}
