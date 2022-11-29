using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class UnitCostDto
    {
        public UnitCostDto(GarmentInternNoteDetail detail, List<GarmentInternNoteItem> internNoteItems, GarmentInternNote element, GarmentInvoice elementInvoice)
        {
            Unit = new UnitDto(detail.UnitId, detail.UnitCode, detail.UnitName, "", "0", "");

            var total = detail.PriceTotal;
            if (elementInvoice != null)
            {
                if (elementInvoice.UseVat)
                {
                    total += detail.PriceTotal * 0.1;
                }

                if (elementInvoice.UseIncomeTax && !elementInvoice.IsPayTax)
                {
                    total -= detail.PriceTotal * (elementInvoice.IncomeTaxRate / 100);
                }
            }

            PriceTotal = detail.PriceTotal;
            Amount = total;
        }

        public UnitCostDto(UnitPaymentOrderDetail detail, List<UnitReceiptNote> unitReceiptNotes, List<UnitReceiptNoteItem> unitReceiptNoteItems)
        {
            var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(item => item.Id == detail.URNItemId);
            var unitReceiptNote = unitReceiptNotes.FirstOrDefault(item => item.Id == unitReceiptNoteItem.URNId);

            Unit = new UnitDto(unitReceiptNote.UnitId, unitReceiptNote.UnitCode, unitReceiptNote.UnitName, unitReceiptNote.DivisionCode, unitReceiptNote.DivisionId, unitReceiptNote.DivisionName);
            Amount = detail.PriceTotal;
        }

        public UnitCostDto(UnitPaymentOrderDetail detail, List<UnitPaymentOrderItem> spbItems, List<UnitReceiptNoteItem> unitReceiptNoteItems, List<UnitReceiptNote> unitReceiptNotes)
        {
            var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(item => item.Id == detail.URNItemId);
            var unitReceiptNote = unitReceiptNotes.FirstOrDefault(item => item.Id == unitReceiptNoteItem.URNId);

            Unit = new UnitDto(unitReceiptNote.UnitId, unitReceiptNote.UnitCode, unitReceiptNote.UnitName, unitReceiptNote.DivisionCode, unitReceiptNote.DivisionId, unitReceiptNote.DivisionName);
            Amount = detail.PriceTotal;
        }

        public UnitCostDto(UnitPaymentOrderDetail detail, List<UnitPaymentOrderItem> spbItems, List<UnitReceiptNoteItem> unitReceiptNoteItems, List<UnitReceiptNote> unitReceiptNotes, UnitPaymentOrder element)
        {
            var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(item => item.Id == detail.URNItemId);
            var unitReceiptNote = unitReceiptNotes.FirstOrDefault(item => item.Id == unitReceiptNoteItem.URNId);

            Unit = new UnitDto(unitReceiptNote.UnitId, unitReceiptNote.UnitCode, unitReceiptNote.UnitName, unitReceiptNote.DivisionCode, unitReceiptNote.DivisionId, unitReceiptNote.DivisionName);

            var quantity = detail.ReceiptQuantity;
            if (detail.QuantityCorrection > 0)
                quantity = detail.QuantityCorrection;

            var price = detail.PricePerDealUnit;
            if (detail.PricePerDealUnitCorrection > 0)
                price = detail.PricePerDealUnitCorrection;

            var result = quantity * price;
            if (detail.PriceTotalCorrection > 0)
                result = detail.PriceTotalCorrection;

            var total = result;
            if (element != null)
            {
                if (element.UseVat)
                {
                    result += total * (element.VatRate/100);

                    //result += total * 0.1;
                }

                if (element.UseIncomeTax && (element.IncomeTaxBy == "Supplier" || element.IncomeTaxBy == "SUPPLIER"))
                {
                    result -= total * (element.IncomeTaxRate / 100);
                }
            }

            Amount = result;
        }

        public UnitDto Unit { get; private set; }
        internal double PriceTotal { get; private set; }
        public double Amount { get; private set; }
    }
}