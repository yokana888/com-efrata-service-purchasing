using Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.ViewModels.DeliveryOrderViewModel
{
    public class DeliveryOrderItemViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate_PurchaseOrder()
        {
            DeliveryOrderItemViewModel viewModel = new DeliveryOrderItemViewModel()
            {
               fulfillments=new List<DeliveryOrderFulFillMentViewModel>()
               {
                   new DeliveryOrderFulFillMentViewModel()
               },
               purchaseOrderExternal=new PurchaseOrderExternal()
               {
                   no="1"
               },
               
            };
            Assert.NotNull(viewModel.fulfillments);
            Assert.NotNull(viewModel.purchaseOrderExternal);
        }
    }
}
