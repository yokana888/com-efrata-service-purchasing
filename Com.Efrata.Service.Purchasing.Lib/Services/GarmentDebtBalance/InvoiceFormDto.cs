using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance
{
    public class InvoiceFormDto
    {
        public InvoiceFormDto()
        {

        }

        public InvoiceFormDto(int invoiceId, DateTimeOffset invoiceDate, string invoiceNo, double dPPAmount, double currencyDPPAmount, double vatAmount, double incomeTaxAmount, bool isPayVAT, bool isPayIncomeTax, double currencyVATAmount, double currencyIncomeTaxAmount, string vatNo)
        {
            InvoiceId = invoiceId;
            InvoiceDate = invoiceDate;
            InvoiceNo = invoiceNo;
            DPPAmount = dPPAmount;
            CurrencyDPPAmount = currencyDPPAmount;
            VATAmount = vatAmount;
            IncomeTaxAmount = incomeTaxAmount;
            IsPayVAT = isPayVAT;
            IsPayIncomeTax = isPayIncomeTax;
            CurrencyVATAmount = currencyVATAmount;
            CurrencyIncomeTaxAmount = currencyIncomeTaxAmount;
            VATNo = vatNo;
        }

        public int InvoiceId { get; private set; }
        public DateTimeOffset InvoiceDate { get; private set; }
        public string InvoiceNo { get; private set; }
        public double DPPAmount { get; private set; }
        public double CurrencyDPPAmount { get; private set; }
        public double VATAmount { get; private set; }
        public double IncomeTaxAmount { get; private set; }
        public bool IsPayVAT { get; private set; }
        public bool IsPayIncomeTax { get; private set; }
        public double CurrencyVATAmount { get; private set; }
        public double CurrencyIncomeTaxAmount { get; private set; }
        public string VATNo { get; private set; }
    }
}
