using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class UnitPaymentOrderPaidStatusViewModel
    {
        public string UnitPaymentOrderNo { get; set; }
        public DateTimeOffset UPODate { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string InvoiceNo { get; set; }
        public string SupplierName { get; set; }
        public string DivisionName { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public double DPP { get; set; }
        public double PPH { get; set; }
        public double PPN { get; set; }
        public double TotalPaid { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset? BankExpenditureNoteDate { get; set; }
        public string BankExpenditureNoteNo { get; set; }
        public string BankExpenditureNotePPHNo { get; set; }
        public DateTimeOffset? BankExpenditureNotePPHDate { get; set; }
        public string CategoryName { get; set; }
        public string statusppn { get; set; }
        public string statuspph { get; set; }
        public double Price { get; set; }
        public bool IsPaid { get; set; }
        public bool IsPaidPPH { get; set; }
        public string UnitName { get; set; }
        public bool UseVat { get; set; }
        public bool UseIncomeTax { get; set; }
        public int Position { get; set; }
        public string SupplierImport { get; set; }
    }
}
