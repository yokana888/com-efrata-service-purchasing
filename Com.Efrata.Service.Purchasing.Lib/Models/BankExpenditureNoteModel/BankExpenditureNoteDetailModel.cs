using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel
{
    public class BankExpenditureNoteDetailModel : BaseModel
    {
        public long UnitPaymentOrderId { get; set; }
        [MaxLength(255)]
        public string Currency { get; set; }
        [MaxLength(255)]
        public string CategoryCode { get; set; }
        [MaxLength(255)]
        public string CategoryName { get; set; }
        [MaxLength(255)]
        public string DivisionCode { get; set; }
        [MaxLength(255)]
        public string DivisionName { get; set; }
        public DateTimeOffset DueDate { get; set; }
        [MaxLength(255)]
        public string InvoiceNo { get; set; }
        public ICollection<BankExpenditureNoteItemModel> Items { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(255)]
        public string SupplierName { get; set; }
        public double TotalPaid { get; set; }
        public DateTimeOffset UPODate { get; set; }
        [MaxLength(255)]
        public string UnitPaymentOrderNo { get; set; }
        public double IncomeTax { get; set; }
        public double Vat { get; set; }
        public double AmountPaid { get; set; }
        public int UPOIndex { get; set; }
        public double SupplierPayment { get; set; }
        public virtual long BankExpenditureNoteId { get; set; }
        [ForeignKey("BankExpenditureNoteId")]
        public virtual BankExpenditureNoteModel BankExpenditureNote { get; set; }
    }
}