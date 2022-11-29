using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class InternalNoteDto
    {

        public InternalNoteDto(int internalNoteId, string internalNoteNo, DateTimeOffset internalNoteDate, DateTimeOffset internalNoteDueDate, int supplierId, string supplierName, string supplierCode, int currencyId, string currencyCode, double currencyRate, List<InternalNoteInvoiceDto> internalNoteInvoices)
        {
            Id = internalNoteId;
            DocumentNo = internalNoteNo;
            Date = internalNoteDate;
            DueDate = internalNoteDueDate;
            Supplier = new SupplierDto(supplierId, supplierName, supplierCode);
            DPP = internalNoteInvoices.Sum(internalNoteInvoice => internalNoteInvoice.Invoice.Amount);

            TotalAmount = internalNoteInvoices.Sum(element =>
            {
                var total = element.Invoice.TotalAmount;
                var correction = element.Invoice.CorrectionAmount;

                if (element.Invoice.UseVAT && element.Invoice.IsPayVAT)
                {
                    total += element.Invoice.TotalAmount * 0.1;
                    correction += element.Invoice.CorrectionAmount * 0.1;
                }
                
                if (element.Invoice.UseIncomeTax && element.Invoice.IsPayTax)
                {
                    total -= element.Invoice.TotalAmount * (element.Invoice.IncomeTaxRate / 100);
                    correction -= element.Invoice.CorrectionAmount * (element.Invoice.IncomeTaxRate / 100);
                }

                return total + correction;
            });

            VATAmount = internalNoteInvoices.Sum(element =>
            {
                var total = 0.0;
                var correction = 0.0;

                if (element.Invoice.UseVAT && element.Invoice.IsPayVAT)
                {
                    total += element.Invoice.TotalAmount * 0.1;
                    correction += element.Invoice.CorrectionAmount * 0.1;
                }

                return total + correction;
            });

            IncomeTaxAmount = internalNoteInvoices.Sum(element =>
            {
                var total = 0.0;
                var correction = 0.0;

                if (element.Invoice.UseIncomeTax && element.Invoice.IsPayTax)
                {
                    total += element.Invoice.TotalAmount * (element.Invoice.IncomeTaxRate / 100);
                    correction += element.Invoice.CorrectionAmount * (element.Invoice.IncomeTaxRate / 100);
                }

                return total + correction;
            });

            Currency = new CurrencyDto(currencyId, currencyCode, currencyRate);
            Items = internalNoteInvoices;
        }

        public int Id { get; set; }
        public string DocumentNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public SupplierDto Supplier { get; set; }
        public double VATAmount { get; set; }
        public double IncomeTaxAmount { get; set; }
        public double DPP { get; set; }
        public double TotalAmount { get; set; }
        public CurrencyDto Currency { get; set; }
        public List<InternalNoteInvoiceDto> Items { get; set; }
    }
}