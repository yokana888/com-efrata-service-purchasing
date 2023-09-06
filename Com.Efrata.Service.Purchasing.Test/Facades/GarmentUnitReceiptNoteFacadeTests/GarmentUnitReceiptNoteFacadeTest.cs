using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringUnitReceiptFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentBeacukaiFacade;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentBeacukaiDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCentralBillReceptionFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCentralBillExpenditureFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteReceptionFacades;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteExpenditureFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDailyPurchasingReportFacade;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitExpenditureDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentReceiptCorrectionDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReceiptCorrectionFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentUnitReceiptNoteFacadeTests
{
    public class GarmentUnitReceiptNoteFacadeTest
    {
        private const string ENTITY = "GarmentUnitReceiptNote";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        private IServiceProvider GetServiceProvider()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("delivery-returns"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("delivery-returns")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCategoryDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("garment/leftover-warehouse-expenditures/fabric")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentLeftoverWarehouseExpenditureFabricDataUtil().GetResultFormatterOkString()) });
            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("garment/leftover-warehouse-expenditures/accessories")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil().GetResultFormatterOkString()) });

            var mockDebtBalanceService = new Mock<IGarmentDebtBalanceService>();
            mockDebtBalanceService
                .Setup(x => x.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .ReturnsAsync(1);

            var mapper = new Mock<IMapper>();
            mapper
                .Setup(x => x.Map<GarmentUnitReceiptNoteViewModel>(It.IsAny<GarmentUnitReceiptNote>()))
                .Returns(new GarmentUnitReceiptNoteViewModel
                {
                    Id = 1,
                    DOId = 1,
                    DOCurrency = new CurrencyViewModel(),
                    Supplier = new SupplierViewModel(),
                    Unit = new UnitViewModel(),
                    Items = new List<GarmentUnitReceiptNoteItemViewModel>
                    {
                        new GarmentUnitReceiptNoteItemViewModel {
                            Product = new GarmentProductViewModel(),
                            Uom = new UomViewModel()
                        }
                    }
                });

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder());

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

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProviderMock.Object;
        }

        private IServiceProvider GetServiceProvider_DOCurrency()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":0.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("delivery-returns"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("delivery-returns")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("garment/leftover-warehouse-expenditures/fabric")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentLeftoverWarehouseExpenditureFabricDataUtil().GetResultFormatterOkString()) });
            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("garment/leftover-warehouse-expenditures/accessories")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil().GetResultFormatterOkString()) });


            var mapper = new Mock<IMapper>();
            mapper
                .Setup(x => x.Map<GarmentUnitReceiptNoteViewModel>(It.IsAny<GarmentUnitReceiptNote>()))
                .Returns(new GarmentUnitReceiptNoteViewModel
                {
                    Id = 1,
                    DOId = 1,
                    DOCurrency = new CurrencyViewModel(),
                    Supplier = new SupplierViewModel(),
                    Unit = new UnitViewModel(),
                    Items = new List<GarmentUnitReceiptNoteItemViewModel>
                    {
                        new GarmentUnitReceiptNoteItemViewModel {
                            Product = new GarmentProductViewModel(),
                            Uom = new UomViewModel()
                        }
                    }
                });

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder());

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
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

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

        private GarmentUnitReceiptNoteDataUtil dataUtil(GarmentUnitReceiptNoteFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new GarmentUnitReceiptNoteDataUtil(facade, garmentDeliveryOrderDataUtil, null);
        }

        private GarmentUnitReceiptNoteDataUtil dataUtil_DOCurrency(GarmentUnitReceiptNoteFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider_DOCurrency(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider_DOCurrency(), _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider_DOCurrency(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new GarmentUnitReceiptNoteDataUtil(facade, garmentDeliveryOrderDataUtil, null);
        }

        private GarmentDeliveryOrderDataUtil dataUtilDO(GarmentDeliveryOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }

        private GarmentBeacukaiDataUtil dataUtilBC(GarmentBeacukaiFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);
            return new GarmentBeacukaiDataUtil(garmentDeliveryOrderDataUtil, facade);
        }

        private GarmentUnitExpenditureNoteDataUtil dataUtilUEN(GarmentUnitExpenditureNoteFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(testName));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil, null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(testName), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);
        
            return new GarmentUnitExpenditureNoteDataUtil(facade, garmentUnitDeliveryOrderDatautil);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.ReadById((int)data.Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Get_Data_DOItems_By_Id()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.ReadDOItemsByURNItemId((int)data.Items.First().Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Generate_Pdf()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var dataViewModel = facade.ReadById((int)data.Id);
            var temp = dataViewModel.DOCurrency.Rate + 1;
            var Response = facade.GeneratePdf(dataViewModel);
            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Generate_Pdf_NoDOId_GDLAIN()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            
            var dataViewModel = facade.ReadById((int)data.Id);
            dataViewModel.DOId = 0;
            dataViewModel.URNType = "GUDANG LAIN";
            var temp = dataViewModel.DOCurrency.Rate + 1;
            var Response = facade.GeneratePdf(dataViewModel);
            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Generate_Pdf_NoDOId_GDSISA()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();

            var dataViewModel = facade.ReadById((int)data.Id);
            dataViewModel.DOId = 0;
            dataViewModel.URNType = "GUDANG SISA";
            var temp = dataViewModel.DOCurrency.Rate + 1;
            var Response = facade.GeneratePdf(dataViewModel);
            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Generate_Pdf_NoDOId_SISASUBCON()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();

            var dataViewModel = facade.ReadById((int)data.Id);
            dataViewModel.DOId = 0;
            dataViewModel.URNType = "SISA SUBCON";
            var temp = dataViewModel.DOCurrency.Rate + 1;
            var Response = facade.GeneratePdf(dataViewModel);
            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Generate_Pdf_PROSES()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();

            var dataViewModel = facade.ReadById((int)data.Id);
            dataViewModel.DOId = 0;
            dataViewModel.URNType = "PROSES";
            var temp = dataViewModel.DOCurrency.Rate + 1;
            var Response = facade.GeneratePdf(dataViewModel);
            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithStorage();
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);

            //var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data1 = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithStorage();
            data1.StorageId = data.StorageId;
            data1.Items.First().UomId = data.Items.First().UomId;
            data1.UnitId = data.UnitId;
            data1.DRId = "1";
            //data1.URNType = "PROSES";
            var Response1 = await facade.Create(data1);
            Assert.NotEqual(0, Response1);
        }

        [Fact]
        public async Task Should_Error_Create_Data_Null_Items()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithStorage();
            data.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Error_Create_Data_DOCurrency()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider_DOCurrency(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil_DOCurrency(facade, GetCurrentMethod()).GetNewDataWithStorage3();
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), dbContext);

            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var data = await dataUtil.GetTestDataWithStorage();
            dbContext.Entry(data).State = EntityState.Detached;
            foreach (var item in data.Items)
            {
                dbContext.Entry(item).State = EntityState.Detached;
            }

            dataUtil.SetDataWithStorage(data);
            var ResponseUpdateStorage = await facade.Update((int)data.Id, data);
            Assert.NotEqual(0, ResponseUpdateStorage);

            //// Create Storage based on UnitId that contain longTick on create DataUtil
            //dataUtil.SetDataWithStorage(data, data.UnitId);
            //var ResponseRestoreStorage = await facade.Update((int)data.Id, data);
            //Assert.NotEqual(ResponseRestoreStorage, 0);

            //data.IsStorage = false;
            //var ResponseDeleteStorage = await facade.Update((int)data.Id, data);
            //Assert.NotEqual(ResponseDeleteStorage, 0);

            //dataUtil.SetDataWithStorage(data);
            //var ResponseAddStorage = await facade.Update((int)data.Id, data);
            //Assert.NotEqual(ResponseAddStorage, 0);
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), dbContext);

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Update(0, new GarmentUnitReceiptNote()));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();

            var Response = await facade.Delete((int)data.Id, (string)data.DeletedReason);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data2()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage2();
            //data.URNType = "PROSES";

            var Response = await facade.Delete((int)data.Id, (string)data.DeletedReason);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Delete_Data_Invalid_Id()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Delete(0, ""));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public void Should_Success_Validate_Data()
        {
            GarmentUnitReceiptNoteViewModel viewModel = new GarmentUnitReceiptNoteViewModel { IsStorage = true };
            Assert.True(viewModel.Validate(null).Count() > 0);

            GarmentUnitReceiptNoteViewModel viewModelCheckDeliveryOrder = new GarmentUnitReceiptNoteViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                Unit = new UnitViewModel { Id = "1" },
                URNType = "PEMBELIAN"
            };
            Assert.True(viewModelCheckDeliveryOrder.Validate(null).Count() > 0);

            GarmentUnitReceiptNoteViewModel viewModelCheckItemsCount = new GarmentUnitReceiptNoteViewModel { DOId = 1 };
            Assert.True(viewModelCheckItemsCount.Validate(null).Count() > 0);

            GarmentUnitReceiptNoteViewModel viewModelCheckItems = new GarmentUnitReceiptNoteViewModel
            {
                DOId = 1,
                URNType = "PEMBELIAN",
                Items = new List<GarmentUnitReceiptNoteItemViewModel>
                {
                    new GarmentUnitReceiptNoteItemViewModel()
                }
            };
            Assert.True(viewModelCheckItems.Validate(null).Count() > 0);

            GarmentUnitReceiptNoteViewModel viewModelCheckItemsConvertion = new GarmentUnitReceiptNoteViewModel
            {
                DOId = 1,
                URNType = "PEMBELIAN",
                Items = new List<GarmentUnitReceiptNoteItemViewModel>
                {
                    new GarmentUnitReceiptNoteItemViewModel
                    {
                        Uom = new UomViewModel
                        {
                            Id = "1"
                        },
                        SmallUom = new UomViewModel
                        {
                            Id = "1"
                        },
                        Conversion = 10,
                        CorrectionConversion= 10,
                        DRItemId=It.IsAny<string>()
                    }
                }
            };
            Assert.True(viewModelCheckItemsConvertion.Validate(null).Count() > 0);

            GarmentUnitReceiptNoteViewModel viewModelCheckItemsExpenditure = new GarmentUnitReceiptNoteViewModel
            {
                DOId = 1,
                URNType = "GUDANG SISA",
                ExpenditureNo = "",
                Items = new List<GarmentUnitReceiptNoteItemViewModel>
                {
                    new GarmentUnitReceiptNoteItemViewModel
                    {
                        Uom = new UomViewModel
                        {
                            Id = "1"
                        },
                        SmallUom = new UomViewModel
                        {
                            Id = "1"
                        },
                        Conversion = 1,
                        CorrectionConversion= 1,
                    }
                }
            };
            Assert.True(viewModelCheckItemsExpenditure.Validate(null).Count() > 0);
        }
        //monitoring
        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();
            await facade.Create(data, USERNAME);
            var uFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datas = await dataUtil(uFacade, GetCurrentMethod()).GetNewDataWithStorage();
            var Responses = await uFacade.Create(datas);
            Assert.NotEqual(0, Responses);
            var Facade = new MonitoringUnitReceiptAllFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = Facade.GetReport(null, null, null, null, null, null, null, null, 1, 25, "{}", 7);
            Assert.NotEqual(0, Response.Item2);

            var Response1 = Facade.GetReport(null, null, null, null, null, null, null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response1.Item1);


        }

        [Fact]
        public async Task Should_Success_Get_Report_Excel()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();
            await facade.Create(data, USERNAME);
            var uFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datas = await dataUtil(uFacade, GetCurrentMethod()).GetNewDataWithStorage();
            var Responses = await uFacade.Create(datas);
            Assert.NotEqual(0, Responses);
            var Facade = new MonitoringUnitReceiptAllFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = Facade.GetReport(null, null, null, null, null, null, null, null, 1, 25, "{}", 7);
            Assert.NotEqual(0, Response.Item2);

            var Response1 = Facade.GenerateExcel(null, null, null, null, null, null, null, null, 1, 25, "{}", 7);
            Assert.IsType<System.IO.MemoryStream>(Response1);
        }

        [Fact]
        public async Task Should_Success_ReadForUnitDO()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.ReadForUnitDO();
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_ReadItemByRO()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.ReadItemByRO();
            Assert.NotEqual(Response.Count, 0);
        }

        [Fact]
        public async Task Should_Success_ReadForUnitDO_With_Filter()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var filter = new
            {
                data.UnitId,
                data.StorageId,
            };
            var Response = facade.ReadForUnitDO("", JsonConvert.SerializeObject(filter));
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_ReadForUnitDOHeader()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.ReadForUnitDOHeader();
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_ReadForUnitDOHeader_With_Filter()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var filter = new
            {
                data.UnitId,
                data.StorageId,
                RONo = "xxx"
            };
            var Response = facade.ReadForUnitDOHeader("", JsonConvert.SerializeObject(filter));
            Assert.NotEmpty(Response);
        }

        //[Fact]
        //public async Task Should_Success_ReadItem_With_Filter()
        //{
        //    var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
        //    var filter = new
        //    {
        //        data.UnitId,
        //        data.StorageId,
        //        data.DONo
        //    };
        //    var Response = facade.ReadURNItem();
        //    Assert.NotEmpty(Response);
        //}

        [Fact]
        public async Task Should_Success_ReadURNItem()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var filter = new
            {
                DONo = data.DONo,
                UnitCode = data.UnitCode,
                StorageCode = data.StorageCode

            };
            var Response = facade.ReadURNItem("", JsonConvert.SerializeObject(filter));
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_ReadItemByRO_With_Filter()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var filter = new
            {
                UnitId = data.UnitId,
                StorageId = data.StorageId,
                RONo = data.Items.First().RONo
            };
            var Response = facade.ReadItemByRO("", JsonConvert.SerializeObject(filter));
            Assert.NotEqual(Response.Count, 0);
        }
        //
        [Fact]
        public async Task Should_Success_ReadDatayDO()
        {
            var facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            GarmentDeliveryOrder dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            await facadeDO.Create(dataDO, USERNAME);

            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData(dataDO, null);

            var facadebc = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            GarmentBeacukai databc = await dataUtilBC(facadebc, GetCurrentMethod()).GetTestData(USERNAME, dataDO);

            var Response = facade.ReadDataByDO();
            Assert.NotEqual(Response.Count, 0);
        }

        [Fact]
        public async Task Should_Success_ReadDataByDO_With_Filter()
        {
            var facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            GarmentDeliveryOrder dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            await facadeDO.Create(dataDO, USERNAME);

            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData(dataDO, null);

            var filter = new
            {
                UnitId = data.UnitId,
            };

            var facadebc = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            GarmentBeacukai databc = await dataUtilBC(facadebc, GetCurrentMethod()).GetTestData(USERNAME, dataDO);

            var Response = facade.ReadDataByDO("", JsonConvert.SerializeObject(filter));
            Assert.NotEqual(Response.Count, 0);
        }
        //

        //[Fact]
        //public async Task Should_Success_ReadURNItem()
        //{
        //    var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
        //    var filter = new
        //    {
        //        DONo= data.DONo,
        //        UnitCode = data.UnitCode,
        //        StorageCode= data.StorageCode

        //    };
        //    var Response = facade.ReadURNItem("", JsonConvert.SerializeObject(filter));
        //    Assert.NotEmpty(Response);
        //}
        //Monitoring Terima BP
        [Fact]
        public async Task Should_Success_Get_Terima_BP_Report_Data()
        {
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

            var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

            GarmentUnitExpenditureNoteFacade facadeuen = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautiluen = dataUtilUEN(facadeuen, GetCurrentMethod());

            var garmentUEN = await Task.Run(() => datautiluen.GetNewData());

            var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO, datautiluen);

            var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

            MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()),GetServiceProvider());

            var dataDO = await datautilDO.GetTestData();
            var dataBon = await datautilBon.GetTestData();
            var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

            var Response = TerimaBP.GetMonitoringTerimaBonPusatReport(null, null, null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response.Item1);
        }

        //[Fact]
        //public async Task Should_Success_Get_Terima_BP_Report_Data_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

        //    DateTime d1 = dataBC.BeacukaiDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime.AddDays(30);

        //    var Response = TerimaBP.GetMonitoringTerimaBonPusatReport(d1, d2, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_BP_Report_Data_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

        //    var Response = TerimaBP.GetMonitoringTerimaBonPusatByUserReport(null, null, null, 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_BP_Report_Data_By_User_Null_Pameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);
        //    DateTime d1 = dataBC.BeacukaiDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime.AddDays(30);

        //    var Response = TerimaBP.GetMonitoringTerimaBonPusatByUserReport(d1, d2, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_BP_Report_Excel()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

        //    var Response = TerimaBP.GenerateExcelMonitoringTerimaBonPusat(null, null, null, 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_BP_Report_Excel_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);
        //    DateTime d1 = dataBC.BeacukaiDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime.AddDays(30);

        //    var Response = TerimaBP.GenerateExcelMonitoringTerimaBonPusat(d1, d2, "BCDL", 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_BP_Report_Excel_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

        //    var Response = TerimaBP.GenerateExcelMonitoringTerimaBonPusatByUser(null, null, null, 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_BP_Report_Excel_By_User_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillReceptionFacade TerimaBP = new MonitoringCentralBillReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);
        //    DateTime d1 = dataBC.BeacukaiDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime.AddDays(30);

        //    var Response = TerimaBP.GenerateExcelMonitoringTerimaBonPusatByUser(d1, d2, "BCDL", 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        ////Monitoring Keluar BP
        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Data()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

        //    var Response = KeluarBP.GetMonitoringKeluarBonPusatReport(dataBon.ReceiptDate.DateTime, dataBon.ReceiptDate.DateTime, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Data_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

        //    DateTime d1 = dataBon.ReceiptDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBon.ReceiptDate.DateTime.AddDays(30);

        //    var Response = KeluarBP.GetMonitoringKeluarBonPusatReport(d1, d2, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Data_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);

        //    var Response = KeluarBP.GetMonitoringKeluarBonPusatByUserReport(dataBon.ReceiptDate.DateTime, dataBon.ReceiptDate.DateTime, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Data_By_User_Null_Pameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);
        //    DateTime d1 = dataBon.ReceiptDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBon.ReceiptDate.DateTime.AddDays(30);

        //    var Response = KeluarBP.GetMonitoringKeluarBonPusatByUserReport(d1, d2, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Excel()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataBC = await datautilBC.GetTestDataWithURN(USERNAME);

        //    var Response = KeluarBP.GenerateExcelMonitoringKeluarBonPusat(null, null, "BCDL", 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Excel_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);
        //    DateTime d1 = dataBon.ReceiptDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBon.ReceiptDate.DateTime.AddDays(30);

        //    var Response = KeluarBP.GenerateExcelMonitoringKeluarBonPusat(d1, d2, "BCDL", 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Excel_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataBC = await datautilBC.GetTestDataWithURN(USERNAME);

        //    var Response = KeluarBP.GenerateExcelMonitoringKeluarBonPusatByUser(null, null, null, 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_BP_Report_Excel_By_User_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBon = await datautilBon.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, garmentDeliveryOrder);
        //    DateTime d1 = dataBon.ReceiptDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBon.ReceiptDate.DateTime.AddDays(30);

        //    var Response = KeluarBP.GenerateExcelMonitoringKeluarBonPusatByUser(d1, d2,"BCDL", 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        ////Monitoring Terima Nota Koreksi
        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Data()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = TerimaNK.GetMonitoringTerimaNKReport(dataNK.CorrectionDate.DateTime, dataNK.CorrectionDate.DateTime, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Data_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = TerimaNK.GetMonitoringTerimaNKReport(d1, d2, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Data_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = TerimaNK.GetMonitoringTerimaNKByUserReport(dataNK.CorrectionDate.DateTime, dataNK.CorrectionDate.DateTime, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Data_By_User_Null_Pameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = TerimaNK.GetMonitoringTerimaNKByUserReport(d1, d2, "BCDL", 1, 25, "{}", 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Excel()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = TerimaNK.GenerateExcelMonitoringTerimaNK(null, null, "BCDL", 1, 25, "{}", 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Excel_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    MonitoringCentralBillExpenditureFacade KeluarBP = new MonitoringCentralBillExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = TerimaNK.GenerateExcelMonitoringTerimaNK(d1, d2, "BCDL", 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Excel_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = TerimaNK.GenerateExcelMonitoringTerimaNKByUser(null, null, null, 1, 25, "{}", 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Terima_NK_Report_Excel_By_User_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteReceptionFacade TerimaNK = new MonitoringCorrectionNoteReceptionFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = TerimaNK.GenerateExcelMonitoringTerimaNKByUser(d1, d2, "BCDL", 1, 25, "{}", 7);

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        ////Monitoring Keluar Nota Koreksi
        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Data()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = KeluarNK.GetMonitoringKeluarNKReport(dataNK.CorrectionDate.DateTime, dataNK.CorrectionDate.DateTime, 1, 25, "{}", 7, "BCDL");
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Data_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = KeluarNK.GetMonitoringKeluarNKReport(d1, d2, 1, 25, "{}", 7, "BCDL");
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Data_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = KeluarNK.GetMonitoringKeluarNKByUserReport(dataNK.CorrectionDate.DateTime, dataNK.CorrectionDate.DateTime, 1, 25, "{}", 7, "BCDL");
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Data_By_User_Null_Pameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = KeluarNK.GetMonitoringKeluarNKByUserReport(d1, d2, 1, 25, "{}", 7, "BCDL");
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Excel()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, datautilBon, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = KeluarNK.GenerateExcelMonitoringKeluarNK(null, null, 1, 25, "{}", 7, null);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Excel_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = KeluarNK.GenerateExcelMonitoringKeluarNK(d1, d2, 1, 25, "{}", 7, "BCDL");

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Excel_By_User()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    var Response = KeluarNK.GenerateExcelMonitoringKeluarNKByUser(null, null, 1, 25, "{}", 7, "BCDL");
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Keluar_NK_Report_Excel_By_User_Null_Parameter()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));

        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentunitreceiptnoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilBon = new GarmentUnitReceiptNoteDataUtil(garmentunitreceiptnoteFacade, datautilDO);

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    MonitoringCorrectionNoteExpenditureFacade KeluarNK = new MonitoringCorrectionNoteExpenditureFacade(_dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();

        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = KeluarNK.GenerateExcelMonitoringKeluarNKByUser(d1, d2, 1, 25, "{}", 7, "BCDL");

        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //Buku Harian Pembelian
        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Data()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataNK = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();
        //    DateTime d1 = dataNK.CorrectionDate.DateTime;
        //    DateTime d2 = dataNK.CorrectionDate.DateTime;

        //    var Response = DataNK.GetGDailyPurchasingReport(null, true, null, null, null, null, 7);
        //    Assert.NotNull(Response.Item1);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Null_Parameter()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataNK = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();
        //    DateTime d1 = dataNK.CorrectionDate.DateTime;
        //    DateTime d2 = dataNK.CorrectionDate.DateTime;

        //    var Response = DataNK.GetGDailyPurchasingReport(null, true, null, null, null, null, 7);
        //    Assert.NotNull(Response.Item1);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataNK = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();
        //    DateTime d1 = dataNK.CorrectionDate.DateTime;
        //    DateTime d2 = dataNK.CorrectionDate.DateTime;

        //    var Response = DataNK.GenerateExcelGDailyPurchasingReport(null, true, null, null, null,null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel_Null_Parameter()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtilDO(facadeDO, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrectionNoteFacade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var datautilCN = new GarmentCorrectionNoteDataUtil(garmentCorrectionNoteFacade, datautilBC, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataNK = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    var dataNK = await datautilCN.GetTestDataNotaKoreksi();
        //    DateTime d1 = dataNK.CorrectionDate.DateTime.AddDays(30);
        //    DateTime d2 = dataNK.CorrectionDate.DateTime.AddDays(30);

        //    var Response = DataNK.GenerateExcelGDailyPurchasingReport(null, true, null, null, null,null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        #region flow detail penerimaan 

        //[Fact]
        //public async void Should_Success_Get_FlowReport_Data()
        //{
        //    GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
        //    var Response = facade.GetReportFlow(DateTime.MinValue, DateTime.MaxValue, model.UnitCode, "", 1, 25, "{}", 7);
        //    Assert.NotEmpty(Response.Item1);
        //}

        [Fact]
        public async void Should_Success_Get_FlowReport_Data_Null_Parameter()
        {
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportFlow(null, null, "C2C", "", 1, 25, "{}", 7);
            Assert.Empty(Response.Item1);
        }

        [Fact]
        public async void Should_Success_Get_Generate_Excel_Flow_For_Unit()
        {
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcelFlowForUnit(DateTime.MinValue, DateTime.MaxValue, model.UnitCode, "", "", 7, model.UnitName);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async void Should_Success_Get_Generate_Excel_Flow_For_Unit_Data()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);

            foreach (var i in dataUrn1.Items)
            {
                //i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }

            await facade.Create(dataUrn1);

            var stockreport = new GarmentUnitReceiptNoteFacade(serviceProvider, dbContext);
            var Response = stockreport.GenerateExcelFlowForUnit(new DateTime(1970, 1, 1), DateTime.Now, dataUrn1.UnitCode, "BB", dataUrn1.Category, 7, dataUrn1.UnitName);
            //var Response2 = stockreport.GenerateExcelStockReport("BP", null, dataUrn2.UnitName, dataUrn2.UnitCode, new DateTime(1970, 1, 1), DateTime.Now, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async void Should_Success_Get_FlowReport_Data_Excel()
        {
            //GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            //var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            //var Response = facade.GenerateExcelLow(DateTime.MinValue, DateTime.MaxValue, model.UnitCode, "", "", 7, model.UnitName);
            //Assert.IsType<System.IO.MemoryStream>(Response);
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            foreach (var i in dataUrn1.Items)
            {
                //i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }

            await facade.Create(dataUrn1);

            var stockreport = new GarmentUnitReceiptNoteFacade(serviceProvider, dbContext);
            var Response = stockreport.GenerateExcelLow(new DateTime(1970, 1, 1), DateTime.Now, dataUrn1.UnitCode, "BB", dataUrn1.Category, 7, dataUrn1.UnitName);
            //var Response2 = stockreport.GenerateExcelStockReport("BP", null, dataUrn2.UnitName, dataUrn2.UnitCode, new DateTime(1970, 1, 1), DateTime.Now, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_Excel_Null_parameter()
        {
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcelLow(null, null, "0", "", "", 7, "");
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        #endregion
        #region StockReport
        [Fact]
        public async void Should_Success_Get_Stock_Report()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);


            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
            }
            //var dataUrn3 = await dataUtilUrn.GetNewData2(nowTicks + 1);
            //dataUrn3.UENNo = "BUK" + dataUrn3.UnitCode;
            //dataUrn3.IsStorage = true;
            //dataUrn3.StorageId = nowTicks;
            //dataUrn3.StorageCode = string.Concat("StorageCode", nowTicks);
            //dataUrn3.StorageName = string.Concat("StorageName", nowTicks);
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, null, 1, 25, "{}", new DateTime(2019, 12, 26), new DateTime(2019, 12, 27));
            //var Response = stockreport.GetStockReport("BB", null, null, null, 7);
            var Response = stockreport.GetStockReport(7, null, "BB", 1, 25, "{}", null, null);
            var Response3 = stockreport.GetStockReport(7, null, "BB", 1, 25, "{}", new DateTime(2019, 12, 25), DateTime.Now);
            var Response4 = stockreport.GetStockReport(7, null, null, 1, 25, "{}", new DateTime(2019, 12, 25), DateTime.Now);
            var Response2 = stockreport.GetStockReport(7, null, "BP", 1, 25, "{}", null, null);
            Assert.NotNull(Response.Item1);
            Assert.NotNull(Response2.Item1);
        }
        [Fact]
        public async void Should_Success_Get_AccountingStock_Report_Null_Response()
        {
            var serviceProvider = GetServiceProvider();


            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCategoryDataUtil().GetResultFormatterOkNullString()) });

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterNullOkString()) });

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



            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;

            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProviderMock.Object, dbContext);
            var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, "BB", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //var Response2 = stockreport.GetStockReport(7, dataUrn2.UnitCode, "BP", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //Assert.NotNull(Response.Item1);
            Assert.NotNull(Response.Item1);

        }

        [Fact]
        public async void Should_Success_Get_AccountingStock_Report_InternalServerError_Response()
        {
            var serviceProvider = GetServiceProvider();


            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
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



            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;

            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProviderMock.Object, dbContext);
            var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, "BB", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //var Response2 = stockreport.GetStockReport(7, dataUrn2.UnitCode, "BP", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //Assert.NotNull(Response.Item1);
            Assert.NotNull(Response.Item1);

        }

        [Fact]
        public async void Should_Success_Get_Stock_Accounting_Report_Response()
        {
            var serviceProvider = GetServiceProvider();


            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCategoryDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOk2String()) });

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



            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;

            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProviderMock.Object, dbContext);
            var Response = stockreport.GetStockReport(7, null, "BB", 1, 25, "{}", null, null);
            //var Response2 = stockreport.GetStockReport(7, dataUrn2.UnitCode, "BP", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //Assert.NotNull(Response.Item1);
            Assert.NotNull(Response.Item1);
        }

        [Fact]
        public async void Should_Success_Get_Excel_Stock_Report()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);


            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
            }
            //var dataUrn3 = await dataUtilUrn.GetNewData2(nowTicks + 1);
            //dataUrn3.UENNo = "BUK" + dataUrn3.UnitCode;
            //dataUrn3.IsStorage = true;
            //dataUrn3.StorageId = nowTicks;
            //dataUrn3.StorageCode = string.Concat("StorageCode", nowTicks);
            //dataUrn3.StorageName = string.Concat("StorageName", nowTicks);
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, null, 1, 25, "{}", new DateTime(2019, 12, 26), new DateTime(2019, 12, 27));
            //var Response = stockreport.GetStockReport("BB", null, null, null, 7);
            var Response = stockreport.GenerateExcelAStockReport("BB", null, null, null, null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        [Fact]
        public async void Should_Success_Get_Excel_Stock_Report_C2A()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);


            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.UnitCode = "EFR";
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.UnitCode = "EFR";
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            dataUrn2.CreatedUtc = new DateTime(2019, 12, 25);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GenerateExcelAStockReport(null, dataUrn1.UnitCode, new DateTime(2019, 12, 26), new DateTime(2019, 12, 27), 7);
            var Response = stockreport.GenerateExcelAStockReport("BB", "BAHAN BAKU", "EFR", "KONFEKSI 2A", new DateTime(2010, 12, 26), DateTime.Now, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        [Fact]
        public async void Should_Success_Get_Excel_Stock_Report_C2B()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);


            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.UnitCode = "C2B";
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.UnitCode = "C2B";
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            dataUrn2.CreatedUtc = new DateTime(2019, 12, 25);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GenerateExcelAStockReport(null, dataUrn1.UnitCode, new DateTime(2019, 12, 26), new DateTime(2019, 12, 27), 7);
            var Response = stockreport.GenerateExcelAStockReport("BB", "BAHAN BAKU", "C2B", "KONFEKSI 2B", null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        [Fact]
        public async void Should_Success_Get_Excel_Stock_Report_C2C()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);


            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.UnitCode = "C2C";
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.UnitCode = "C2C";
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            dataUrn2.CreatedUtc = new DateTime(2019, 12, 25);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GenerateExcelAStockReport(null, dataUrn1.UnitCode, new DateTime(2019, 12, 26), new DateTime(2019, 12, 27), 7);
            var Response = stockreport.GenerateExcelAStockReport("BB", "BAHAN BAKU", "C2C", "KONFEKSI 2C", new DateTime(2010, 12, 26), DateTime.Now, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        [Fact]
        public async void Should_Success_Get_Excel_Stock_Report_C1B()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);


            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.UnitCode = "C1B";
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.UnitCode = "C1B";
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            dataUrn2.CreatedUtc = new DateTime(2019, 12, 25);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GenerateExcelAStockReport(null, dataUrn1.UnitCode, new DateTime(2019, 12, 26), new DateTime(2019, 12, 27), 7);
            var Response = stockreport.GenerateExcelAStockReport("BB", "BAHAN BAKU", "C1B", "KONFEKSI 1B", new DateTime(2010, 12, 26), DateTime.Now, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async void Should_Success_Get_Excel_Stock_Report_SMP1()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);


            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.UnitCode = "SMP1";
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.UnitCode = "SMP1";
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            dataUrn2.CreatedUtc = new DateTime(2019, 12, 25);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GenerateExcelAStockReport(null, dataUrn1.UnitCode, new DateTime(2019, 12, 26), new DateTime(2019, 12, 27), 7);
            var Response = stockreport.GenerateExcelAStockReport("BB", "BAHAN BAKU", "SMP1", "SAMPLE", new DateTime(2010, 12, 26), DateTime.Now, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        /*[Fact]
        public async void Should_Success_Get_Excel_Stock_Report_C1A()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade facadeUDO = new Lib.Facades.GarmentUnitDeliveryOrderFacades.GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);
            foreach (var a in dataDO.Items)
            {
                foreach (var b in a.Details)
                {
                    b.CodeRequirment = "BB";
                }
            }
            await facadeDO.Create(dataDO, USERNAME);
            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData3(null, dataDO);
            dataUrn1.UnitCode = "C1A";
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.UnitCode = "C1A";
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);
            dataUrn2.CreatedUtc = new DateTime(2019, 12, 25);
            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123";
                i.ProductName = "Name123";
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "Code123BP";
                i.ProductName = "Name123BP";
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new AccountingStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GenerateExcelAStockReport(null, dataUrn1.UnitCode, new DateTime(2019, 12, 26), new DateTime(2019, 12, 27), 7);
            var Response = stockreport.GenerateExcelAStockReport("BB", "BAHAN BAKU", "C1A", "KONFEKSI 1A", new DateTime(2010, 12, 26), DateTime.Now, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }*/
        #endregion
        [Fact]
        public async Task Should_Success_Get_Monitoring_IN()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.GetReportIN(null, null, "", 1, 25, "{}", 7);
            Assert.NotNull(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Excel_Monitoring_IN()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            var Response = facade.GenerateExcelMonIN(null, null, "", 7);
            Assert.IsType<MemoryStream>(Response);
        }

        #region GarmentStockReport
        [Fact]
        public async void Should_Success_Get_Stock2_Report()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;

            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new GarmentStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, "BB", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //var Response2 = stockreport.GetStockReport(7, dataUrn2.UnitCode, "BP", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //var Response3 = stockreport.GetStockReport(7, null, null, 1, 25, "{}", null, null);
            //Assert.NotNull(Response.Item1);
            //Assert.NotNull(Response.Item1);

        }

        [Fact]
        public async void Should_Success_Get_Stock21_Report_Response()
        {
            var serviceProvider = GetServiceProvider();


            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCategoryDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOk2String()) });

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



            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;

            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new GarmentStockReportFacade(serviceProviderMock.Object, dbContext);
            //var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, "BB", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //var Response2 = stockreport.GetStockReport(7, dataUrn2.UnitCode, "BP", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //Assert.NotNull(Response.Item1);
            //Assert.NotNull(Response.Item1);

        }

        [Fact]
        public async void Should_Success_Get_Excel_Stock2_Report()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            //var dataUrn3 = await dataUtilUrn.GetNewData2(nowTicks + 1);
            //dataUrn3.UENNo = "BUK" + dataUrn3.UnitCode;
            //dataUrn3.IsStorage = true;
            //dataUrn3.StorageId = nowTicks;
            //dataUrn3.StorageCode = string.Concat("StorageCode", nowTicks);
            //dataUrn3.StorageName = string.Concat("StorageName", nowTicks);
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            //var dataUDO = await dataUtilUDO.GetNewDataMultipleItem(dataUrn1, dataUrn2);
            //await facadeUDO.Create(dataUDO);
            //var dataUEN = await dataUtilUEN.GetNewDataTypeTransfer(dataUDO);
            //await facadeUEN.Create(dataUEN);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new GarmentStockReportFacade(serviceProvider, dbContext);
            //var Response = stockreport.GenerateExcelStockReport("BB", null, dataUrn1.UnitName, dataUrn1.UnitCode, new DateTime(1970, 1, 1), DateTime.Now, 7);
            //var Response2 = stockreport.GenerateExcelStockReport("BP", null, dataUrn2.UnitName, dataUrn2.UnitCode, new DateTime(1970, 1, 1), DateTime.Now, 7);
            //Assert.IsType<System.IO.MemoryStream>(Response2);
        }

        [Fact]
        public async void Should_Success_Get_Stock2_Report_Null_Response()
        {
            var serviceProvider = GetServiceProvider();


            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentCategoryDataUtil().GetResultFormatterOkNullString()) });

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterNullOkString()) });

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



            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;

            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new GarmentStockReportFacade(serviceProviderMock.Object, dbContext);
            //var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, "BB", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //var Response2 = stockreport.GetStockReport(7, dataUrn2.UnitCode, "BP", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //Assert.NotNull(Response.Item1);
            //Assert.NotNull(Response.Item1);

        }

        [Fact]
        public async void Should_Success_Get_Stock2_Report_InternalServerError_Response()
        {
            var serviceProvider = GetServiceProvider();


            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-categories"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
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



            var dbContext = _dbContext(GetCurrentMethod());
            GarmentUnitReceiptNoteFacade facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var dataUtilUrn = dataUtil(facade, GetCurrentMethod());
            GarmentUnitDeliveryOrderFacade facadeUDO = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
            var dataUtilUDO = new GarmentUnitDeliveryOrderDataUtil(facadeUDO, dataUtilUrn);
            GarmentUnitExpenditureNoteFacade facadeUEN = new GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(facadeUEN, dataUtilUDO);
            GarmentReceiptCorrectionFacade facadeRC = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilRC = new GarmentReceiptCorrectionDataUtil(facadeRC, dataUtilUrn);

            DateTimeOffset now = DateTimeOffset.Now;
            long nowTicks = now.Ticks;
            var dataUrn1 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn1.IsStorage = true;
            dataUrn1.StorageId = nowTicks;
            dataUrn1.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn1.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn1.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn1.ReceiptDate = new DateTime(2019, 12, 25);
            dataUrn1.CreatedUtc = new DateTime(2019, 12, 25);
            var dataUrn2 = await dataUtilUrn.GetNewData2(nowTicks);
            dataUrn2.IsStorage = true;
            dataUrn2.StorageId = nowTicks;
            dataUrn2.StorageCode = string.Concat("StorageCode", nowTicks);
            dataUrn2.StorageName = string.Concat("StorageName", nowTicks);
            dataUrn2.UENNo = "BUK" + dataUrn1.UnitCode;
            dataUrn2.UnitCode = dataUrn1.UnitCode;
            dataUrn2.ReceiptDate = new DateTime(2019, 12, 26);

            foreach (var i in dataUrn1.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTest123";
                i.ProductName = "Name123";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;

            }
            foreach (var i in dataUrn2.Items)
            {
                i.UENItemId = 1;
                i.ProductCode = "CodeTestBP123";
                i.ProductName = "Name123BP";
                i.ReceiptQuantity = 5;
                i.Conversion = 1;
            }
            await facade.Create(dataUrn1);
            await facade.Create(dataUrn2);
            var dataRC = await dataUtilRC.GetNewData(dataUrn1);
            await facadeRC.Create(dataRC.GarmentReceiptCorrection, USERNAME);
            var stockreport = new GarmentStockReportFacade(serviceProviderMock.Object, dbContext);
            //var Response = stockreport.GetStockReport(7, dataUrn1.UnitCode, "BB", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //var Response2 = stockreport.GetStockReport(7, dataUrn2.UnitCode, "BP", 1, 25, "{}", new DateTime(2010, 12, 26), DateTime.Now);
            //Assert.NotNull(Response.Item1);
            //Assert.NotNull(Response.Item1);

        }

        #endregion

        #region Mutation
        [Fact]
        public async Task Should_Success_Get_Mutation_BBCentral()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var facadeMutation = new MutationBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            foreach (var b in data.Items)
            {
                b.ProductName = "Name123";
            }

            await facade.Create(data);
            var Response = facadeMutation.GetReportBBCentral(1, 25, "{}", null, null, 7);
            Assert.NotNull(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Excel_Mutation_BBCentral()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var facadeMutation = new MutationBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            foreach (var b in data.Items)
            {
                b.ProductName = "Name123";
            }

            await facade.Create(data);
            var Response = facadeMutation.GenerateExcelBBCentral(null, null, 7);
            Assert.IsType<MemoryStream>(Response);
        }
        [Fact]
        public async Task Should_Success_Get_Mutation_BPCentral()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var facadeMutation = new MutationBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            foreach (var b in data.Items)
            {
                b.ProductName = "Name123BP";
            }

            await facade.Create(data);
            var Response = facadeMutation.GetReportBPCentral(1, 25, "{}", null, null, 7);
            Assert.NotNull(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Excel_Mutation_BPCentral()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var facadeMutation = new MutationBeacukaiFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            foreach (var b in data.Items)
            {
                b.ProductName = "Name123";
            }

            await facade.Create(data);
            var Response = facadeMutation.GenerateExcelBPCentral(null, null, 7);
            Assert.IsType<MemoryStream>(Response);
        }
        #endregion

        [Fact]
        public async Task Should_Success_Revise_Create_Date()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
            List<long> garmentUnitReceipts = new List<long>();
            garmentUnitReceipts.Add(data.Id);
            var Response = facade.UrnDateRevise(garmentUnitReceipts, "test", DateTime.Now);
            Assert.NotEqual(0, Response);
        }

        //[Fact]
        //public async Task Should_Error_Revise_Create_Date_Items()
        //{
        //    var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorage();
        //    List<long> garmentUnitReceipts = new List<long>();
        //    garmentUnitReceipts.Add(data.Id);
        //    data.URNNo = null;
        //    data.CreatedUtc = DateTime.MinValue;
        //    Exception e = Assert.Throws<Exception>(() => facade.UrnDateRevise(garmentUnitReceipts, "test", DateTime.Now));
        //    Assert.NotNull(e.Message);
        //}

        

    }


}