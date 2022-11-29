using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel
{
    public class CreditorAccountViewModel
    {
        public int CreditorAccountId { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string Code { get; set; }

        public string InvoiceNo { get; set; }

        public DateTimeOffset Date { get; set; }

        public int Id { get; set; }

        public double Mutation { get; set; }

        public string MemoNo { get; set; }

        public string UnitReceiptNoteNo { get; set; }

        public string ExternalPurchaseOrderNo { get; set; }
    }
}
