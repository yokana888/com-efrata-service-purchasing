using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using iTextSharp.text;
using System;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport
{
    public class ReportIndexDto
    {
        public ReportIndexDto(GarmentDeliveryOrder garmentDeliveryOrders, GarmentBeacukaiItem deliveryOrderCustoms, GarmentDeliveryOrderItem deliveryOrderItems, GarmentInvoiceItem deliveryOrderInvoiceItems, GarmentInvoice deliveryOrderInvoices, GarmentExternalPurchaseOrder deliveryOrderExternalPurchaseOrders, GarmentInternNoteDetail deliveryOrderInternalNoteDetails, GarmentInternNoteItem deliveryOrderInternalNoteItems, GarmentInternNote deliveryOrderInternalNotes,GarmentDeliveryOrderDetail garmentDeliveryOrderDetail)
        {
            if (deliveryOrderCustoms != null)
            {
                CustomsArrivalDate = deliveryOrderCustoms.ArrivalDate;
            }

            if (deliveryOrderExternalPurchaseOrders != null)
            {
                SupplierId = deliveryOrderExternalPurchaseOrders.SupplierId;
                SupplierName = deliveryOrderExternalPurchaseOrders.SupplierName;
                IsImportSupplier = deliveryOrderExternalPurchaseOrders.SupplierImport;
                AccountingCategoryName = deliveryOrderExternalPurchaseOrders.Category;
                PurchasingCategoryName = deliveryOrderExternalPurchaseOrders.Category;

            }

            var dppAmount = 0.0;
            if (deliveryOrderInternalNoteDetails != null)
            {
                ProductName = deliveryOrderInternalNoteDetails.ProductName;
                InternalNoteQuantity = deliveryOrderInternalNoteDetails.Quantity;
                dppAmount = deliveryOrderInternalNoteDetails.PriceTotal;
                Total = dppAmount;
            }

            if (garmentDeliveryOrders != null)
            {
                GarmentDeliveryOrderId = (int)garmentDeliveryOrders.Id;
                GarmentDeliveryOrderNo = garmentDeliveryOrders.DONo;
                BillNo = garmentDeliveryOrders.BillNo;
                PaymentBill = garmentDeliveryOrders.PaymentBill;
            }

            if (deliveryOrderInvoices != null)
            {
                InvoiceId = (int)deliveryOrderInvoices.Id;
                InvoiceNo = deliveryOrderInvoices.InvoiceNo;
                VATNo = deliveryOrderInvoices.VatNo;

                var vatAmount = 0.0;
                if (deliveryOrderInvoices.UseVat && deliveryOrderInvoices.IsPayVat)
                {
                    vatAmount = dppAmount * 0.1;
                    Total += vatAmount;
                }

                var incomeTaxAmount = 0.0;
                if (deliveryOrderInvoices.UseIncomeTax && deliveryOrderInvoices.IsPayTax)
                {
                    incomeTaxAmount = dppAmount * deliveryOrderInvoices.IncomeTaxRate / 100;
                    Total += incomeTaxAmount;
                }

                VATAmount = vatAmount;
                IncomeTaxAmount = incomeTaxAmount;
            }
            if (garmentDeliveryOrderDetail != null)
            {
                PriceCorrection = (double)garmentDeliveryOrderDetail.PriceTotalCorrection;
            }
        }
        //select new ReportIndexDto(deliveryOrderCustoms.ArrivalDate, deliveryOrderExternalPurchaseOrders.SupplierId, deliveryOrderExternalPurchaseOrders.SupplierName, deliveryOrderExternalPurchaseOrders.SupplierImport, deliveryOrderInternalNoteDetails.ProductName, (int) garmentDeliveryOrders.Id, garmentDeliveryOrders.DONo, garmentDeliveryOrders.BillNo, garmentDeliveryOrders.PaymentBill, (int) deliveryOrderInvoices.Id, deliveryOrderInvoices.InvoiceNo, deliveryOrderInvoices.VatNo, (int) deliveryOrderInternalNotes.Id, deliveryOrderInternalNotes.INNo, 0, deliveryOrderExternalPurchaseOrders.Category, 0, deliveryOrderExternalPurchaseOrders.Category, deliveryOrderInternalNoteDetails.Quantity, (int) deliveryOrderInternalNotes.CurrencyId.GetValueOrDefault(), deliveryOrderInternalNotes.CurrencyCode, deliveryOrderInternalNotes.CurrencyRate, deliveryOrderInternalNoteDetails.PriceTotal, deliveryOrderInvoices.UseVat, deliveryOrderInvoices.IsPayVat, deliveryOrderInvoices.UseIncomeTax, deliveryOrderInvoices.IsPayTax, deliveryOrderInvoices.IncomeTaxRate);

        public ReportIndexDto(DateTimeOffset customsArrivalDate, int supplierId, string supplierCode, string supplierName, bool isImportSupplier, string productName, int garmentDeliveryOrderId, string garmentDeliveryOrderNo, string billNo, string paymentBill, int invoiceId, string invoiceNo, string vatNo, int internalNoteId, string internalNoteNo, int purchasingCategoryId, string purchasingCategoryName, int accountingCategoryId, string accountingCategoryName, double internalNoteQuantity, int currencyId, string currencyCode, double currencyRate, double dppAmount, bool isUseVAT, bool isPayVAT, bool isUseIncomeTax, bool isIncomeTaxPaidBySupplier, double incomeTaxRate, DateTimeOffset customsDate, string customsNo, string customsType, string importValueRemark, double priceTotalCorrection)
        {
            CurrencyDPPAmount = dppAmount;
            DPPAmount = dppAmount * currencyRate;

            if (isUseVAT && isPayVAT)
            {
                VATAmount = DPPAmount * 0.1;
            }

            if (isUseIncomeTax && isIncomeTaxPaidBySupplier)
            {
                IncomeTaxAmount = DPPAmount * incomeTaxRate / 100;
            }

            //Total = DPPAmount + VATAmount - IncomeTaxAmount;
            Total = (DPPAmount + VATAmount - IncomeTaxAmount) + (priceTotalCorrection - (DPPAmount + VATAmount - IncomeTaxAmount));


            PriceCorrection = priceTotalCorrection - (DPPAmount + VATAmount - IncomeTaxAmount);

            CustomsArrivalDate = customsArrivalDate;
            SupplierId = supplierId;
            SupplierCode = supplierCode;
            SupplierName = supplierName;
            IsImportSupplier = isImportSupplier;
            ProductName = productName;
            GarmentDeliveryOrderId = garmentDeliveryOrderId;
            GarmentDeliveryOrderNo = garmentDeliveryOrderNo;
            BillNo = billNo;
            PaymentBill = paymentBill;
            InvoiceId = invoiceId;
            InvoiceNo = invoiceNo;
            VATNo = vatNo;
            InternalNoteId = internalNoteId;
            InternalNoteNo = internalNoteNo;
            PurchasingCategoryId = purchasingCategoryId;
            PurchasingCategoryName = purchasingCategoryName;
            AccountingCategoryId = accountingCategoryId;
            AccountingCategoryName = accountingCategoryName;
            InternalNoteQuantity = internalNoteQuantity;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            IsIncomeTaxPaidBySupplier = isIncomeTaxPaidBySupplier;
            CustomsDate = customsDate;
            CustomsNo = customsNo;
            CustomsType = customsType;
            ImportValueRemark = importValueRemark;
        }
        public ReportIndexDto(DateTimeOffset customsArrivalDate, int supplierId, string supplierName, bool isImportSupplier, string productName, int garmentDeliveryOrderId, string garmentDeliveryOrderNo, string billNo, string paymentBill, int invoiceId, string invoiceNo, string vatNo, int internalNoteId, string internalNoteNo, int purchasingCategoryId, string purchasingCategoryName, int accountingCategoryId, string accountingCategoryName, double internalNoteQuantity, int currencyId, string currencyCode, double currencyRate, double dppAmount, bool isUseVAT, bool isPayVAT, bool isUseIncomeTax, bool isIncomeTaxPaidBySupplier, double incomeTaxRate, DateTimeOffset customsDate, string customsNo, string customsType, string importValueRemark,string supplierCode)
        {
            CurrencyDPPAmount = dppAmount;
            DPPAmount = dppAmount * currencyRate;

            if (isUseVAT && isPayVAT)
            {
                VATAmount = DPPAmount * 0.1;
            }

            if (isUseIncomeTax && isIncomeTaxPaidBySupplier)
            {
                IncomeTaxAmount = DPPAmount * incomeTaxRate / 100;
            }

            Total = DPPAmount + VATAmount - IncomeTaxAmount;

            CustomsArrivalDate = customsArrivalDate;
            SupplierId = supplierId;
            SupplierName = supplierName;
            IsImportSupplier = isImportSupplier;
            ProductName = productName;
            GarmentDeliveryOrderId = garmentDeliveryOrderId;
            GarmentDeliveryOrderNo = garmentDeliveryOrderNo;
            BillNo = billNo;
            PaymentBill = paymentBill;
            InvoiceId = invoiceId;
            InvoiceNo = invoiceNo;
            VATNo = vatNo;
            InternalNoteId = internalNoteId;
            InternalNoteNo = internalNoteNo;
            PurchasingCategoryId = purchasingCategoryId;
            PurchasingCategoryName = purchasingCategoryName;
            AccountingCategoryId = accountingCategoryId;
            AccountingCategoryName = accountingCategoryName;
            InternalNoteQuantity = internalNoteQuantity;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            IsIncomeTaxPaidBySupplier = isIncomeTaxPaidBySupplier;
            CustomsDate = customsDate;
            CustomsNo = customsNo;
            CustomsType = customsType;
            ImportValueRemark = importValueRemark;
            SupplierCode = supplierCode;
        }

        public double Total { get; private set; }
        public DateTimeOffset CustomsArrivalDate { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierCode { get; private set; }
        public string SupplierName { get; private set; }
        public bool IsImportSupplier { get; private set; }
        public string ProductName { get; private set; }
        public int GarmentDeliveryOrderId { get; private set; }
        public string GarmentDeliveryOrderNo { get; private set; }
        public string BillNo { get; private set; }
        public string PaymentBill { get; private set; }
        public int InvoiceId { get; private set; }
        public string InvoiceNo { get; private set; }
        public string VATNo { get; private set; }
        public int InternalNoteId { get; private set; }
        public string InternalNoteNo { get; private set; }
        public int PurchasingCategoryId { get; private set; }
        public string PurchasingCategoryName { get; private set; }
        public int AccountingCategoryId { get; private set; }
        public string AccountingCategoryName { get; private set; }
        public double InternalNoteQuantity { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public double DPPAmount { get; private set; }
        public double VATAmount { get; private set; }
        public double IncomeTaxAmount { get; private set; }
        public bool IsIncomeTaxPaidBySupplier { get; private set; }
        public double CurrencyTotal { get; private set; }
        public double CurrencyDPPAmount { get; private set; }
        public DateTimeOffset CustomsDate { get; private set; }
        public string CustomsNo { get; private set; }
        public string CustomsType { get; private set; }
        public string ImportValueRemark { get; private set; }
        public double PriceCorrection { get; private set; }
    }
}