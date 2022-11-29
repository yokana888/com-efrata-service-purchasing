using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.ViewModels.GarmentUnitReceiptNoteViewModel
{
    public class GarmentUnitReceiptNoteViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate_First()
        {
            GarmentUnitReceiptNoteItemViewModel internalNoteInvoices = new GarmentUnitReceiptNoteItemViewModel();
            internalNoteInvoices.URNId = 1;
            internalNoteInvoices.DODetailId = 1;
            internalNoteInvoices.PRNo = "";
            internalNoteInvoices.PRItemId = 1;
            internalNoteInvoices.POId = 1;
            internalNoteInvoices.POItemId = 1;
            internalNoteInvoices.PricePerDealUnit = 1;
            internalNoteInvoices.IsCorrection = false;
            internalNoteInvoices.ReceiptCorrection = 1;
            internalNoteInvoices.OrderQuantity = 1;
            internalNoteInvoices.DOCurrencyRate = 1;
            internalNoteInvoices.ExpenditureItemId = 1;
            internalNoteInvoices.UENItemId = 1;

            Assert.NotNull(internalNoteInvoices);
        }
    }
}
