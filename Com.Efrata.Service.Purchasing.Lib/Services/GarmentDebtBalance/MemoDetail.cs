namespace Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance
{
    public class MemoDetail
    {
        public MemoDetail(int garmentDeliveryOrderId, string garmentDeliveryOrderNo, string supplierCode, string supplierName, string internalNoteNo, string billsNo, string paymentBills, string currencyCode, double currencyRate, double purchaseAmount)
        {
            GarmentDeliveryOrderId = garmentDeliveryOrderId;
            GarmentDeliveryOrderNo = garmentDeliveryOrderNo;
            RemarksDetail = $"{supplierCode} - {supplierName}";
            SupplierCode = supplierCode;
            SupplierName = supplierName;
            InternalNoteNo = internalNoteNo;
            BillsNo = billsNo;
            PaymentBills = paymentBills;
            CurrencyCode = currencyCode;
            PurchasingRate = currencyRate;
            PurchaseAmount = purchaseAmount;
        }

        public int GarmentDeliveryOrderId { get; private set; }
        public string GarmentDeliveryOrderNo { get; private set; }
        public string RemarksDetail { get; private set; }
        public double PurchasingRate { get; private set; }
        public double PurchaseAmount { get; private set; }
        public string SupplierCode { get; private set; }
        public string SupplierName { get; private set; }
        public string InternalNoteNo { get; private set; }
        public string BillsNo { get; private set; }
        public string PaymentBills { get; private set; }
        public string CurrencyCode { get; private set; }

        public int PaymentRate { get; private set; }
        public int MemoAmount { get; private set; }
        public int MemoIdrAmount { get; private set; }

    }
}