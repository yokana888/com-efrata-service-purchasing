using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class InternalNoteInvoiceDto
    {
        public InternalNoteInvoiceDto(string invoiceNo, DateTimeOffset invoiceDate, string productNames, int categoryId, string categoryName, string paymentMethod, int invoiceId, string deliveryOrdersNo, string billsNo, string paymentBills, double amount, bool useVAT, bool isPayVAT, bool useIncomeTax, bool isPayTax, double incomeTaxRate, double correctionAmount,List<DeliveryOrderDto> detailDO)
        {
            Invoice = new InvoiceDto(invoiceNo, invoiceDate, productNames, categoryId, categoryName, paymentMethod, invoiceId, deliveryOrdersNo, billsNo, paymentBills, amount, useVAT, isPayVAT, useIncomeTax, isPayTax, incomeTaxRate, correctionAmount,detailDO);
        }

        public InternalNoteInvoiceDto(string invoiceNo, DateTimeOffset invoiceDate, string productNames, int categoryId, string categoryName, string paymentMethod, int invoiceId, string deliveryOrdersNo, string billsNo, string paymentBills, double amount, bool useVAT, bool isPayVAT, bool useIncomeTax, bool isPayTax, double incomeTaxRate, double correctionAmount)
        {
            Invoice = new InvoiceDto(invoiceNo, invoiceDate, productNames, categoryId, categoryName, paymentMethod, invoiceId, deliveryOrdersNo, billsNo, paymentBills, amount, useVAT, isPayVAT, useIncomeTax, isPayTax, incomeTaxRate, correctionAmount);
        }

        public InvoiceDto Invoice { get; set; }
    }
}