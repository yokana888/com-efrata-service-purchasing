using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public class DispositionMemoLoaderDto
    {
        public DispositionMemoLoaderDto(UnitPaymentOrderDto unitPaymentOrders, List<UnitReceiptNoteDto> unitReceiptNotes, double purchaseAmount, double purchaseAmountCurrency, string remark)
        {
            UnitPaymentOrder = unitPaymentOrders;
            UnitReceiptNotes = unitReceiptNotes;
            PurchaseAmount = purchaseAmount;
            PurchaseAmountCurrency = purchaseAmountCurrency;
            Remark = remark;
        }

        public UnitPaymentOrderDto UnitPaymentOrder { get; set; }
        public List<UnitReceiptNoteDto> UnitReceiptNotes { get; set; }
        public double PurchaseAmountCurrency { get; set; }
        public double PurchaseAmount { get; set; }
        public string Remark { get; set; }
    }
}