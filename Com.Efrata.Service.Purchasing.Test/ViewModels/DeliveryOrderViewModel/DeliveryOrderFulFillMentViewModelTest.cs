using Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.ViewModels.DeliveryOrderViewModel
{
  public  class DeliveryOrderFulFillMentViewModelTest
    {
        
        [Fact]
        public void Should_Success_Instantiate()
        {
            DeliveryOrderFulFillMentViewModel viewModel = new DeliveryOrderFulFillMentViewModel()
            {
                EPODetailId = 1,
                deliveredQuantity = 1,
                POItemId = 1,
                PRItemId = 1,
                product = new ProductViewModel()
                {
                    price = 1
                },
                purchaseOrder = new PurchaseOrder()
                {
                    purchaseRequest = new PurchaseRequest()
                },
                purchaseOrderQuantity = 1,
                purchaseOrderUom = new UomViewModel(),
                receiptQuantity = 1,
                remark = "remark"
            };

            Assert.Equal(1, viewModel.EPODetailId);
            Assert.Equal(1, viewModel.deliveredQuantity);
            Assert.Equal(1, viewModel.POItemId);
            Assert.Equal(1, viewModel.PRItemId);
            Assert.NotNull(viewModel.product);
            Assert.NotNull(viewModel.purchaseOrder);
            Assert.NotNull(viewModel.purchaseOrderUom);
            Assert.Equal(1, viewModel.purchaseOrderQuantity);
            Assert.Equal(1, viewModel.receiptQuantity);
            Assert.Equal("remark", viewModel.remark);
        }

        [Fact]
        public void Should_Success_Instantiate_PurchaseOrder()
        {
            PurchaseOrder viewModel = new PurchaseOrder()
            {
                purchaseRequest=new PurchaseRequest()
            };
            Assert.NotNull(viewModel.purchaseRequest);
        }

        [Fact]
        public void Should_Success_Instantiate_PurchaseRequest()
        {
            PurchaseRequest viewModel = new PurchaseRequest()
            {
                no="1",
                unit=new UnitViewModel(),
                _id=1
            };
            Assert.NotNull(viewModel.unit);
            Assert.Equal("1", viewModel.no);
            Assert.Equal(1, viewModel._id);
        }
        
    }
}

