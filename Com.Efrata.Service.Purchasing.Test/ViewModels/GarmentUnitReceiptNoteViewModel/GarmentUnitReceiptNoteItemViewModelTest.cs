using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.ViewModels.GarmentUnitReceiptNoteViewModel
{
    public class GarmentUnitReceiptNoteItemViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate()
        {
            GarmentUnitReceiptNoteItemViewModel viewModel = new GarmentUnitReceiptNoteItemViewModel();
            viewModel.URNId = 1;
            viewModel.DODetailId = 1;
            viewModel.PRNo = "";
            viewModel.PRItemId = 1;
            viewModel.POId = 1;
            viewModel.POItemId = 1;
            viewModel.PricePerDealUnit = 1;
            viewModel.IsCorrection = true;
            viewModel.ReceiptCorrection = 1;
            viewModel.OrderQuantity = 1;
            viewModel.DOCurrencyRate = 1;
            viewModel.ExpenditureItemId = 1;
            viewModel.UENItemId = 1;
            viewModel.PaymentType = "asd";
            viewModel.PaymentMethod = "asdasd";

            Assert.NotNull(viewModel);
        }
    }
}
