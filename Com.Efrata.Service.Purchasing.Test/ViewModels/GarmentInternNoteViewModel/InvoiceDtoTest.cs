using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.ViewModels.GarmentInternNoteViewModel
{
    public class InvoiceDtoTest
    {
        [Fact]
        public void Should_Success_Instantiate_First()
        {
            InvoiceDto viewModel = new InvoiceDto("invoiceNo", DateTimeOffset.Now, "productNames", 1, "categoryName", "paymentMethod", 1, "deliveryOrdersNo", "billsNo", "paymentBills", 1, true, true, true, true, 1, 1);

            Assert.NotEqual(0, viewModel.Amount);
            Assert.NotEqual(0, viewModel.TotalAmount);
            Assert.NotEqual(0, viewModel.CorrectionAmount);
            Assert.Equal(1, viewModel.Id);
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void Should_Success_Instantiate_Second()
        {
            List<DeliveryOrderDto> deliveryOrders = new List<DeliveryOrderDto>();
            DeliveryOrderDto deliveryOrder = new DeliveryOrderDto("doNo", 1, "paymentBill", "billNo", 1, 1);
            deliveryOrders.Add(deliveryOrder);

            InvoiceDto viewModel = new InvoiceDto("invoiceNo", DateTimeOffset.Now, "productNames", 1, "categoryName", "paymentMethod", 1, "deliveryOrdersNo", "billsNo", "paymentBills", 1, true, true, true, true, 1, 1, deliveryOrders);

            Assert.NotEqual(0, viewModel.Amount);
            Assert.NotEqual(0, viewModel.TotalAmount);
            Assert.Equal(1, viewModel.Id);
            Assert.NotNull(viewModel);
        }
    }
}
