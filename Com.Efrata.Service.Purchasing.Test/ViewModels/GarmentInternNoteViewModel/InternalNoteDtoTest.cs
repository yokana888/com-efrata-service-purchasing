using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.ViewModels.GarmentInternNoteViewModel
{
    public class InternalNoteDtoTest
    {
        [Fact]
        public void Should_Success_Instantiate_First()
        {
            List<InternalNoteInvoiceDto> internalNoteInvoices = new List<InternalNoteInvoiceDto>();
            InternalNoteInvoiceDto internalNoteInvoice = new InternalNoteInvoiceDto("invoiceNo", DateTimeOffset.Now, "productNames", 1, "categoryName", "paymentMethod", 1, "deliveryOrdersNo", "billsNo", "paymentBills", 1, true, true, true, true, 1, 1);
            internalNoteInvoices.Add(internalNoteInvoice);
            InternalNoteDto viewModel = new InternalNoteDto(1, "internalNoteNo", DateTimeOffset.Now, DateTimeOffset.Now, 1, "supplierName", "supplierCode", 1, "currencyCode", 1, internalNoteInvoices);

            Assert.NotEqual(0, viewModel.DPP);
            Assert.NotEqual(0, viewModel.VATAmount);
            Assert.NotEqual(0, viewModel.IncomeTaxAmount);
            Assert.NotEqual(0, viewModel.TotalAmount);
            Assert.Equal(1, viewModel.Id);
            Assert.NotNull(viewModel);
        }
    }
}
