using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public class UnitPaymentOrderMemoLoaderDto
    {
        public UnitPaymentOrderMemoLoaderDto(ExpenditureDto expenditure, SupplierDto supplier, string remark, UnitPaymentOrderDto unitPaymentOder, List<UnitReceiptNoteDto> unitReceiptNotes, double purchaseAmountCurrency, double purchaseAmount, double paymentAmountCurrency, double paymentAmount)
        {
            Expenditure = expenditure;
            Supplier = supplier;
            Remark = remark;
            UnitPaymentOrder = unitPaymentOder;
            UnitReceiptNotes = unitReceiptNotes;
            PurchaseAmountCurrency = purchaseAmountCurrency;
            PurchaseAmount = purchaseAmount;
            PaymentAmountCurrency = paymentAmountCurrency;
            PaymentAmount = paymentAmount;
        }

        public ExpenditureDto Expenditure { get; set; }
        public SupplierDto Supplier { get; set; }
        public string Remark { get; set; }
        public UnitPaymentOrderDto UnitPaymentOrder { get; set; }
        public List<UnitReceiptNoteDto> UnitReceiptNotes { get; set; }
        public double PurchaseAmountCurrency { get; set; }
        public double PurchaseAmount { get; set; }
        public double PaymentAmountCurrency { get; set; }
        public double PaymentAmount { get; set; }

    }
}