using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class DeliveryOrderDto
    {
        public DeliveryOrderDto(string dONo, double totalAmount, string paymentBill, string billNo, long dOId,double currencyRate)
        {
            DONo = dONo;
            TotalAmount = totalAmount;
            PaymentBill = paymentBill;
            BillNo = billNo;
            DOId = dOId;
            CurrencyRate = currencyRate;
        }

        public string DONo { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentBill { get; set; }
        public string BillNo { get; set; }
        public long DOId { get; set; }
        public double CurrencyRate { get; set; }
    }
}
