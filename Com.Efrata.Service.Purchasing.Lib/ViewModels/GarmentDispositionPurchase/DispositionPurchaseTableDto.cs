using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase
{
    public class DispositionPurchaseTableDto
    {
        public int Id { get; set; }
        public string DispositionNo { get; set; }
        public DateTimeOffset CreatedUtc { get; set; } //disposition date
        public string Category { get; set; }
        public string SupplierName { get; set; }//Supplier
        public DateTimeOffset DueDate { get; set; }
        public string CurrencyName { get; set; }//CurrencName
        public double AmountDisposition { get; set; }
        public DateTimeOffset VerifiedDateReceive { get; set; }
        public DateTimeOffset VerifiedDateSend { get; set; }
        public string CreatedBy { get; set; }//CurrencName
    }
}
