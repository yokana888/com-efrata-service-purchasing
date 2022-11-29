using System;

namespace Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance
{
    public class CustomsFormDto
    {
        public CustomsFormDto()
        {

        }

        public CustomsFormDto(int purchasingCategoryId, string purchasingCategoryName, string billsNo, string paymentBills, int garmentDeliveryOrderId, string garmentDeliveryOrderNo, int supplierId, string supplierCode, string supplierName, bool supplierIsImport, int currencyId, string currencyCode, double currencyRate, string productNames, DateTimeOffset arrivalDate, double dppAmount, double currencyDPPAmount, string paymentType)
        {
            PurchasingCategoryId = purchasingCategoryId;
            PurchasingCategoryName = purchasingCategoryName;
            BillsNo = billsNo;
            PaymentBills = paymentBills;
            GarmentDeliveryOrderId = garmentDeliveryOrderId;
            GarmentDeliveryOrderNo = garmentDeliveryOrderNo;
            SupplierId = supplierId;
            SupplierCode = supplierCode;
            SupplierName = supplierName;
            SupplierIsImport = supplierIsImport;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            ProductNames = productNames;
            ArrivalDate = arrivalDate;
            DPPAmount = dppAmount;
            CurrencyDPPAmount = currencyDPPAmount;
            PaymentType = paymentType;
        }

        public int PurchasingCategoryId { get; private set; }
        public string PurchasingCategoryName { get; private set; }
        public string BillsNo { get; private set; }
        public string PaymentBills { get; private set; }
        public int GarmentDeliveryOrderId { get; private set; }
        public string GarmentDeliveryOrderNo { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierCode { get; private set; }
        public string SupplierName { get; private set; }
        public bool SupplierIsImport { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public string ProductNames { get; private set; }
        public DateTimeOffset ArrivalDate { get; private set; }
        public double DPPAmount { get; private set; }
        public double CurrencyDPPAmount { get; private set; }
        public string PaymentType { get; private set; }
    }
}