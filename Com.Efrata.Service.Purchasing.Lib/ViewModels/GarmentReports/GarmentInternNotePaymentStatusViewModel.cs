using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class GarmentInternNotePaymentStatusViewModel
    {
        public int count { get; set; }
        public string INNo { get; set; }
        public DateTimeOffset? INDate { get; set; }
        public string SuppCode { get; set; }
        public string SuppName { get; set; }
        public string PaymentMethod { get; set; }
        public string CurrCode { get; set; }
        public DateTimeOffset? PaymentDueDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset? InvoDate { get; set; }
        public string DoNo { get; set; }
        public DateTimeOffset? DoDate { get; set; }
        public double PriceTot { get; set; }
        public string BillNo { get; set; }
        public string PayBill { get; set; }
        public string NPN { get; set; }
        public DateTimeOffset? VatDate { get; set; }
        public string NPH { get; set; }
        public DateTimeOffset? IncomeTaxDate { get; set; }
        public string CorrNo { get; set; }
        public DateTimeOffset? CorDate { get; set; }
        public string CorrType { get; set; }
        public string Nomor { get; set; }
        public DateTimeOffset? Tgl { get; set; }
        public decimal Jumlah { get; set; }
        public long InvoiceId { get; set; }
    }
}
