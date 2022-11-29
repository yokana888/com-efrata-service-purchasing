using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Migrations;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitExpenditureDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using System.IO;
using Microsoft.AspNetCore.JsonPatch;
using System.Data;
using System.Data.SqlClient;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentBeacukaiFacade;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentBeacukaiDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Newtonsoft.Json;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentUnitExpenditureNoteTests
{
    public class BasicTests
    {
        private const string ENTITY = "GarmentUnitExpenditureNote";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        private IServiceProvider GetServiceProvider()
        {

			var httpClientService = new Mock<IHttpClientService>();
			HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
			message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"codeRequirement\":\"BB\",\"code\":\"BB\",\"rate\":13700.0,\"name\":\"FABRIC\",\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
			 

			httpClientService
				.Setup(x => x.GetAsync(It.IsAny<string>()))
				.ReturnsAsync(message);

			httpClientService
				.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("expenditure-goods/byRO"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentExpenditureGoodDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("customs-reports/getPEB")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new BeacukaiAddedDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("customs-reports/getPEB"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new BeacukaiAddedDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("finishing-outs/for-traceable")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentFinishingOutDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
               .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("dpp-vat-bank-expenditure-notes/invoice")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new DPPVATBankExpenditureNoteDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("garment-sample-finishing-outs/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentFinishingOutDataUtil().GetNullFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("cutting-outs/for-traceable")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCuttingOutDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentExpenditureGoodDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/byInvoice")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentExpenditureGoodDataUtil().GetMultipleResultFormatterOkCMTString()) });
            httpClientService
                .Setup(x => x.PostAsync(It.Is<string>(s => s.Contains("garment-debt-balances/customs")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("expenditure-goods"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentExpenditureGoodDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/monitoring/garment-cmt-sales"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentInvoiceMonitoringDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("preparings/byRONO")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentPreparingDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOkString()) });
            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCategoryDataUtil().GetMultipleResultFormatterOkString()) });

            //HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            //httpResponseMessage.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            //var httpClientService = new Mock<IHttpClientService>();
            //httpClientService
            //    .Setup(x => x.GetAsync(It.IsAny<string>()))
            //    .ReturnsAsync(httpResponseMessage);

            var mapper = new Mock<IMapper>();
            mapper
                .Setup(x => x.Map<GarmentUnitExpenditureNoteViewModel>(It.IsAny<GarmentUnitExpenditureNote>()))
                .Returns(new GarmentUnitExpenditureNoteViewModel
                {
                    Id = 1,
                    UnitDONo = "UnitDONO1234",
                    ExpenditureType = "TRANSFER",
                    Storage = new Lib.ViewModels.IntegrationViewModel.StorageViewModel(),
                    StorageRequest = new Lib.ViewModels.IntegrationViewModel.StorageViewModel(),
                    UnitSender = new UnitViewModel(),
                    UnitRequest = new UnitViewModel(),
                    Items = new List<GarmentUnitExpenditureNoteItemViewModel>
                    {
                        new GarmentUnitExpenditureNoteItemViewModel {
                            ProductId = 1,
                            UomId = 1,
                        }
                    }
                });

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder());

            var mockDebtBalanceService = new Mock<IGarmentDebtBalanceService>();
            mockDebtBalanceService
                .Setup(x => x.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .ReturnsAsync(1);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(mapper.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDebtBalanceService)))
                .Returns(mockDebtBalanceService.Object);


            return serviceProviderMock.Object;
        }
        private IServiceProvider GetServiceProviderUnitReceiptNote()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            var mapper = new Mock<IMapper>();
            mapper
                .Setup(x => x.Map<GarmentUnitReceiptNoteViewModel>(It.IsAny<GarmentUnitReceiptNote>()))
                .Returns(new GarmentUnitReceiptNoteViewModel
                {
                    Items = new List<GarmentUnitReceiptNoteItemViewModel>
                    {
                        new GarmentUnitReceiptNoteItemViewModel()
                    }
                });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(mapper.Object);

            return serviceProviderMock.Object;
        }

        private IServiceProvider GetServiceProviderUnitReceiptNote_DOCurrency()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":0.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            var mapper = new Mock<IMapper>();
            mapper
                .Setup(x => x.Map<GarmentUnitReceiptNoteViewModel>(It.IsAny<GarmentUnitReceiptNote>()))
                .Returns(new GarmentUnitReceiptNoteViewModel
                {
                    Items = new List<GarmentUnitReceiptNoteItemViewModel>
                    {
                        new GarmentUnitReceiptNoteItemViewModel()
                    }
                });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(mapper.Object);

            return serviceProviderMock.Object;
        }

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
		
		private GarmentUnitExpenditureNoteDataUtil dataUtil(GarmentUnitExpenditureNoteFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(testName));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(testName), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);


            return new GarmentUnitExpenditureNoteDataUtil(facade, garmentUnitDeliveryOrderDatautil);
        }

        private GarmentUnitExpenditureNoteDataUtil dataUtil_DOCurrency(GarmentUnitExpenditureNoteFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote_DOCurrency(), _dbContext(testName));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(testName), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);


            return new GarmentUnitExpenditureNoteDataUtil(facade, garmentUnitDeliveryOrderDatautil);
        }

        private GarmentDeliveryOrderDataUtil dataUtilDO(GarmentDeliveryOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }

        private GarmentExternalPurchaseOrderDataUtil dataUtilExternal(GarmentExternalPurchaseOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            return new GarmentExternalPurchaseOrderDataUtil(facade, garmentInternalPurchaseOrderDataUtil);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_PO()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetBasicPriceByPOSerialNumber(data.Items.First().POSerialNumber);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_ReadLoader_Data()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            //var filter1 = JsonConvert.SerializeObject(new {
            //    Key= "IsReceived",
            //    Condition= 2,
            //    Value= false
            //});
            //var filter2= JsonConvert.SerializeObject(new {
            //    Key= "ExpenditureType",
            //    Condition= 2,
            //    Value= "SISA"
            //});
            var filter = "[{\"Key\":\"IsReceived\",\"Condition\":2,\"Value\":false},{\"Key\":\"ExpenditureType\",\"Condition\":2,\"Value\":\"PROSES\"}]";
            //filter.Add(filter1);
            //filter.Add(filter2);

            var Response = facade.ReadLoader(1,25,"{}",null, filter, Lib.Helpers.ConditionType.ENUM_INT);
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)data.Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Get_UEN_Data_By_Id()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            var Response = facade.ReadByUENId((int)data.Id);
            Assert.NotEqual(0, Response.Id);
        }
        //
        [Fact]
        public async Task Should_Success_Get_GUEN_Data_By_Id()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var data = await dataUtil(facadeExpend, GetCurrentMethod()).GetTestData();
            var Response = facadeExpend.GetDataUEN(1);
            Assert.NotNull(Response);
        }
        //
        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }

        //      [Fact]
        //      public async Task Should_Success_Create_Data_External()
        //      {
        //          var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
        //          data.ExpenditureType = "EXTERNAL";
        //          data.ExpenditureTo = "PEMBELIAN";
        //          var Response = await facade.Create(data);

        //          Assert.NotEqual(0, Response);

        //          // var datas = await dataUtil(facade, GetCurrentMethod()).GetTestData();
        //          //var Response = facade.ReadById((int)data.Id);

        //          //var dbContext = _dbContext(GetCurrentMethod());
        //          //var newData = dbContext.GarmentUnitExpenditureNotes
        //          //    .AsNoTracking()
        //          //    .Include(x => x.Items)
        //          //    .Single(m => m.Id == data.Id);

        //          //List<GarmentUnitExpenditureNoteItem> items = new List<GarmentUnitExpenditureNoteItem>();
        //          //foreach (var item in newData.Items)
        //          //{
        //          //    var i = new GarmentUnitExpenditureNoteItem
        //          //    {
        //          //        IsSave = true,
        //          //        DODetailId = item.DODetailId,

        //          //        EPOItemId = item.EPOItemId,

        //          //        URNItemId = item.URNItemId,
        //          //        UnitDOItemId = item.Id,
        //          //        PRItemId = item.PRItemId,

        //          //        FabricType = item.FabricType,
        //          //        POItemId = item.POItemId,
        //          //        POSerialNumber = item.POSerialNumber,

        //          //        ProductId = item.ProductId,
        //          //        ProductCode = item.ProductCode,
        //          //        ProductName = item.ProductName,
        //          //        ProductRemark = item.ProductRemark,
        //          //        Quantity = 5,

        //          //        RONo = item.RONo,

        //          //        UomId = item.UomId,
        //          //        UomUnit = item.UomUnit,

        //          //        PricePerDealUnit = item.PricePerDealUnit,
        //          //        DOCurrencyRate = item.DOCurrencyRate,
        //          //        Conversion = 1,
        //          //    };
        //          //    items.Add(i);
        //          //}

        //          //var data2 = new GarmentUnitExpenditureNote
        //          //{
        //          //    UnitSenderId = newData.UnitSenderId,
        //          //    UnitSenderCode = newData.UnitSenderCode,
        //          //    UnitSenderName = newData.UnitSenderName,

        //          //    UnitRequestId = newData.UnitRequestId,
        //          //    UnitRequestCode = newData.UnitRequestCode,
        //          //    UnitRequestName = newData.UnitRequestName,

        //          //    UnitDOId = newData.UnitDOId,
        //          //    UnitDONo = newData.UnitDONo,

        //          //    StorageId = newData.StorageId,
        //          //    StorageCode = newData.StorageCode,
        //          //    StorageName = newData.StorageName,

        //          //    StorageRequestId = newData.StorageRequestId,
        //          //    StorageRequestCode = newData.StorageRequestCode,
        //          //    StorageRequestName = newData.StorageRequestName,

        //          //    ExpenditureType = "EXTERNAL",
        //          //    ExpenditureTo = "EXTERNAL",
        //          //    UENNo = "UENNO12345",

        //          //    ExpenditureDate = DateTimeOffset.Now,

        //          //    IsPreparing = false,
        //          //    Items = items

        //          //};

        //          //var Response2 = await facade.Create(data2);

        //          //Assert.NotEqual(Response2, 0);
        //      }

        [Fact]
        public async Task Should_Success_Create_Data_one_Item()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            //List<GarmentUnitExpenditureNoteItem> items = new List<GarmentUnitExpenditureNoteItem>();
            //items.Add(data.Items.First());
            //data.Items = items;
            data.Items.First().IsSave = false;
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }


        [Fact]
        public async Task Should_Success_Create_Data_Null_Summary()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithStorage();
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]

        public async Task Should_Success_Create_Data_Type_Transfer()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataTypeTransfer();
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]

        public async Task Should_Success_Create_Data_Type_Sample()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataTypeTransfer();
            data.ExpenditureType = "SAMPLE";
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }


        [Fact]
        public async Task Should_Success_Create_Data_Type_TransferSample()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataTypeTransfer();
            data.ExpenditureType = "TRANSFER SAMPLE";
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }


        [Fact]
        public async Task Should_Success_Create_Data_LAINLAIN()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithStorage();
            data.ExpenditureType = "LAIN-LAIN";
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }
        //[Fact]
        //public async Task Should_Success_Create_Data_Type_Sample_FromSample()
        //{
        //    var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataTypeTransfer();
        //    data.ExpenditureType = "SAMPLE";
        //    data.UnitSenderCode = "SMP1";
        //    var Response = await facade.Create(data);
        //    Assert.NotEqual(0, Response);
        //}

        [Fact]
        public async Task Should_Error_Create_Data_Null_Items()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            data.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Error_Create_Data_DOCurrency()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataTypeTransfer();

            foreach (var garmentUnitExpenditureNoteItem in data.Items)
            {
                var garmentUnitDeliveryOrderItem = dbContext.GarmentUnitDeliveryOrderItems.FirstOrDefault(s => s.Id == garmentUnitExpenditureNoteItem.UnitDOItemId);
                garmentUnitDeliveryOrderItem.DOCurrencyRate = 0;
            }

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var data = await dataUtil.GetTestData();

            var newData = dbContext.GarmentUnitExpenditureNotes
                .AsNoTracking()
                .Include(x => x.Items)
                .Single(m => m.Id == data.Id);

            newData.Items.First().IsSave = false;

            var ResponseUpdate = await facade.Update((int)newData.Id, newData);
            Assert.NotEqual(0, ResponseUpdate);
        }

        [Fact]
        public async Task Should_Success_Update_Data_Type_Transfer()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());

            var dataTransfer = await dataUtil.GetTestDataAcc();

            var newData = dbContext.GarmentUnitExpenditureNotes
                .AsNoTracking()
                .Include(x => x.Items)
                .Single(m => m.Id == dataTransfer.Id);

            newData.Items.First().IsSave = true;
            var ResponseUpdateTypeTransfer = await facade.Update((int)newData.Id, newData);
            Assert.NotEqual(0, ResponseUpdateTypeTransfer);
        }

        [Fact]

        public async Task Should_Success_Update_Data_Type_Sample()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());

            var dataTransfer = await dataUtil.GetTestDataSample();

            var newData = dbContext.GarmentUnitExpenditureNotes
                .AsNoTracking()
                .Include(x => x.Items)
                .Single(m => m.Id == dataTransfer.Id);

            newData.Items.First().IsSave = true;
            var ResponseUpdateTypeTransfer = await facade.Update((int)newData.Id, newData);
            Assert.NotEqual(0, ResponseUpdateTypeTransfer);
        }

        [Fact]
        public async Task Should_Success_Update_Data_Type_Transfer_null_Summary()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var dataTransfer = await dataUtil.GetTestDataWithStorageReqeust();

            var newData2 = new GarmentUnitExpenditureNote
            {
                Id = dataTransfer.Id,
                Items = new List<GarmentUnitExpenditureNoteItem>
                      {
                          new GarmentUnitExpenditureNoteItem
                          {
                              Id = dataTransfer.Items.First().Id
                          }
                      }
            };
            foreach (var item in dataTransfer.Items)
            {
                item.Quantity = 1;
            }

            var ResponseUpdate2 = await facade.Update((int)dataTransfer.Id, dataTransfer);
            Assert.NotEqual(0, ResponseUpdate2);
        }

        [Fact]
        public async Task Should_Error_Update_Data_Null_Items()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);

            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            dbContext.Entry(data).State = EntityState.Detached;
            data.Items = null;

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)data.Id, data));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Error_Update_Data_Type_DOCurrency()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var dataTransfer = await dataUtil.GetTestDataAcc();

            var newData = dbContext.GarmentUnitExpenditureNotes
                .AsNoTracking()
                .Include(x => x.Items)
                .Single(m => m.Id == dataTransfer.Id);

            foreach (var garmentUnitExpenditureNoteItem in newData.Items)
            {
                var garmentUnitDeliveryOrderItem = dbContext.GarmentUnitDeliveryOrderItems.FirstOrDefault(s => s.Id == garmentUnitExpenditureNoteItem.UnitDOItemId);
                garmentUnitDeliveryOrderItem.DOCurrencyRate = 0;
            }

            newData.Items.First().IsSave = true;

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)newData.Id, newData));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = await facade.Delete((int)data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data_Sample()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataSample();

            var Response = await facade.Delete((int)data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Delete_Data_Invalid_Id()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Delete(0));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Validate_Data()
        {
            GarmentUnitExpenditureNoteViewModel viewModel = new GarmentUnitExpenditureNoteViewModel { };
            Assert.True(viewModel.Validate(null).Count() > 0);

            GarmentUnitExpenditureNoteViewModel viewModelCheckExpenditureDate = new GarmentUnitExpenditureNoteViewModel
            {
                ExpenditureDate = DateTimeOffset.Now
            };
            Assert.True(viewModelCheckExpenditureDate.Validate(null).Count() > 0);

            GarmentUnitExpenditureNoteViewModel viewModelCheckUnitDeliveryOrder = new GarmentUnitExpenditureNoteViewModel
            {
                ExpenditureDate = DateTimeOffset.Now,
                UnitDODate = DateTimeOffset.Now.AddDays(2),
                UnitDONo = "UnitDONO123",

                IsTransfered = false,
                IsReceived = false
            };
            Assert.True(viewModelCheckUnitDeliveryOrder.Validate(null).Count() > 0);

            GarmentUnitExpenditureNoteViewModel viewModelCheckItemsCount = new GarmentUnitExpenditureNoteViewModel { UnitDOId = 1 };
            Assert.True(viewModelCheckItemsCount.Validate(null).Count() > 0);

            Mock<IGarmentUnitDeliveryOrderFacade> garmentUnitDeliveryOrderFacadeMock = new Mock<IGarmentUnitDeliveryOrderFacade>();

            Mock<IGarmentUnitExpenditureNoteFacade> garmentUnitExpenditureNoteFacadeMock = new Mock<IGarmentUnitExpenditureNoteFacade>();
            garmentUnitDeliveryOrderFacadeMock.Setup(s => s.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder
                {
                    Id = 1,

                    Items = new List<GarmentUnitDeliveryOrderItem>
                    {
                              new GarmentUnitDeliveryOrderItem
                              {
                                  Id = 1,
                                  Quantity = 4
                              },
                    }
                });

            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(IGarmentUnitDeliveryOrderFacade)))
                .Returns(garmentUnitDeliveryOrderFacadeMock.Object);
            serviceProvider.Setup(x => x.GetService(typeof(PurchasingDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var item = data.Items.First();
            var garmentUnitExpenditureNote = new GarmentUnitExpenditureNoteViewModel
            {
                UnitDOId = 1,
                Items = new List<GarmentUnitExpenditureNoteItemViewModel>
                      {
                          new GarmentUnitExpenditureNoteItemViewModel
                          {
                              Id = item.Id,
                              UnitDOItemId = 1,
                              Quantity = 10,
                              IsSave = true,
                              ReturQuantity = 1,
                          },

                          new GarmentUnitExpenditureNoteItemViewModel
                          {
                              Id = item.Id,
                              UnitDOItemId = 1,
                              Quantity = 100,
                              IsSave = true,
                              ReturQuantity = 1,

                          },

                          new GarmentUnitExpenditureNoteItemViewModel
                          {
                              Id = item.Id,
                              UnitDOItemId = 1,
                              Quantity = 0,
                              IsSave = true,
                              ReturQuantity = 1,
                          },
                      }
            };

            Mock<IGarmentUnitExpenditureNoteFacade> garmentUnitExpenditreMock = new Mock<IGarmentUnitExpenditureNoteFacade>();
            garmentUnitExpenditreMock.Setup(s => s.ReadById(1))
                .Returns(garmentUnitExpenditureNote);
            garmentUnitExpenditreMock.Setup(s => s.ReadById(It.IsAny<int>()))
                .Returns(garmentUnitExpenditureNote);

            serviceProvider.
                Setup(x => x.GetService(typeof(IGarmentUnitExpenditureNoteFacade)))
                .Returns(garmentUnitExpenditreMock.Object);
            System.ComponentModel.DataAnnotations.ValidationContext garmentUnitDeliveryOrderValidate = new System.ComponentModel.DataAnnotations.ValidationContext(garmentUnitExpenditureNote, serviceProvider.Object, null);
            Assert.True(garmentUnitExpenditureNote.Validate(garmentUnitDeliveryOrderValidate).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_For_Preparing()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataForPreparing();
            var Response = facade.ReadForGPreparing();
            Assert.NotEmpty(Response.Data);
        }


        [Fact]
        public async Task Should_Error_Update_Data_Null_Items_For_Preparing_Create()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);

            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            dbContext.Entry(data).State = EntityState.Detached;
            data.Items = null;

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.UpdateIsPreparing(0, null));
            Assert.NotNull(e.Message);
        }

		//fail pas PR
        //[Fact]
        //public async Task Should_Success_Update_Data_For_DeliveryReturn()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);
        //    var dataUtil = this.dataUtil(facade, GetCurrentMethod());
        //    var data = await dataUtil.GetTestData();

        //    var newData = dbContext.GarmentUnitExpenditureNotes
        //        .AsNoTracking()
        //        .Include(x => x.Items)
        //        .Single(m => m.Id == data.Id);

        //    newData.Items.First().IsSave = false;

        //    var ResponseUpdate = await facade.UpdateReturQuantity((int)newData.Id, 1, 0);
        //    Assert.NotEqual(0, ResponseUpdate);
        //}

        [Fact]
        public async Task Should_Error_Update_Data_Null_Items_For_DeliveryReturn()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), dbContext);


            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            dbContext.Entry(data).State = EntityState.Detached;
            data.Items = null;

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.UpdateReturQuantity(0, 0, 0));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Error_Get_Data_By_Id()
        {
            var dbString = GetCurrentMethod() + "Task Should_Error_Get_Data_By_Id";
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(dbString));
            var data = await dataUtil(facade, dbString).GetTestDataAcc();
            //var Response = facade.GetROAsalById((int)data.Id);

            Assert.Throws<System.InvalidOperationException>(() => facade.GetROAsalById((int)data.Id));
            //   Assert.NotEqual(0, Response.DetailExpenditureId);
        }


		//#region Flow_Detail_material
		//[Fact]
		//public async Task Should_Success_GetReport_Flow_Detail()
		//{
		//    var dbContext = _dbContext(GetCurrentMethod());
		//    var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
		//    var responseLocalSupplier = await Facade.Create(modelLocalSupplier);


		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GetReport("", "", "", dateFrom, dateTo, 0, "", 1, 25);



		//    Assert.NotNull(results.Item1);
		//}


		//[Fact]
		//public async Task Should_Success_GetXLS_Flow_Detail()
		//{
		//    var dbContext = _dbContext(GetCurrentMethod());
		//    var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
		//    var responseLocalSupplier = await Facade.Create(modelLocalSupplier);


		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GenerateExcel("", "", "", "", "", dateFrom, dateTo, 0);

		//    Assert.NotNull(results);
		//}

		//[Fact]
		//public async Task Should_Success_GetXLS_Flow_Detail_Expend()
		//{
		//    var dbContext = _dbContext(GetCurrentMethod());
		//    var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
		//    modelLocalSupplier.ExpenditureDate = DateTimeOffset.MinValue;
		//    var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GenerateExcel("", "", "", "", "", dateFrom, dateTo, 0);

		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GenerateExcel("", "", "", "", "", dateFrom, dateTo, 0);


		//    Assert.NotNull(results);
		//}


		//[Fact]
		//public async Task Should_Success_GetXLS_Flow_Detail_NUll_Result()
		//{
		//    var dbContext = _dbContext(GetCurrentMethod());
		//    var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
		//    var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GenerateExcel("BB", "", "", "", "", dateFrom, dateTo, 0);



		//    Assert.NotNull(results);
		//}


		//[Fact]
		//public async Task Should_Success_GetXLS_Flow_Detail_Unit_Expend()
		//{
		//    var dbContext = _dbContext(GetCurrentMethod());
		//    var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
		//    modelLocalSupplier.ExpenditureDate = DateTimeOffset.MinValue;
		//    var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GenerateExcelForUnit("", "", "", "", "", dateFrom, dateTo, 0);

		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GenerateExcelForUnit("", "", "", "", "", dateFrom, dateTo, 0);



		//    Assert.NotNull(results);
		//}

		//[Fact]
		//public async Task Should_Success_GetXLS_Flow_Detail_Unit_NUll_Result()
		//{
		//    var dbContext = _dbContext(GetCurrentMethod());
		//    var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
		//    var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

		//    var reportService = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
		//    var dateTo = DateTime.UtcNow.AddDays(1);
		//    var dateFrom = dateTo.AddDays(-30);
		//    var results = reportService.GenerateExcelForUnit("BB", "", "", "", "", dateFrom, dateTo, 0);



		//    Assert.NotNull(results);
		//}

		//#endregion
		[Fact]
		public async Task Should_Success_Get_Monitoring_Flow()
		{
			var dbContext = _dbContext(GetCurrentMethod());
			var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			var Response = facade.GetReportOut(null, null, "", 1, 25, "{}", 7);
			Assert.NotNull(Response.Item1);
		}
		[Fact]

        public async Task Should_Success_Get_Monitoring_Out()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportOut(null, null, "", 1, 25, "{}", 7);
            Assert.NotNull(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Excel_Monitoring_Out()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcelMonOut(null, null, "", 7);
            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async void Should_Success_Patch_One()
        {
            GarmentUnitExpenditureNoteFacade facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var model = await dataUtil.GetTestData();

            JsonPatchDocument<GarmentUnitExpenditureNote> jsonPatch = new JsonPatchDocument<GarmentUnitExpenditureNote>();
            jsonPatch.Replace(m => m.IsPreparing, true);

            var Response = await facade.PatchOne(model.Id, jsonPatch);
            Assert.NotEqual(0, Response);
        }

        //      [Fact]
        //      public async void Should_Error_Patch_One()
        //      {
        //          GarmentUnitExpenditureNoteFacade facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var dataUtil = this.dataUtil(facade, GetCurrentMethod());
        //          var model = await dataUtil.GetTestData();

        //          JsonPatchDocument<GarmentUnitExpenditureNote> jsonPatch = new JsonPatchDocument<GarmentUnitExpenditureNote>();
        //          jsonPatch.Replace(m => m.Id, 0);

        //          var Response = await Assert.ThrowsAnyAsync<Exception>(async () => await facade.PatchOne(model.Id, jsonPatch));
        //          Assert.NotNull(Response.Message);
        //      }

        //      [Fact]
        //      public async Task Should_Success_GetReport_CMT_Report()
        //      {
        //          var dbContext = _dbContext(GetCurrentMethod());
        //          var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewDataForPreparing();
        //          var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

        //          //long nowTicks = DateTimeOffset.Now.Ticks;
        //          //string nowTicksA = $"{nowTicks}a";
        //          //string RONo = $"RO{nowTicksA}";
        //          string RO = modelLocalSupplier.Items.FirstOrDefault().RONo;
        //          DataTable dataTable = new DataTable();
        //          dataTable.Columns.Add("Invoice", typeof(String));
        //          dataTable.Columns.Add("ExpenditureGoodId", typeof(String));
        //          dataTable.Columns.Add("RO", typeof(String));
        //          dataTable.Columns.Add("Article", typeof(String));
        //          dataTable.Columns.Add("qtyBJ", typeof(double));
        //          dataTable.Rows.Add("", "", RO, "", 0);

        //          Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //          mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //              .Returns(dataTable.CreateDataReader());
        //          mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //              .Returns(dataTable.CreateDataReader());

        //          var reportService = new GarmentReportCMTFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //          var dateTo = DateTime.UtcNow.AddDays(1);
        //          var dateFrom = dateTo.AddDays(-30);
        //          var results = reportService.GetReport(dateFrom, dateTo, 0, 1, 25,"", 0);



        //          Assert.NotNull(results.Item1);
        //      }

        //      [Fact]
        //      public async Task Should_Success_GetXls_CMT_Report()
        //      {
        //          var dbContext = _dbContext(GetCurrentMethod());
        //          var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewDataForPreparing();
        //          var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

        //          //long nowTicks = DateTimeOffset.Now.Ticks;
        //          //string nowTicksA = $"{nowTicks}a";
        //          string RO = modelLocalSupplier.Items.FirstOrDefault().RONo;

        //          DataTable dataTable = new DataTable();
        //          dataTable.Columns.Add("Invoice", typeof(String));
        //          dataTable.Columns.Add("ExpenditureGoodId", typeof(String));
        //          dataTable.Columns.Add("RO", typeof(String));
        //          dataTable.Columns.Add("Article", typeof(String));
        //          dataTable.Columns.Add("qtyBJ", typeof(double));
        //          dataTable.Rows.Add("Invoice", "Id", RO, "Article", 0);

        //          Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //          mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //              .Returns(dataTable.CreateDataReader());
        //          mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //              .Returns(dataTable.CreateDataReader());

        //          var reportService = new GarmentReportCMTFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //          var dateTo = DateTime.UtcNow.AddDays(1);
        //          var dateFrom = dateTo.AddDays(-30);
        //          var results = reportService.GenerateExcel(dateFrom, dateTo, 0, 0, null);



        //          Assert.NotNull(results);
        //      }

        //      [Fact]
        //      public async Task Should_Success_GetXls_CMT_Report_Null_Result()
        //      {
        //          var dbContext = _dbContext(GetCurrentMethod());
        //          var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
        //          var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

        //          //long nowTicks = DateTimeOffset.Now.Ticks;
        //          //string nowTicksA = $"{nowTicks}a";
        //          string RO = modelLocalSupplier.Items.FirstOrDefault().RONo;

        //          DataTable dataTable = new DataTable();
        //          dataTable.Columns.Add("Invoice", typeof(String));
        //          dataTable.Columns.Add("ExpenditureGoodId", typeof(String));
        //          dataTable.Columns.Add("RO", typeof(String));
        //          dataTable.Columns.Add("Article", typeof(String));
        //          dataTable.Columns.Add("qtyBJ", typeof(double));
        //          dataTable.Rows.Add("Invoice", "Id", RO, "Article", 0);

        //          Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //          mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //              .Returns(dataTable.CreateDataReader());
        //          mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //              .Returns(dataTable.CreateDataReader());

        //          var reportService = new GarmentReportCMTFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //          var dateTo = DateTime.UtcNow.AddDays(1);
        //          var dateFrom = dateTo.AddDays(-30);
        //          var results = reportService.GenerateExcel(dateFrom, dateTo, 0, 0, null);



        //          Assert.NotNull(results);
        //      }

        //      [Fact]
        //      public async Task Should_Success_Master_Unit()
        //      {
        //          var dbContext = _dbContext(GetCurrentMethod());
        //          var Facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var modelLocalSupplier = await dataUtil(Facade, GetCurrentMethod()).GetNewData();
        //          var responseLocalSupplier = await Facade.Create(modelLocalSupplier);

        //          DataTable dataTable = new DataTable();

        //          dataTable.Columns.Add("UnitName", typeof(String));
        //          dataTable.Columns.Add("UnitCode", typeof(double));
        //          dataTable.Rows.Add("UnitName", "1");

        //          Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //          mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //              .Returns(dataTable.CreateDataReader());
        //          mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //              .Returns(dataTable.CreateDataReader());

        //          var reportService = new GarmentReportCMTFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //          var dateTo = DateTime.UtcNow.AddDays(1);
        //          var dateFrom = dateTo.AddDays(-30);
        //          var results = reportService.Read(1, 25, "", "UnitName","");



        //          Assert.NotNull(results);
        //      }
        //      [Fact]
        //      public async Task Should_Success_RO_Feature()
        //      {
        //          var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

        //          var ro = "";

        //          foreach(var i in data.Items)
        //          {
        //              ro = i.RONo;
        //          }

        //          var RoFacade = new ROFeatureFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //          var Response = RoFacade.GetROReport(7, ro, 1, 25, "{}");

        //          Assert.NotNull(Response.Item1);
        //          //var Response = facade.Read()
        //      }


        //      //
        //      //
        [Fact]
        public async Task Should_Success_GetReport_Realization_CMT()
        {
            var externalFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilexternal = dataUtilExternal(externalFacade, GetCurrentMethod());
            GarmentExternalPurchaseOrder data = await dataUtilExternal(externalFacade, GetCurrentMethod()).GetNewDataFabric();

            foreach (var i in data.Items)
            {
                i.ProductName = "FABRIC";
                i.RONo = "RONo123";
            }

            data.PaymentMethod = "CMT";

            await externalFacade.Create(data, "test");

            var doFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var doDataUtil = new GarmentDeliveryOrderDataUtil(doFacade, datautilexternal);

            var DO = await doDataUtil.GetNewData(data);

            await doFacade.Create(DO, "test");

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, doDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, DO);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            datauen.UnitSenderId = 20;
            foreach (var uen in datauen.Items)
            {
                uen.ProductRemark = "ProductRemark";
            }
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var reportService = new GarmentRealizationCMTReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var dateTo = DateTime.UtcNow.AddDays(1);
            var dateFrom = dateTo.AddDays(-30);

            var results = reportService.GetReport(dateFrom, dateTo, datauen.UnitSenderCode, 1, 25, "", 0);

            Assert.NotNull(results);

        }

        [Fact]
        public async Task Should_Success_GetReport_Realization_CMT_500_Kasbank()
        {
            var externalFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilexternal = dataUtilExternal(externalFacade, GetCurrentMethod());
            GarmentExternalPurchaseOrder data = await dataUtilExternal(externalFacade, GetCurrentMethod()).GetNewDataFabric();

            foreach (var i in data.Items)
            {
                i.ProductName = "FABRIC";
                i.RONo = "RONo123";
            }

            data.PaymentMethod = "CMT";

            await externalFacade.Create(data, "test");

            var doFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var doDataUtil = new GarmentDeliveryOrderDataUtil(doFacade, datautilexternal);

            var DO = await doDataUtil.GetNewData(data);

            await doFacade.Create(DO, "test");

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, doDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, DO);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            datauen.UnitSenderId = 20;
            foreach (var uen in datauen.Items)
            {
                uen.ProductRemark = "ProductRemark";
            }
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
               .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentExpenditureGoodDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/monitoring/garment-cmt-sales"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentInvoiceMonitoringDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("dpp-vat-bank-expenditure-notes/invoice")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            var reportService = new GarmentRealizationCMTReportFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));

            var dateTo = DateTime.UtcNow.AddDays(1);
            var dateFrom = dateTo.AddDays(-30);

            var results = reportService.GetReport(dateFrom, dateTo, datauen.UnitSenderCode, 1, 25, "", 0);

            Assert.NotNull(results);

        }

        [Fact]
        public async Task Should_Success_GetReport_Realization_CMT_InternalServerError_Result()
        {
            var externalFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilexternal = dataUtilExternal(externalFacade, GetCurrentMethod());
            GarmentExternalPurchaseOrder data = await dataUtilExternal(externalFacade, GetCurrentMethod()).GetNewDataFabric();

            foreach (var i in data.Items)
            {
                i.ProductName = "FABRIC";
                i.RONo = "RONo123";
            }

            data.PaymentMethod = "CMT";

            await externalFacade.Create(data, "test");

            var doFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var doDataUtil = new GarmentDeliveryOrderDataUtil(doFacade, datautilexternal);

            var DO = await doDataUtil.GetNewData(data);

            await doFacade.Create(DO, "test");

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, doDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, DO);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            datauen.UnitSenderId = 20;
            foreach (var uen in datauen.Items)
            {
                uen.ProductRemark = "ProductRemark";
            }
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
               .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/monitoring/garment-cmt-sales"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            httpClientService
               .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("dpp-vat-bank-expenditure-notes/invoice")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            var reportService = new GarmentRealizationCMTReportFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));

            var dateTo = DateTime.UtcNow.AddDays(1);
            var dateFrom = dateTo.AddDays(-30);

            var results = reportService.GetReport(dateFrom, dateTo, datauen.UnitSenderCode, 1, 25, "", 0);

            Assert.NotNull(results);

        }

        [Fact]
        public async Task Should_Success_GetXls_Realization_CMT()
        {
            var externalFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilexternal = dataUtilExternal(externalFacade, GetCurrentMethod());
            GarmentExternalPurchaseOrder data = await dataUtilExternal(externalFacade, GetCurrentMethod()).GetNewDataFabric();

            foreach (var i in data.Items)
            {
                i.ProductName = "FABRIC";
                i.RONo = "RONo123";
            }

            data.PaymentMethod = "CMT";

            await externalFacade.Create(data, "test");

            var doFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var doDataUtil = new GarmentDeliveryOrderDataUtil(doFacade, datautilexternal);

            var DO = await doDataUtil.GetNewData(data);

            await doFacade.Create(DO, "test");

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, doDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, DO);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            datauen.UnitSenderId = 20;
            foreach (var uen in datauen.Items)
            {
                uen.ProductRemark = "ProductRemark";
            }
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var reportService = new GarmentRealizationCMTReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var dateTo = DateTime.UtcNow.AddDays(1);
            var dateFrom = dateTo.AddDays(-30);

            var results = reportService.GenerateExcel(dateFrom, dateTo, datauen.UnitSenderCode, 0, "");

            Assert.IsType<MemoryStream>(results);
        }

        [Fact]
        public async Task Should_Success_GetXls_Realization_CMT_Null_Result()
        {
            var externalFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilexternal = dataUtilExternal(externalFacade, GetCurrentMethod());
            GarmentExternalPurchaseOrder data = await dataUtilExternal(externalFacade, GetCurrentMethod()).GetNewDataFabric();


            data.PaymentMethod = "CMT";

            await externalFacade.Create(data, "test");

            var doFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var doDataUtil = new GarmentDeliveryOrderDataUtil(doFacade, datautilexternal);

            var DO = await doDataUtil.GetNewData(data);

            await doFacade.Create(DO, "test");

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, doDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, DO);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            datauen.UnitSenderId = 20;
            foreach (var uen in datauen.Items)
            {
                uen.ProductRemark = "ProductRemark";
            }
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var reportService = new GarmentRealizationCMTReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var dateTo = DateTime.UtcNow.AddDays(1);
            var dateFrom = dateTo.AddDays(-30);

            var results = reportService.GenerateExcel(null, null, "cvc", 0, null);

            Assert.IsType<MemoryStream>(results);
        }

        [Fact]
        public async Task Should_Success_Get_ROfeature()
        {
            var externalFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilexternal = dataUtilExternal(externalFacade, GetCurrentMethod());
            GarmentExternalPurchaseOrder data = await dataUtilExternal(externalFacade, GetCurrentMethod()).GetNewDataFabric();

            var ro = "";

            foreach (var i in data.Items)
            {
                i.ProductName = "FABRIC";
                ro = i.RONo;
            }

            data.PaymentMethod = "CMT";

            await externalFacade.Create(data, "test");

            var doFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var doDataUtil = new GarmentDeliveryOrderDataUtil(doFacade, datautilexternal);

            var DO = await doDataUtil.GetNewData(data);

            await doFacade.Create(DO, "test");

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, doDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData2(null, DO);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = ro;
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            datauen.UnitSenderId = 20;
            foreach (var uen in datauen.Items)
            {
                uen.ProductRemark = "ProductRemark";
            }
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            //var ro = "";

            //foreach(var i in dataurn.Items)
            //{
            //    ro = i.RONo;
            //}

            var RoFacade = new ROFeatureFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var Response = RoFacade.GetROReport(7, ro, 1, 25, "{}");

            Assert.NotNull(Response);
        }


        #region traceable
        [Fact]
        public async Task Should_Success_Get_Traceable_In_By_BCNo()
        {

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var facadeTraceable = new TraceableBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetReportTraceableIN(dataBC.BeacukaiNo, "BCNo", dataBC.CustomsType);

            Assert.NotNull(Response.Item1);


        }

        [Fact]
        public async Task Should_Success_Get_Traceable_Null_Response_In_By_BCNo()
        {

            var httpClientService = new Mock<IHttpClientService>();

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("customs-reports/getPEB")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new BeacukaiAddedDataUtil().GetNullFormatterOkString()) });
            httpClientService
              .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("garment-sample-finishing-outs/traceable-by-ro")), It.IsAny<HttpContent>()))
              .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentFinishingOutDataUtil().GetNullFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("finishing-outs/for-traceable")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentFinishingOutDataUtil().GetNullFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("cutting-outs/for-traceable")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCuttingOutDataUtil().GetNullFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentExpenditureGoodDataUtil().GetNullFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder());
            var mockDebtBalanceService = new Mock<IGarmentDebtBalanceService>();
            mockDebtBalanceService
                .Setup(x => x.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .ReturnsAsync(1);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDebtBalanceService)))
                .Returns(mockDebtBalanceService.Object);

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var facadeTraceable = new TraceableBeacukaiFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetReportTraceableIN(dataBC.BeacukaiNo, "BCNo", dataBC.CustomsType);

            Assert.NotNull(Response.Item1);


        }

        //[Fact]
        //public async Task Should_Success_Get_Traceable_In_By_RONo()
        //{

        //    var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
        //    GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

        //    await facade.Create(data, "Unit Test");

        //    var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
        //    var dataBC = await datautilBC.GetNewData(USERNAME, data);

        //    dataBC.CustomsType = "BC 23";
        //    dataBC.BeacukaiNo = "BeacukaiNo";

        //    await facadeBC.Create(dataBC, USERNAME);

        //    var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
        //    var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

        //    var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

        //    var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
        //    var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

        //    var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
        //    await garmentUnitReceiptNoteFacade.Create(dataurn);

        //    var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
        //    dataunitDO.RONo = "RONo123";
        //    await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

        //    var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
        //    await garmentUnitExpenditureNoteFacade.Create(datauen);

        //    var facadeTraceable = new TraceableBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    var Response = facadeTraceable.GetReportTraceableIN(dataBC.BeacukaiNo, "RONo", dataunitDO.RONo);

        //    Assert.NotNull(Response.Item1);


        //}
        [Fact]
        public async Task Should_Success_Get_Excel_Traceable_In_By_BCNo()
        {

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var facadeTraceable = new TraceableBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetTraceableInExcel(dataBC.BeacukaiNo, "BCNo", dataBC.CustomsType);

            Assert.IsType<MemoryStream>(Response);


        }

        [Fact]
        public async Task Should_Success_Get_Excel_Traceable_In_By_BCNo_Null_Result()
        {

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var facadeTraceable = new TraceableBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetTraceableInExcel("", "BCNo", dataBC.CustomsType);

            Assert.IsType<MemoryStream>(Response);


        }

        [Fact]
        public async Task Should_Success_Get_Excel_Null_BUK_Traceable_In_By_BCNo()
        {
            var httpClientService = new Mock<IHttpClientService>();

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("customs-reports/getPEB")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("finishing-outs/for-traceable")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            httpClientService
            .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("garment-sample-finishing-outs/traceable-by-ro")), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentFinishingOutDataUtil().GetNullFormatterOkString()) });
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("cutting-outs/for-traceable")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder());
            var mockDebtBalanceService = new Mock<IGarmentDebtBalanceService>();
            mockDebtBalanceService
                .Setup(x => x.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .ReturnsAsync(1);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDebtBalanceService)))
                .Returns(mockDebtBalanceService.Object);



            var facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), serviceProviderMock.Object);
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var facadeTraceable = new TraceableBeacukaiFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetTraceableInExcel(dataBC.BeacukaiNo, "BCNo", dataBC.CustomsType);

            Assert.IsType<MemoryStream>(Response);


        }

        [Fact]
        public async Task Should_Success_Get_Traceable_Out()
        {
            var facadeTraceable = new TraceableBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));





            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);


            var response = facadeTraceable.getQueryTraceableOut("BCNo123");


            Assert.NotNull(response);
            //var response2 = facadeTraceable.getQueryDetail("RONo123");


            //Assert.NotNull(response2);

        }


        [Fact]
        public async Task Should_Success_Get_Traceable_Out_INternalServerError_Response()
        {
            var httpClientService = new Mock<IHttpClientService>();


            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("customs-reports/getPEB/byBCNo"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            httpClientService
                 .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/byInvoice")), It.IsAny<HttpContent>()))
                 .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
               .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("preparings/byRONO")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));


            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder());
            var mockDebtBalanceService = new Mock<IGarmentDebtBalanceService>();
            mockDebtBalanceService
                .Setup(x => x.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .ReturnsAsync(1);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDebtBalanceService)))
                .Returns(mockDebtBalanceService.Object);


            var facadeTraceable = new TraceableBeacukaiFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));





            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);


            var response = facadeTraceable.getQueryTraceableOut("BCNo123");


            Assert.NotNull(response);
            //var response2 = facadeTraceable.getQueryDetail("RONo123");


            //Assert.NotNull(response2);

        }

        [Fact]
        public async Task Should_Success_Get_Traceable_Out_Null_Response()
        {
            var httpClientService = new Mock<IHttpClientService>();


            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("customs-reports/getPEB/byBCNo"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new BeacukaiAddedDataUtil().GetNullFormatterOkString()) });

            httpClientService
                 .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/byInvoice")), It.IsAny<HttpContent>()))
                 .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentExpenditureGoodDataUtil().GetNullFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });
            httpClientService
               .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("preparings/byRONO")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentPreparingDataUtil().GetNullFormatterOkString()) });


            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder());
            var mockDebtBalanceService = new Mock<IGarmentDebtBalanceService>();
            mockDebtBalanceService
                .Setup(x => x.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .ReturnsAsync(1);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDebtBalanceService)))
                .Returns(mockDebtBalanceService.Object);


            var facadeTraceable = new TraceableBeacukaiFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));





            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);


            var response = facadeTraceable.getQueryTraceableOut("BCNo123");


            Assert.NotNull(response);
            //var response2 = facadeTraceable.getQueryDetail("RONo123");


            //Assert.NotNull(response2);

        }

        [Fact]
        public async Task Should_Success_Get_Excel_Traceable_Out()
        {
            var facadeTraceable = new TraceableBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);


            var response2 = facadeTraceable.GetExceltraceOut("BCNo123");


            Assert.IsType<MemoryStream>(response2);

        }
        #endregion


        [Fact]
        public async Task Should_Success_Revise_Create_Date()
        {
            var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            List<long> garmentUnitReceipts = new List<long>();
            garmentUnitReceipts.Add(data.Id);

            var Response = facade.UenDateRevise(garmentUnitReceipts, "test", DateTime.Now);
            Assert.NotEqual(0, Response);
        }

        //[Fact]
        //public async Task Should_Error_Revise_Create_Date_Items()
        //{
        //    var facade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithStorage();
        //    data.Id = 0;
        //    List<long> garmentUnitReceipts = new List<long>(1);
        //    Exception e = Assert.Throws<Exception>(() => facade.UenDateRevise(garmentUnitReceipts, "test", DateTime.Now));
        //    Assert.NotNull(e.Message);
        //}

        #region historyFlowProduct
        [Fact]
        public async Task Should_Success_Get_History_Product()
        {

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            foreach (var i in data.Items)
            {
                foreach (var d in i.Details)
                {
                    d.ProductCode = "CodeTest123";
                }
            }

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var facadeTraceable = new MonitoringFlowProductFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetFlow(null, "BeacukaiNo", "CodeTest123");

            Assert.NotNull(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Excel_History_Product()
        {

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            foreach (var i in data.Items)
            {
                foreach (var d in i.Details)
                {
                    d.ProductCode = "CodeTest123";
                }
            }

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var facadeTraceable = new MonitoringFlowProductFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetProductFlowExcel(null, "BeacukaiNo", "CodeTest123");

            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Excel_History_Product_Null_Result()
        {

            var facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datauitlDO = dataUtilDO(facade, GetCurrentMethod());
            GarmentDeliveryOrder data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();

            foreach (var i in data.Items)
            {
                foreach (var d in i.Details)
                {
                    d.ProductCode = "CodeTest123";
                }
            }

            await facade.Create(data, "Unit Test");

            var facadeBC = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datauitlDO, facadeBC);
            var dataBC = await datautilBC.GetNewData(USERNAME, data);

            dataBC.CustomsType = "BC 23";
            dataBC.BeacukaiNo = "BeacukaiNo";

            await facadeBC.Create(dataBC, USERNAME);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, datauitlDO, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProviderUnitReceiptNote(), _dbContext(GetCurrentMethod()));
            var garmentUnitExpenditureNoteDatautil = new GarmentUnitExpenditureNoteDataUtil(garmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDatautil);

            var dataurn = await garmentUnitReceiptNoteDatautil.GetNewData(null, data);
            await garmentUnitReceiptNoteFacade.Create(dataurn);

            var dataunitDO = await garmentUnitDeliveryOrderDatautil.GetNewData(dataurn);
            dataunitDO.RONo = "RONo123";
            await garmentUnitDeliveryOrderFacade.Create(dataunitDO);

            var datauen = await garmentUnitExpenditureNoteDatautil.GetNewData(dataunitDO);
            await garmentUnitExpenditureNoteFacade.Create(datauen);

            var facadeTraceable = new MonitoringFlowProductFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var Response = facadeTraceable.GetProductFlowExcel(null, "BCNo", null);

            Assert.IsType<MemoryStream>(Response);
        }

		#endregion
		[Fact]
		public async Task Should_Success_Get_MonitoringFlow()
		{
			var dbContext = _dbContext(GetCurrentMethod());
			var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

			var facade = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facadeExpend, GetCurrentMethod()).GetTestDataMonitoringFlow();
			var Response = facade.GetReport("BB","", "",DateTime.Now,DateTime.Now, 7, "{}",25,1);
			Assert.NotNull(Response.Item1);
		}
		[Fact]
		public async Task Should_Success_Get_MonitoringFlowSMP1()
		{
			var dbContext = _dbContext(GetCurrentMethod());
			var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

			var facade = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facadeExpend, GetCurrentMethod()).GetTestDataMonitoringFlow();
			var Response = facade.GetReport("BB", "", "SMP1", DateTime.Now, DateTime.Now, 7, "{}", 25, 1);
			Assert.NotNull(Response.Item1);
		}
		[Fact]
		public async Task Should_Success_Get_ExcelMonitoringFlow()
		{
			var dbContext = _dbContext(GetCurrentMethod());
			var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

			var facade = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facadeExpend, GetCurrentMethod()).GetTestDataMonitoringFlow();
			var Response = facade.GenerateExcel("BB", "","FABRIC", "SMP1","SAMPLE", DateTime.Now, DateTime.Now, 7);
			Assert.NotNull(Response);
		}
		[Fact]
		public async Task Should_Success_Get_ExcelUnitMonitoringFlow()
		{
			var dbContext = _dbContext(GetCurrentMethod());
			var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

			var facade = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facadeExpend, GetCurrentMethod()).GetTestDataMonitoringFlow();
			var Response = facade.GenerateExcelForUnit("BB", "", "FABRIC", "SMP1", "SAMPLE", DateTime.Now, DateTime.Now, 7);
			Assert.NotNull(Response);
		}

        [Fact]
        public async Task ReadLoaderProductByROJob_Success()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            var facade = new GarmentFlowDetailMaterialReportFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facadeExpend, GetCurrentMethod()).GetTestData();
            var Response = facadeExpend.ReadLoaderProductByROJob(null, "{'RONo': 'ro'}");
            Assert.NotNull(Response);
        }
    }
}