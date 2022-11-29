using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    public class GarmentDispositionNoteDto
    {
        public GarmentDispositionNoteDto(int id, string documentNo, DateTimeOffset date, DateTimeOffset dueDate, int supplierId, string supplierCode, string supplierName, double vATAmount, double currencyVATAmount, double incomeTaxAmount, double currencyIncomeTaxAmount, double totalPaid, double currencyTotalPaid, int currencyId, string currencyCode, double currencyRate, double dPPAmount, double currencyDPPAmount, List<GarmentDispositionNoteItemDto> items, string proformaNo,string category)
        {
            Id = id;
            DocumentNo = documentNo;
            Date = date;
            DueDate = dueDate;
            SupplierId = supplierId;
            SupplierCode = supplierCode;
            SupplierName = supplierName;
            VATAmount = vATAmount;
            CurrencyVATAmount = currencyVATAmount;
            IncomeTaxAmount = incomeTaxAmount;
            CurrencyIncomeTaxAmount = currencyIncomeTaxAmount;
            TotalPaid = totalPaid;
            CurrencyTotalPaid = currencyTotalPaid;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            DPPAmount = dPPAmount;
            CurrencyDPPAmount = currencyDPPAmount;
            Items = items;
            ProformaNo = proformaNo;
            Category = category;
        }

        public int Id { get; private set; }
        public string DocumentNo { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public DateTimeOffset DueDate { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierCode { get; private set; }
        public string SupplierName { get; private set; }
        public double VATAmount { get; private set; }
        public double CurrencyVATAmount { get; private set; }
        public double IncomeTaxAmount { get; private set; }
        public double CurrencyIncomeTaxAmount { get; private set; }
        public double TotalPaid { get; private set; }
        public double CurrencyTotalPaid { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public double DPPAmount { get; private set; }
        public double CurrencyDPPAmount { get; private set; }
        public string ProformaNo { get; private set; }
        public string Category { get; set; }
        public List<GarmentDispositionNoteItemDto> Items { get; private set; }
    }
}