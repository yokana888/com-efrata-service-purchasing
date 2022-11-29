using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInvoiceFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Moq;
//using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInvoiceDataUtils
{
    public class GarmentInvoiceDataUtil
    {
        private GarmentInvoiceItemDataUtil garmentInvoiceItemDataUtil;
        private GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil;
        private readonly GarmentInvoiceFacade facade;
        private GarmentInvoiceDetailDataUtil GarmentInvoiceDetailDataUtil;

        public GarmentInvoiceDataUtil(GarmentInvoiceFacade facade)
        {
            this.facade = facade;
        }

        public GarmentInvoiceDataUtil(GarmentInvoiceItemDataUtil GarmentInvoiceItemDataUtil, GarmentInvoiceDetailDataUtil GarmentInvoiceDetailDataUtil, GarmentDeliveryOrderDataUtil GarmentDeliveryOrderDataUtil, GarmentInvoiceFacade facade)
        {
            this.garmentInvoiceItemDataUtil = GarmentInvoiceItemDataUtil;
            this.GarmentInvoiceDetailDataUtil = GarmentInvoiceDetailDataUtil;
            this.garmentDeliveryOrderDataUtil = GarmentDeliveryOrderDataUtil;
            this.facade = facade;
        }
        public async Task<GarmentInvoiceItem> GetNewDataItem(string user)
        {
            var garmentDO = await garmentDeliveryOrderDataUtil.GetNewData("User");
            return new GarmentInvoiceItem
            {

                DeliveryOrderId = garmentDO.Id,
                DODate = DateTimeOffset.Now,
                DeliveryOrderNo = "dono",
                ArrivalDate = DateTimeOffset.Now,
                TotalAmount = 2000,
                PaymentType = "type",
                Details = new List<GarmentInvoiceDetail>
                            {
                                new GarmentInvoiceDetail
                                {
                                    EPOId=It.IsAny<int>(),
                                    EPONo="epono",
                                    IPOId=It.IsAny<int>(),
                                    PRItemId=It.IsAny<int>(),
                                    PRNo="prno",
                                    RONo="12343",
                                    ProductId= It.IsAny<int>(),
                                    ProductCode="code",
                                    ProductName="name",
                                    UomId=It.IsAny<int>(),
                                    UomUnit="ROLL",
                                    DOQuantity=40,
                                    PricePerDealUnit=5000,
                                    POSerialNumber="PM132434",
                                     PaymentDueDays=2
                                }
                }
            };
        }
        public async Task<GarmentInvoice> GetNewData(string user, GarmentDeliveryOrder garmentDeliveryOrder = null)
        {
            var garmentDO = garmentDeliveryOrder ?? await garmentDeliveryOrderDataUtil.GetNewData("User");
            //long nowTicks = DateTimeOffset.Now.Ticks;
            return new GarmentInvoice
            {
                InvoiceNo = "InvoiceNo",
                InvoiceDate = DateTimeOffset.Now,
                SupplierId = It.IsAny<int>(),
                SupplierCode = "codeS",
                SupplierName = "nameS",
                IncomeTaxId = It.IsAny<int>(),
                IncomeTaxName = "name",
                IncomeTaxNo = "Inc",
                IncomeTaxDate = DateTimeOffset.Now,
                IncomeTaxRate = 2,
                VatNo = "vat",
                VatId = It.IsAny<int>(),
                VatRate = 10,
                VatDate = DateTimeOffset.Now,
                UseIncomeTax = true,
                UseVat = true,
                IsPayTax = true,
                HasInternNote = false,
                CurrencyId = It.IsAny<int>(),
                CurrencyCode = "TEST",
                Items = new List<GarmentInvoiceItem>
                    {
                        new GarmentInvoiceItem
                        {

                           DeliveryOrderId =garmentDO.Id,
                           DODate=DateTimeOffset.Now,
                           DeliveryOrderNo="dono",
                           ArrivalDate  =  DateTimeOffset.Now,
                           TotalAmount =2000,
                           PaymentType="type",

                           PaymentMethod="method",
                            Details= new List<GarmentInvoiceDetail>
                            {
                                new GarmentInvoiceDetail
                                {
                                    EPOId=It.IsAny<int>(),
                                    EPONo="epono",
                                    IPOId=It.IsAny<int>(),
                                    PRItemId=It.IsAny<int>(),
                                    PRNo="prno",
                                    RONo="12343",
                                    ProductId= It.IsAny<int>(),
                                    ProductCode="code",
                                    ProductName="name",
                                    UomId=It.IsAny<int>(),
                                    UomUnit="ROLL",
                                    DOQuantity=40,
                                    PricePerDealUnit=5000,
                                    POSerialNumber="PM132434",
                                     PaymentDueDays=2

                                }
                            }
                        }
                }
            };
        }
        public async Task<GarmentInvoice> GetTestData(string user)
        {
            GarmentInvoice model = await GetNewData(user);

            await facade.Create(model, user);

            return model;
        }
        public async Task<GarmentInvoice> GetTestData2(string user, GarmentDeliveryOrder deliveryOrder)
        {
            GarmentInvoice model = await GetNewData(user, deliveryOrder);
            model.NPH = "acdg";
            model.NPN = "acdf";

            await facade.Create(model, user);
            return model;
        }
        public async Task<GarmentInvoice> GetNewDataViewModel(string user)
        {
            var garmentDeliveryOrder = await garmentDeliveryOrderDataUtil.GetNewData(user);
            DateTime dateWithoutOffset = new DateTime(2010, 7, 16, 13, 32, 00);
            return new GarmentInvoice
            {
                InvoiceNo = "InvoiceNo",
                InvoiceDate = dateWithoutOffset,
                SupplierId = It.IsAny<int>(),
                SupplierCode = "codeS",
                SupplierName = "nameS",
                IncomeTaxId = It.IsAny<int>(),
                IncomeTaxName = "name",
                IncomeTaxNo = "Inc",
                IncomeTaxDate = DateTimeOffset.Now,
                IncomeTaxRate = 2,
                VatNo = "vat",
                VatId = It.IsAny<int>(),
                VatRate = 10,
                VatDate = DateTimeOffset.Now,
                UseIncomeTax = true,
                UseVat = true,
                IsPayTax = true,
                HasInternNote = false,
                CurrencyId = It.IsAny<int>(),
                CurrencyCode = "TEST",
                Items = new List<GarmentInvoiceItem> { garmentInvoiceItemDataUtil.GetNewDataViewModel(garmentDeliveryOrder) }
            };
        }
        public async Task<GarmentInvoice> GetNewDataNoUseIncomeTaxNoUseVatViewModel(string user)
        {
            var garmentDeliveryOrder = await garmentDeliveryOrderDataUtil.GetNewData(user);
            DateTime dateWithoutOffset = new DateTime(2010, 7, 16, 13, 32, 00);
            return new GarmentInvoice
            {
                InvoiceNo = "InvoiceNo",
                InvoiceDate = dateWithoutOffset,
                SupplierId = It.IsAny<int>(),
                SupplierCode = "codeS",
                SupplierName = "nameS",
                IncomeTaxId = It.IsAny<int>(),
                UseIncomeTax = false,
                UseVat = false,
                IsPayTax = true,
                HasInternNote = false,
                CurrencyId = It.IsAny<int>(),
                CurrencyCode = "TEST",
                Items = new List<GarmentInvoiceItem> { garmentInvoiceItemDataUtil.GetNewDataViewModel(garmentDeliveryOrder) }
            };
        }


        public async Task<GarmentInvoice> GetTestDataViewModel(string user)
        {
            GarmentInvoice model = await GetNewDataViewModel(user);

            await facade.Create(model, user);

            return model;
        }

        public async Task<GarmentInvoice> GetTestDataNoUseIncomeTaxNoUseVatViewModel(string user)
        {
            GarmentInvoice model = await GetNewDataNoUseIncomeTaxNoUseVatViewModel(user);

            await facade.Create(model, user);

            return model;
        }

        public GarmentInvoice GetNewData_VBRequestPOExternal()
        {
            return new GarmentInvoice()
            {
                Items=new List<GarmentInvoiceItem>()
                {
                    new GarmentInvoiceItem()
                    {
                        Details=new List<GarmentInvoiceDetail>()
                        {
                            new GarmentInvoiceDetail()
                        }
                    }
                }

            };
        }
        public async Task<GarmentInvoice> GetTestData_VBRequestPOExternal(string user)
        {
            GarmentInvoice model = GetNewData_VBRequestPOExternal();

            await facade.Create(model, user);

            return model;
        }
    }
}
