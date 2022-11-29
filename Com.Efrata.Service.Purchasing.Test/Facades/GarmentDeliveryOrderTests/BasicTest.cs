using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentDeliveryOrderTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentDeliveryOrder";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private PurchasingDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }

        private Mock<IServiceProvider> GetServiceProviderError()
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = null;
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }

        private GarmentDeliveryOrderDataUtil dataUtil(GarmentDeliveryOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider,_dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }

        [Fact]
        public async Task Should_Error_Get_Currency_when_Create_Data()
        {

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProviderError().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            foreach(var item in model.Items)
            {
                item.CurrencyCode = "test";
            }
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_2()
        {

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData2();
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(null, USERNAME));
            Assert.NotNull(e.Message);
        }

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

        //    GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
        //    {
        //        Id = model.Id,
        //        supplier = new SupplierViewModel(),
        //        internNo = "1",
        //        billNo = "test",
        //        paymentBill = "test",
        //        totalAmount = 1,
        //        shipmentType = "test",
        //        shipmentNo = "test",
        //        paymentMethod = "test",
        //        paymentType = "test",
        //        docurrency = new CurrencyViewModel(),
        //        items = new List<GarmentDeliveryOrderItemViewModel>
        //        {
        //            new GarmentDeliveryOrderItemViewModel
        //            {
        //                purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
        //                paymentDueDays = 1,
        //                currency = new CurrencyViewModel(),

        //                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
        //                {
        //                    new GarmentDeliveryOrderFulfillmentViewModel
        //                    {
        //                        pOId = 1,
        //                        pOItemId = 1,
        //                        conversion = 0,
        //                        quantityCorrection = 0,
        //                        pricePerDealUnit = 0,
        //                        priceTotalCorrection = 0,
        //                        isSave = true
        //                    }
        //                }
        //            },
        //            new GarmentDeliveryOrderItemViewModel
        //            {
        //                Id = model.Items.ElementAt(0).Id,
        //                purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
        //                paymentDueDays = 1,
        //                currency = new CurrencyViewModel(),

        //                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
        //                {
        //                    new GarmentDeliveryOrderFulfillmentViewModel
        //                    {
        //                        Id = model.Items.ElementAt(0).Details.ElementAt(0).Id,
        //                        pOId = 1,
        //                        pOItemId = 1,
        //                        conversion = 0,
        //                        quantityCorrection = 0,
        //                        pricePerDealUnit = 0,
        //                        priceTotalCorrection = 0,
        //                        isSave = false
        //                    }
        //                }
        //            }
        //        }

        //    };

        //    List<GarmentDeliveryOrderItem> item = new List<GarmentDeliveryOrderItem>(model.Items);
        //    List<GarmentDeliveryOrderDetail> detail = new List<GarmentDeliveryOrderDetail>(item[0].Details);

        //    model.Items = model.Items.Concat(new[] { new GarmentDeliveryOrderItem
        //        {
        //            EPOId = 1,
        //            EPONo = "test",
        //            PaymentDueDays = 1,
        //            CurrencyCode = "test",
        //            CurrencyId = 1,
        //            Details = new List<GarmentDeliveryOrderDetail>
        //                    {
        //                        new GarmentDeliveryOrderDetail
        //                        {
        //                            POId = detail[0].POId,
        //                            POItemId = detail[0].POItemId,
        //                            Conversion = detail[0].Conversion,
        //                            QuantityCorrection = detail[0].QuantityCorrection,
        //                            PricePerDealUnit = detail[0].PricePerDealUnit,
        //                            PriceTotalCorrection = detail[0].PriceTotalCorrection,
        //                            DOQuantity = detail[0].DOQuantity,
        //                            EPOItemId = detail[0].EPOItemId,
        //                            CodeRequirment = "test",
        //                        }
        //                    }
        //        }});

        //    var Response = await facade.Update((int)model.Id, viewModel, model, USERNAME);
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data2()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetTestData2();

        //    GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
        //    {
        //        Id = model.Id,
        //        supplier = new SupplierViewModel(),
        //        internNo = "test",
        //        billNo = "test",
        //        paymentBill = "test",
        //        totalAmount = 1,
        //        shipmentType = "test",
        //        shipmentNo = "test",
        //        paymentMethod = "test",
        //        paymentType = "test",
        //        docurrency = new CurrencyViewModel(),
        //        items = new List<GarmentDeliveryOrderItemViewModel>
        //        {
        //            new GarmentDeliveryOrderItemViewModel
        //            {
        //                purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
        //                paymentDueDays = 1,
        //                currency = new CurrencyViewModel(),

        //                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
        //                {
        //                    new GarmentDeliveryOrderFulfillmentViewModel
        //                    {
        //                        pOId = 1,
        //                        pOItemId = 1,
        //                        conversion = 0,
        //                        quantityCorrection = 0,
        //                        pricePerDealUnit = 0,
        //                        priceTotalCorrection = 0,
        //                        isSave = true
        //                    }
        //                }
        //            },
        //            new GarmentDeliveryOrderItemViewModel
        //            {
        //                Id = model.Items.ElementAt(0).Id,
        //                purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
        //                paymentDueDays = 1,
        //                currency = new CurrencyViewModel(),

        //                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
        //                {
        //                    new GarmentDeliveryOrderFulfillmentViewModel
        //                    {
        //                        Id = model.Items.ElementAt(0).Details.ElementAt(0).Id,
        //                        pOId = 1,
        //                        pOItemId = 1,
        //                        conversion = 0,
        //                        quantityCorrection = 0,
        //                        pricePerDealUnit = 0,
        //                        priceTotalCorrection = 0,
        //                        isSave = false
        //                    }
        //                }
        //            }
        //        }

        //    };

        //    List<GarmentDeliveryOrderItem> item = new List<GarmentDeliveryOrderItem>(model.Items);
        //    List<GarmentDeliveryOrderDetail> detail = new List<GarmentDeliveryOrderDetail>(item[0].Details);

        //    model.Items = model.Items.Concat(new[] { new GarmentDeliveryOrderItem
        //    {
        //        EPOId = 1,
        //        EPONo = "test",
        //        PaymentDueDays = 1,
        //        CurrencyCode = "test",
        //        CurrencyId = 1,
        //        Details = new List<GarmentDeliveryOrderDetail>
        //                {
        //                    new GarmentDeliveryOrderDetail
        //                    {
        //                        POId = detail[0].POId,
        //                        POItemId = detail[0].POItemId,
        //                        Conversion = detail[0].Conversion,
        //                        QuantityCorrection = detail[0].QuantityCorrection,
        //                        PricePerDealUnit = detail[0].PricePerDealUnit,
        //                        PriceTotalCorrection = detail[0].PriceTotalCorrection,
        //                        DOQuantity = detail[0].DOQuantity,
        //                        EPOItemId = detail[0].EPOItemId,
        //                    }
        //                }
        //    }});

        //    var Response = await facade.Update((int)model.Id, viewModel, model, USERNAME);
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data3()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetTestData3();

        //    GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
        //    {
        //        Id = model.Id,
        //        supplier = new SupplierViewModel(),
        //        internNo = "test",
        //        billNo = "test",
        //        paymentBill = "test",
        //        totalAmount = 1,
        //        shipmentType = "test",
        //        shipmentNo = "test",
        //        paymentMethod = "test",
        //        paymentType = "test",
        //        docurrency = new CurrencyViewModel(),
        //        items = new List<GarmentDeliveryOrderItemViewModel>
        //        {
        //            new GarmentDeliveryOrderItemViewModel
        //            {
        //                purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
        //                paymentDueDays = 1,
        //                currency = new CurrencyViewModel(),

        //                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
        //                {
        //                    new GarmentDeliveryOrderFulfillmentViewModel
        //                    {
        //                        pOId = 1,
        //                        pOItemId = 1,
        //                        conversion = 0,
        //                        quantityCorrection = 0,
        //                        pricePerDealUnit = 0,
        //                        priceTotalCorrection = 0,
        //                        isSave = true
        //                    }
        //                }
        //            },
        //            new GarmentDeliveryOrderItemViewModel
        //            {
        //                Id = model.Items.ElementAt(0).Id,
        //                purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
        //                paymentDueDays = 1,
        //                currency = new CurrencyViewModel(),

        //                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
        //                {
        //                    new GarmentDeliveryOrderFulfillmentViewModel
        //                    {
        //                        Id = model.Items.ElementAt(0).Details.ElementAt(0).Id,
        //                        pOId = 1,
        //                        pOItemId = 1,
        //                        conversion = 0,
        //                        quantityCorrection = 0,
        //                        pricePerDealUnit = 0,
        //                        priceTotalCorrection = 0,
        //                        isSave = true
        //                    }
        //                }
        //            },
        //        }
        //    };

        //    List<GarmentDeliveryOrderItem> item = new List<GarmentDeliveryOrderItem>(model.Items);
        //    List<GarmentDeliveryOrderDetail> detail = new List<GarmentDeliveryOrderDetail>(item[0].Details);

        //    model.Items = model.Items.Concat(new[] { new GarmentDeliveryOrderItem
        //    {
        //        EPOId = 1,
        //        EPONo = "test",
        //        PaymentDueDays = 1,
        //        CurrencyCode = "test",
        //        CurrencyId = 1,
        //        Details = new List<GarmentDeliveryOrderDetail>
        //                {
        //                    new GarmentDeliveryOrderDetail
        //                    {
        //                        POId = detail[0].POId,
        //                        POItemId = detail[0].POItemId,
        //                        Conversion = detail[0].Conversion,
        //                        QuantityCorrection = detail[0].QuantityCorrection,
        //                        PricePerDealUnit = detail[0].PricePerDealUnit,
        //                        PriceTotalCorrection = detail[0].PriceTotalCorrection,
        //                        DOQuantity = detail[0].DOQuantity,
        //                        EPOItemId = detail[0].EPOItemId,
        //                    }
        //                }
        //    }});

        //    var Response = await facade.Update((int)model.Id, viewModel, model, USERNAME);
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data4()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetTestData3();
        //    var model2 = await dataUtil(facade, GetCurrentMethod()).GetTestData4();

        //    GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
        //    {
        //        Id = model.Id,
        //        supplier = new SupplierViewModel(),
        //        customsId = 1,
        //        billNo = "test",
        //        paymentBill = "test",
        //        totalAmount = 1,
        //        shipmentType = "test",
        //        shipmentNo = "test",
        //        paymentMethod = "test",
        //        paymentType = "test",
        //        docurrency = new CurrencyViewModel(),
        //        items = new List<GarmentDeliveryOrderItemViewModel>
        //        {
        //            new GarmentDeliveryOrderItemViewModel
        //            {
        //                Id = (model.Items.ElementAt(0).Id + 2),
        //                purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
        //                paymentDueDays = 1,
        //                currency = new CurrencyViewModel(),

        //                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
        //                {
        //                    new GarmentDeliveryOrderFulfillmentViewModel
        //                    {
        //                        Id = model.Items.ElementAt(0).Details.ElementAt(0).Id,
        //                        pOId = 1,
        //                        pOItemId = 1,
        //                        conversion = 0,
        //                        quantityCorrection = 0,
        //                        pricePerDealUnit = 0,
        //                        priceTotalCorrection = 0,
        //                        isSave = true
        //                    }
        //                }
        //            }
        //        }

        //    };
        //    model2.Items.Remove(model2.Items.FirstOrDefault());
        //    var Response = await facade.Update((int)model2.Id, viewModel, model2, USERNAME);
        //    Assert.NotEqual(Response, 0);
        //}

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.Update(0, null, model, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = await facade.Delete((int)model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data2()
        {
            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData2();
            var Response = await facade.Delete((int)model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data3()
        {
            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData3();
            var Response = await facade.Delete((int)model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Delete(0, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Loader()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadLoader();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Loader_With_Params()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadLoader(Search: "[\"DONo\"]", Select: "{ \"doNO\" : \"DONo\", \"DODate\" : 1 }");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_For_InternNote()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadForInternNote(new List<long> { model.Id });
            Assert.NotEmpty(Response);
        }

        [Fact]
		public async Task Should_Success_Get_Data_By_Supplier()
		{
			GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			var Response = facade.ReadBySupplier("code","{}");
			Assert.NotNull(Response);
		}
		[Fact]
		public async Task Should_Success_Get_Data_For_Customs()
		{
			GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			var Response = facade.DOForCustoms("code", "{}");
			Assert.NotNull(Response);
		}
		[Fact]
		public async Task Should_Success_Get_Data_Is_Received()
		{
			GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			model.IsInvoice = true;
			List<int> _id = new List<int>();
			_id.Add((int)model.Id);
			var Response = facade.IsReceived(_id);
			Assert.NotNull(Response);
		}
		[Fact]
		public async Task Should_Success_Get_Data_Is_Received2()
		{
			GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			model.IsInvoice = false;
			foreach(var data in model.Items)
			{
				foreach(var item in data.Details)
				{
					item.ReceiptQuantity = 2;
				}
			}
			List<int> _id = new List<int>();
			_id.Add((int)model.Id);
			var Response = facade.IsReceived(_id);
			Assert.NotNull(Response);
		}
		[Fact]
        public void Should_Success_Validate_Data()
        {
            GarmentDeliveryOrderViewModel nullViewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                }
            };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name ="test",
                    Country = "test"
                },

            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Where_DoDate_GreaterThan_ArrivalDate()
        {
            GarmentDeliveryOrderViewModel nullViewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                }
            };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
            {
                doDate = new DateTimeOffset(DateTime.Today).AddDays(7),
                arrivalDate = new DateTimeOffset(DateTime.Today).AddDays(1),
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                },
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Item()
        {
            GarmentDeliveryOrderViewModel nullViewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                }
            };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                },
                internNo = "test",
                billNo = "test",
                paymentBill = "test",
                totalAmount = 1,
                shipmentType = "test",
                shipmentNo = "test",
                paymentMethod = "test",
                paymentType = "test",
                docurrency = new CurrencyViewModel(),
                items = new List<GarmentDeliveryOrderItemViewModel>
                {
                    new GarmentDeliveryOrderItemViewModel
                    {
                        purchaseOrderExternal = null,
                        paymentDueDays = 1,
                        currency = new CurrencyViewModel(),
                        
                        fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
                        {
                            new GarmentDeliveryOrderFulfillmentViewModel
                            {
                                pOId = 1,
                                pOItemId = 1,
                                conversion = 2,
                                purchaseOrderUom = new UomViewModel()
                                {
                                    Id= "1",
                                    Unit = "test"
                                },
                                smallUom = new UomViewModel()
                                {
                                    Id = "1",
                                    Unit = "test"
                                }
                            }
                        }
                    }
                }

            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Fulfillment_Null()
        {
            GarmentDeliveryOrderViewModel nullViewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                }
            };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                },
                internNo = "test",
                billNo = "test",
                paymentBill = "test",
                totalAmount = 1,
                shipmentType = "test",
                shipmentNo = "test",
                paymentMethod = "test",
                paymentType = "test",
                docurrency = new CurrencyViewModel(),
                items = new List<GarmentDeliveryOrderItemViewModel>
                {
                    new GarmentDeliveryOrderItemViewModel
                    {
                        purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
                        paymentDueDays = 1,
                        currency = new CurrencyViewModel(),
                        fulfillments = null
                    }
                }

            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Fulfillment_With_Conversion_0()
        {
            GarmentDeliveryOrderViewModel nullViewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                }
            };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                },
                internNo = "test",
                billNo = "test",
                paymentBill = "test",
                totalAmount = 1,
                shipmentType = "test",
                shipmentNo = "test",
                paymentMethod = "test",
                paymentType = "test",
                docurrency = new CurrencyViewModel(),
                items = new List<GarmentDeliveryOrderItemViewModel>
                {
                    new GarmentDeliveryOrderItemViewModel
                    {
                        purchaseOrderExternal = new PurchaseOrderExternal{ Id = 1,no="test"},
                        paymentDueDays = 1,
                        currency = new CurrencyViewModel(),

                        fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
                        {
                            new GarmentDeliveryOrderFulfillmentViewModel
                            {
                                pOId = 1,
                                pOItemId = 1,
                                conversion = 0,
                                quantityCorrection = 0,
                                pricePerDealUnit = 0,
                                priceTotalCorrection = 0,
                            }
                        }
                    }
                }

            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Validate_Data_Duplicate()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            GarmentDeliveryOrderViewModel viewModel = new GarmentDeliveryOrderViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test",
                    Country = "test"
                },
                incomeTax = new IncomeTaxViewModel(),
                docurrency = new CurrencyViewModel(),
            };
            viewModel.Id = model.Id + 1;
            viewModel.doNo = model.DONo;
            viewModel.supplier.Id = model.SupplierId;
            viewModel.doDate = model.DODate;
            viewModel.arrivalDate = model.ArrivalDate;
            viewModel.docurrency.Id = (long)model.DOCurrencyId;
            viewModel.docurrency.Code = model.DOCurrencyCode;
            viewModel.incomeTax.Id = (int)model.IncomeTaxId;
            viewModel.incomeTax.Name = model.IncomeTaxName;
            viewModel.incomeTax.Rate = (double)model.IncomeTaxRate;
            viewModel.remark = model.Remark;
            viewModel.isCorrection = (bool)model.IsCorrection;
            viewModel.useVat = (bool)model.UseVat;
            viewModel.useIncomeTax = (bool)model.UseIncomeTax;
            

            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(PurchasingDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));

            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(viewModel, serviceProvider.Object, null);

            var validationResultCreate = viewModel.Validate(validationContext).ToList();

            var errorDuplicateDONo = validationResultCreate.SingleOrDefault(r => r.ErrorMessage.Equals("DoNo is already exist"));
            Assert.NotNull(errorDuplicateDONo);
        }

        [Fact]
        public async Task Should_Success_Get_Data_For_UnitReceiptNote()
        {
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<GarmentDeliveryOrderViewModel>
                {
                    new GarmentDeliveryOrderViewModel
                    {
                        items = new List<GarmentDeliveryOrderItemViewModel>
                        {
                            new GarmentDeliveryOrderItemViewModel
                            {
                                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
                                {
                                    new GarmentDeliveryOrderFulfillmentViewModel()
                                }
                            }
                        }
                    }
                });

            var serviceProvider = GetServiceProvider();
            serviceProvider
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(mapper.Object);

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var filter = new
            {
                model.SupplierId,
                model.Items.First().Details.First().UnitId
            };
            var filterString = JsonConvert.SerializeObject(filter);
            var Response = facade.ReadForUnitReceiptNote(Filter:filterString);
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_For_CorrectionNoteQuantity()
        {
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<GarmentDeliveryOrderViewModel>
                {
                    new GarmentDeliveryOrderViewModel
                    {
                        items = new List<GarmentDeliveryOrderItemViewModel>
                        {
                            new GarmentDeliveryOrderItemViewModel
                            {
                                fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
                                {
                                    new GarmentDeliveryOrderFulfillmentViewModel()
                                }
                            }
                        }
                    }
                });

            var serviceProvider = GetServiceProvider();
            serviceProvider
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(mapper.Object);

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadForCorrectionNoteQuantity();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public void Should_Success_GetReportHeaderAccuracyofArrivaly()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GetReportHeaderAccuracyofArrival("Bahan Baku", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response);

            var Response2 = facade.GetReportHeaderAccuracyofArrival("Bahan Pendukung", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response2);
        }

        [Fact]
        public void Should_Success_GenerateExcelArrivalHeader()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GenerateExcelArrivalHeader("Bahan Baku", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response);

            var Response2 = facade.GenerateExcelArrivalHeader("Bahan Pendukung", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response2);
        }

        [Fact]
        public void Should_Success_GetReportDetailAccuracyofArrival()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GetReportDetailAccuracyofArrival("", "Bahan Baku", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response);

            var Response2 = facade.GetReportDetailAccuracyofArrival("", "Bahan Pendukung", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response2);
        }

        [Fact]
        public void Should_Success_GetAccuracyOfArrivalDetail()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GetAccuracyOfArrivalDetail("", "Bahan Baku", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response);

            var Response2 = facade.GetAccuracyOfArrivalDetail("", "Bahan Pendukung", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response2);
        }

        [Fact]
        public void Should_Success_GenerateExcelArrivalDetail()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GenerateExcelArrivalDetail("", "Bahan Baku", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response);

            var Response2 = facade.GenerateExcelArrivalDetail("", "Bahan Pendukung", DateTime.Now, DateTime.Now, 0);
            Assert.NotNull(Response2);
        }

        [Fact]
        public void Should_Success_GetReportHeaderAccuracyofDelivery()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GetReportHeaderAccuracyofDelivery(DateTime.Now, DateTime.Now, "", "", 0);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GenerateExcelDeliveryHeader()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GenerateExcelDeliveryHeader(DateTime.Now, DateTime.Now, "", "", 0);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GetReportDetailAccuracyofDelivery()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GetReportDetailAccuracyofDelivery("", DateTime.Now, DateTime.Now, "", "", 0);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GenerateExcelDeliveryDetail()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GenerateExcelDeliveryDetail("", DateTime.Now, DateTime.Now, "", "", 0);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_GetReportDO()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GetReportDO("", "", 0, "", "" , DateTime.Now, DateTime.Now, 1, 1, "{}", 0);
            Assert.NotNull(Response);
        }

        //[Fact]
        //public void Should_Success_GenerateExcelDO()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var Response = facade.GenerateExcelDO("", "", 0, "", "", DateTime.Now, DateTime.Now, 0);
        //    Assert.NotNull(Response);
        //}
    }
}
