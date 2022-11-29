using System;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPaymentReport
{
    public class GarmentDispositionPaymentReportDto
    {
        public GarmentDispositionPaymentReportDto(int dispositionId, string dispositionNoteNo, DateTimeOffset dispositionNoteDate, DateTimeOffset dispositionNoteDueDate, string proformaNo, int supplierId, string supplierCode, string supplierName, int currencyId, string currencyCode, double currencyRate, double dPPAmount, double vATAmount, double incomeTaxAmount, double othersExpenditureAmount, double totalAmount, int categoryId, string categoryCode, string categoryName, int externalPurchaseOrderId, string externalPurchaseOrderNo, double dispositionQuantity, int deliveryOrderId, string deliveryOrderNo, double deliveryOrderQuantity, string paymentBillsNo, string billsNo, int customsNoteId, string customsNoteNo, DateTimeOffset? customsNoteDate, int unitReceiptNoteId, string unitReceiptNoteNo, int internalNoteId, string internalNoteNo, DateTimeOffset? internalNoteDate, string dispositionCreatedBy)
        {
            DispositionId = dispositionId;
            DispositionNoteNo = dispositionNoteNo;
            DispositionNoteDate = dispositionNoteDate;
            DispositionNoteDueDate = dispositionNoteDueDate;
            ProformaNo = proformaNo;
            SupplierId = supplierId;
            SupplierCode = supplierCode;
            SupplierName = supplierName;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            DPPAmount = dPPAmount;
            VATAmount = vATAmount;
            IncomeTaxAmount = incomeTaxAmount;
            OthersExpenditureAmount = othersExpenditureAmount;
            TotalAmount = totalAmount;
            CategoryId = categoryId;
            CategoryCode = categoryCode;
            CategoryName = categoryName;
            ExternalPurchaseOrderId = externalPurchaseOrderId;
            ExternalPurchaseOrderNo = externalPurchaseOrderNo;
            DispositionQuantity = dispositionQuantity;
            DeliveryOrderId = deliveryOrderId;
            DeliveryOrderNo = deliveryOrderNo;
            DeliveryOrderQuantity = deliveryOrderQuantity;
            PaymentBillsNo = paymentBillsNo;
            BillsNo = billsNo;
            CustomsNoteId = customsNoteId;
            CustomsNoteNo = customsNoteNo;
            CustomsNoteDate = customsNoteDate;
            UnitReceiptNoteId = unitReceiptNoteId;
            UnitReceiptNoteNo = unitReceiptNoteNo;
            InternalNoteId = internalNoteId;
            InternalNoteNo = internalNoteNo;
            InternalNoteDate = internalNoteDate;
            DispositionCreatedBy = dispositionCreatedBy;
        }

        public int DispositionId { get; private set; }
        public string DispositionNoteNo { get; private set; }
        public DateTimeOffset DispositionNoteDate { get; private set; }
        public DateTimeOffset DispositionNoteDueDate { get; private set; }
        public string ProformaNo { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierCode { get; private set; }
        public string SupplierName { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public double DPPAmount { get; private set; }
        public double VATAmount { get; private set; }
        public double IncomeTaxAmount { get; private set; }
        public double OthersExpenditureAmount { get; private set; }
        public double TotalAmount { get; private set; }
        public int CategoryId { get; private set; }
        public string CategoryCode { get; private set; }
        public string CategoryName { get; private set; }
        public int ExternalPurchaseOrderId { get; private set; }
        public string ExternalPurchaseOrderNo { get; private set; }
        public double DispositionQuantity { get; private set; }
        public int DeliveryOrderId { get; private set; }
        public string DeliveryOrderNo { get; private set; }
        public double DeliveryOrderQuantity { get; private set; }
        public string PaymentBillsNo { get; private set; }
        public string BillsNo { get; private set; }
        public int CustomsNoteId { get; private set; }
        public string CustomsNoteNo { get; private set; }
        public DateTimeOffset? CustomsNoteDate { get; private set; }
        public int UnitReceiptNoteId { get; private set; }
        public string UnitReceiptNoteNo { get; private set; }
        public int InternalNoteId { get; private set; }
        public string InternalNoteNo { get; private set; }
        public DateTimeOffset? InternalNoteDate { get; private set; }
        public string DispositionCreatedBy { get; private set; }
    }
}