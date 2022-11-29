using System;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade
{
    public class CreateCreditorAccountViewModel
    {
        public int UnitPaymentCorrectionId { get; set; }
        public string UnitPaymentCorrectionNo { get; set; }
        public string UnitReceiptNoteNo { get; set; }
        public decimal UnitPaymentCorrectionDPP { get; set; }
        public decimal UnitPaymentCorrectionPPN { get; set; }
        public decimal UnitPaymentCorrectionMutation { get; set; }
        public DateTimeOffset UnitPaymentCorrectionDate { get; set; }
    }
}