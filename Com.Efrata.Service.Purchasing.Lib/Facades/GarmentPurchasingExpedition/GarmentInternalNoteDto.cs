using System;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    public class GarmentInternalNoteDto
    {
        public GarmentInternalNoteDto(int id, string documentNo, DateTimeOffset date, DateTimeOffset dueDate, int supplierId, string supplierName, double vat, double incomeTax, double totalPaid, int currencyId, string currencyCode, double amountDPP, double correctionAmount, string paymentType, string paymentMethod, int paymentDueDays, string invoicesNo)
        {
            Id = id;
            DocumentNo = documentNo;
            Date = date;
            DueDate = dueDate;
            SupplierId = supplierId;
            SupplierName = supplierName;
            VAT = vat;
            CorrectionAmount = correctionAmount;
            IncomeTax = incomeTax;
            TotalPaid = totalPaid;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            AmountDPP = amountDPP;
            PaymentType = paymentType;
            PaymentMethod = paymentMethod;
            PaymentDueDays = paymentDueDays;
            InvoicesNo = invoicesNo;
        }

        public GarmentInternalNoteDto(int id, string documentNo, DateTimeOffset date, DateTimeOffset dueDate, int supplierId, string supplierName, double vAT, double incomeTax, double totalPaid, int currencyId, string currencyCode, double amountDPP, string paymentType, string paymentMethod, int paymentDueDays, string invoicesNo, string productName, long productId, string productCategory,long invoiceId)
        {
            Id = id;
            DocumentNo = documentNo;
            Date = date;
            DueDate = dueDate;
            SupplierId = supplierId;
            SupplierName = supplierName;
            VAT = vAT;
            IncomeTax = incomeTax;
            TotalPaid = totalPaid;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            AmountDPP = amountDPP;
            PaymentType = paymentType;
            PaymentMethod = paymentMethod;
            PaymentDueDays = paymentDueDays;
            InvoicesNo = invoicesNo;
            ProductName = productName;
            ProductId = productId;
            ProductCategory = productCategory;
            InvoicesId = invoiceId;
        }

        public int Id { get; private set; }
        public string DocumentNo { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public DateTimeOffset DueDate { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierName { get; private set; }
        public double VAT { get; private set; }
        public double CorrectionAmount { get; private set; }
        public double IncomeTax { get; private set; }
        public double TotalPaid { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double AmountDPP { get; private set; }
        public string PaymentType { get; private set; }
        public string PaymentMethod { get; private set; }
        public int PaymentDueDays { get; private set; }
        public string InvoicesNo { get; private set; }
        public long InvoicesId { get; set; }
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public string ProductCategory { get; set; }

    }
}