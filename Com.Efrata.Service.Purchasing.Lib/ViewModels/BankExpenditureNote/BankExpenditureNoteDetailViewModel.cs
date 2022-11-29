using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.BankExpenditureNote
{
    public class BankExpenditureNoteDetailViewModel : BaseViewModel
    {
        public string UId { get; set; }
        public long UnitPaymentOrderId { get; set; }
        public string Currency { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string InvoiceNo { get; set; }
        public List<BankExpenditureNoteItemViewModel> Items { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public double TotalPaid { get; set; }
        public DateTimeOffset UPODate { get; set; }
        public string UnitPaymentOrderNo { get; set; }
        public double IncomeTax { get; set; }
        public double Vat { get; set; }
        public double AmountPaid { get; set; }
        public double SupplierPayment { get; set; }
    }
}
