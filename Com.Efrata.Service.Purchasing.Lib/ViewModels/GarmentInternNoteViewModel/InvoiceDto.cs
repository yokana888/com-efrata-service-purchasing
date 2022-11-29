using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class InvoiceDto
    {
        public InvoiceDto(string invoiceNo, DateTimeOffset invoiceDate, string productNames, int categoryId, string categoryName, string paymentMethod, int invoiceId, string deliveryOrdersNo, string billsNo, string paymentBills, double amount, bool useVAT, bool isPayVAT, bool useIncomeTax, bool isPayTax, double incomeTaxRate, double correctionAmount)
        {
            DocumentNo = invoiceNo;
            Date = invoiceDate;
            ProductNames = productNames;
            Category = new CategoryDto(categoryId, categoryName);
            PaymentMethod = paymentMethod;
            Id = invoiceId;
            DeliveryOrdersNo = deliveryOrdersNo;
            BillsNo = billsNo;
            PaymentBills = paymentBills;

            UseVAT = useVAT;
            IsPayVAT = isPayVAT;
            UseIncomeTax = useIncomeTax;
            IsPayTax = isPayTax;
            IncomeTaxRate = incomeTaxRate;
            TotalAmount = amount;

            Amount = amount;

            if (useVAT && isPayVAT)
                Amount += amount * 0.1;

            if (useIncomeTax && isPayTax)
                Amount -= amount * (incomeTaxRate / 100);

            Amount += correctionAmount;
            CorrectionAmount = correctionAmount;
        }
        public InvoiceDto(string invoiceNo, DateTimeOffset invoiceDate, string productNames, int categoryId, string categoryName, string paymentMethod, int invoiceId, string deliveryOrdersNo, string billsNo, string paymentBills, double amount, bool useVAT, bool isPayVAT, bool useIncomeTax, bool isPayTax, double incomeTaxRate, double correctionAmount, List<DeliveryOrderDto> detailDO)
        {
            DocumentNo = invoiceNo;
            Date = invoiceDate;
            ProductNames = productNames;
            Category = new CategoryDto(categoryId, categoryName);
            PaymentMethod = paymentMethod;
            Id = invoiceId;
            DeliveryOrdersNo = deliveryOrdersNo;
            BillsNo = billsNo;
            PaymentBills = paymentBills;

            UseVAT = useVAT;
            IsPayVAT = isPayVAT;
            UseIncomeTax = useIncomeTax;
            IsPayTax = isPayTax;
            IncomeTaxRate = incomeTaxRate;
            TotalAmount = amount;

            Amount = amount;
            var correction = correctionAmount;

            if (useVAT && isPayVAT)
            {
                Amount += amount * 0.1;
                correction += correctionAmount * 0.1;
            }

            if (useIncomeTax && isPayTax)
            {
                Amount -= amount * (incomeTaxRate / 100);
                correction -= correctionAmount * (incomeTaxRate / 100);
            }

            Amount += correction;
            CorrectionAmount = correctionAmount;
            DetailDO = detailDO;
        }

        public string DocumentNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ProductNames { get; set; }
        public CategoryDto Category { get; set; }
        public string PaymentMethod { get; set; }
        public double Amount { get; set; }
        public int Id { get; set; }
        public string DeliveryOrdersNo { get; set; }
        public string BillsNo { get; private set; }
        public string PaymentBills { get; private set; }
        public bool UseVAT { get; private set; }
        public bool IsPayVAT { get; private set; }
        public bool UseIncomeTax { get; private set; }
        public bool IsPayTax { get; private set; }
        public double IncomeTaxRate { get; private set; }
        public double TotalAmount { get; private set; }
        public double CorrectionAmount { get; private set; }
        public List<DeliveryOrderDto> DetailDO { get; set; }
    }
}