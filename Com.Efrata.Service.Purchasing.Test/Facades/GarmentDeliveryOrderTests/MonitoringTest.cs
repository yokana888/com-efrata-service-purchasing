using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentBeacukaiFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDailyPurchasingReportFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInvoiceFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentBeacukaiDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInvoiceDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentDeliveryOrderTests
{
    public class MonitoringTest
    {
        private const string ENTITY = "GarmentDeliveryOrderMonitoring";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        protected string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

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
            HttpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            HttpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });

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

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }

        //[Fact]
        //public async Task Should_Success_Get_Report_AccuracyArrival()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DODate = DateTimeOffset.Now.AddDays(-35);
        //    foreach (var item in data.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "LBL";
        //        }
        //    }
        //    await facade.Create(data, USERNAME);

        //    var Facade = new GarmentDeliveryOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
        //    var Response = Facade.GetReportHeaderAccuracyofArrival(null, null, null, 7);
        //    Assert.NotNull(Response.Item1);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DODate = DateTimeOffset.Now.AddDays(-35);
        //    foreach (var item in data2.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data2, USERNAME);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DODate = DateTimeOffset.Now.AddDays(-34);
        //    foreach (var item in data3.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data3, USERNAME);

        //    var data4 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data4.DODate = DateTimeOffset.Now.AddDays(-33);
        //    foreach (var item in data4.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "LBL";
        //        }
        //    }
        //    await facade.Create(data4, USERNAME);

        //    var Response1 = Facade.GetReportHeaderAccuracyofArrival(null, null, null, 7);
        //    Assert.NotNull(Response1.Item1);

        //    long nowTicks = DateTimeOffset.Now.Ticks;
        //    string nowTicksA = $"{nowTicks}a";
        //    AccuracyOfArrivalReportViewModel viewModelAccuracy = new AccuracyOfArrivalReportViewModel
        //    {
        //        supplier = new SupplierViewModel(),
        //        product = new GarmentProductViewModel(),
        //    };
        //    viewModelAccuracy.Id = 1;
        //    viewModelAccuracy.doNo = data.DONo;
        //    viewModelAccuracy.supplier.Id = data.SupplierId;
        //    viewModelAccuracy.supplier.Code = data.SupplierCode;
        //    viewModelAccuracy.supplier.Name = data.SupplierName;
        //    viewModelAccuracy.doDate = data.DODate;
        //    viewModelAccuracy.poSerialNumber = nowTicksA;
        //    viewModelAccuracy.product.Id = nowTicks;
        //    viewModelAccuracy.product.Code = nowTicksA;
        //    viewModelAccuracy.prDate = DateTimeOffset.Now;
        //    viewModelAccuracy.poDate = DateTimeOffset.Now;
        //    viewModelAccuracy.epoDate = DateTimeOffset.Now;
        //    viewModelAccuracy.article = nowTicksA;
        //    viewModelAccuracy.roNo = nowTicksA;
        //    viewModelAccuracy.shipmentDate = DateTimeOffset.Now;
        //    viewModelAccuracy.status = nowTicksA;
        //    viewModelAccuracy.staff = nowTicksA;
        //    viewModelAccuracy.category = nowTicksA;
        //    viewModelAccuracy.dateDiff = (int)nowTicks;
        //    viewModelAccuracy.ok_notOk = nowTicksA;
        //    viewModelAccuracy.percentOk_notOk = (int)nowTicks;
        //    viewModelAccuracy.jumlahOk = (int)nowTicks;
        //    viewModelAccuracy.jumlah = (int)nowTicks;
        //    viewModelAccuracy.paymentMethod = data.PaymentMethod;
        //    viewModelAccuracy.paymentType = data.PaymentType;
        //    string paymentMethod = viewModelAccuracy.paymentMethod;
        //    string paymentType = viewModelAccuracy.paymentType;

        //    var Response2 = Facade.GetReportDetailAccuracyofArrival($"BuyerCode{nowTicksA}", null, null, null, 7);
        //    Assert.NotNull(Response2.Item1);

        //    var Response3 = Facade.GetReportDetailAccuracyofArrival($"BuyerCode{nowTicksA}", "Bahan Pendukung", null, null, 7);
        //    Assert.NotNull(Response3.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_AccuracyArrival_Excel()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    await facade.Create(data, USERNAME);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DODate = DateTimeOffset.Now.AddDays(-35);
        //    foreach (var item in data2.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data2, USERNAME);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DODate = DateTimeOffset.Now.AddDays(-34);
        //    foreach (var item in data3.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data3, USERNAME);

        //    var data4 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data4.DODate = DateTimeOffset.Now.AddDays(-33);
        //    foreach (var item in data4.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "LBL";
        //        }
        //    }
        //    await facade.Create(data4, USERNAME);

        //    var Facade = new GarmentDeliveryOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
        //    var Response = Facade.GenerateExcelArrivalHeader(null, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);

        //    long nowTicks = DateTimeOffset.Now.Ticks;
        //    string nowTicksA = $"{nowTicks}a";
        //    var Response1 = Facade.GenerateExcelArrivalDetail($"BuyerCode{nowTicksA}", null, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response1);
        //}


        //[Fact]
        //public async Task ShouldSuccess_GetReportHeaderAccuracyofArrival_with_CategoryBB()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    PurchasingDbContext dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
        //    var data = await dataUtil(facade, testName).GetTestData();

        //    //Act
        //    var response = facade.GetReportHeaderAccuracyofArrival("Bahan Baku", null, null,1);

        //    //Assert
        //    Assert.NotNull(response);
        //}

        //[Fact]
        //public async Task ShouldSuccess_GetReportHeaderAccuracyofArrival_with_CategoryBP()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    PurchasingDbContext dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
        //    var data = await dataUtil(facade, testName).GetTestData();

        //    //Act
        //    var response = facade.GetReportHeaderAccuracyofArrival("Bahan Pendukung", null, null, 1);

        //    //Assert
        //    Assert.NotNull(response);
        //}


        [Fact]
        public void ShouldSuccess_GetAccuracyOfArrivalHeader()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            string testName = GetCurrentAsyncMethod();
            PurchasingDbContext dbCOntext = _dbContext(testName);

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);

            //Act
            var response = facade.GetAccuracyOfArrivalHeader(null, null, null, 0);

            //Assert
            Assert.NotNull(response);
        }
               
        [Fact]
        public async Task ShouldSuccess_GetAccuracyOfArrivalHeader_with_CategoryBB()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            string testName = GetCurrentAsyncMethod();
            PurchasingDbContext dbCOntext = _dbContext(testName);

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
            var data = await dataUtil(facade, testName).GetTestData();

            //Act
            var response = facade.GetAccuracyOfArrivalHeader("Bahan Baku", null, null, 0);

            //Assert
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ShouldSuccess_GetAccuracyOfArrivalHeader_with_CategoryBP()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            string testName = GetCurrentAsyncMethod();
            PurchasingDbContext dbCOntext = _dbContext(testName);

            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
            var data = await dataUtil(facade, testName).GetTestData();

            //Act
            var response = facade.GetAccuracyOfArrivalHeader("Bahan Pendukung", null, null, 0);

            //Assert
            Assert.NotNull(response);
        }
        //[Fact]
        //public async Task ShouldSuccess_GetAccuracyOfArrivalHeader_with_CategoryBP()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    PurchasingDbContext dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
        //    var data = await dataUtil(facade, testName).GetTestData();

        //    //Act
        //    var response = facade.GetAccuracyOfArrivalHeader("Bahan Pendukung", DateTime.Now.AddDays(-28), DateTime.Now.AddDays(28));

        //    //Assert
        //    Assert.NotNull(response);
        //    Assert.True(response.Total > 0);
        //}

        //[Fact]
        //public async Task ShouldThrowsException_GetAccuracyOfArrivalHeader_When_InvalidDateRange()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    var dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
        //    var data = await dataUtil(facade, testName).GetTestData();

        //    //Act and Assert
        //    Assert.Throws<Exception>(() => facade.GetAccuracyOfArrivalHeader("Bahan Pendukung", DateTime.Now.AddDays(2), DateTime.Now));
        //}

        //[Fact]
        //public async Task ShouldSuccess_GetAccuracyOfArrivalDetail_with_CategoryBB()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    var dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
        //    var data = await dataUtil(facade, testName).GetTestData();
        //    //Act
        //    var response = facade.GetAccuracyOfArrivalDetail(data.SupplierCode, "Bahan Baku", DateTime.Now.AddDays(28), DateTime.Now.AddDays(28));

        //    //Assert
        //    Assert.NotNull(response);
        //}

        //[Fact]
        //public async Task ShouldSuccess_GetAccuracyOfArrivalDetail_with_CategoryBP()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    var dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
        //    var data = await dataUtil(facade, testName).GetTestData();
        //    //Act
        //    var response = facade.GetAccuracyOfArrivalDetail(data.SupplierCode, "Bahan Pendukung", null, null);

        //    //Assert
        //    Assert.NotNull(response);
        //}

        //[Fact]
        //public async Task ShouldThrowsException_GetAccuracyOfArrivalDetail_When_InvalidDateRange()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    var dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);
        //    var data = await dataUtil(facade, testName).GetTestData();

        //    //Act and Assert
        //    Assert.Throws<Exception>(() => facade.GetAccuracyOfArrivalDetail(data.SupplierCode, "Bahan Pendukung", DateTime.Now.AddDays(2), DateTime.Now));
        //}


        //[Fact]
        //public void ShouldSuccess_GetAccuracyOfArrivalDetail_With_EmptyData()
        //{
        //    //Setup
        //    Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
        //    string testName = GetCurrentAsyncMethod();
        //    PurchasingDbContext dbCOntext = _dbContext(testName);

        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProviderMock.Object, dbCOntext);

        //    //Act
        //    var response = facade.GetAccuracyOfArrivalDetail(null, null, null,null);

        //    //Assert
        //    Assert.NotNull(response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_AccuracyDelivery()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DODate = DateTimeOffset.Now.AddDays(10);
        //    foreach (var item in data.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "LBL";
        //        }
        //    }
        //    await facade.Create(data, USERNAME);

        //    var Facade = new GarmentDeliveryOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
        //    var Response = Facade.GetReportHeaderAccuracyofDelivery(null, null, data.PaymentType, data.PaymentMethod, 7);
        //    Assert.NotNull(Response.Item1);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DODate = DateTimeOffset.Now.AddDays(10);
        //    foreach (var item in data2.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data2, USERNAME);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DODate = DateTimeOffset.Now.AddDays(10);
        //    foreach (var item in data3.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data3, USERNAME);

        //    var data4 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data4.DODate = DateTimeOffset.Now.AddDays(11);
        //    foreach (var item in data4.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "LBL";
        //        }
        //    }
        //    await facade.Create(data4, USERNAME);

        //    var Response1 = Facade.GetReportHeaderAccuracyofDelivery(null, null, "", "", 7);
        //    Assert.NotNull(Response1.Item1);

        //    long nowTicks = DateTimeOffset.Now.Ticks;
        //    string nowTicksA = $"{nowTicks}a";
        //    var Response2 = Facade.GetReportDetailAccuracyofDelivery($"BuyerCode{nowTicksA}", null, null, "", "", 7);
        //    Assert.NotNull(Response2.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_AccuracyDelivery_Excel()
        //{
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    await facade.Create(data, USERNAME);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DODate = DateTimeOffset.Now.AddDays(10);
        //    foreach (var item in data2.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data2, USERNAME);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DODate = DateTimeOffset.Now.AddDays(10);
        //    foreach (var item in data3.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "SUB";
        //        }
        //    }
        //    await facade.Create(data3, USERNAME);

        //    var data4 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data4.DODate = DateTimeOffset.Now.AddDays(10);
        //    foreach (var item in data4.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.ProductCode = "LBL";
        //        }
        //    }
        //    await facade.Create(data4, USERNAME);

        //    var Facade = new GarmentDeliveryOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
        //    var Response = Facade.GenerateExcelDeliveryHeader(null, null, "", "", 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);

        //    long nowTicks = DateTimeOffset.Now.Ticks;
        //    string nowTicksA = $"{nowTicks}a";
        //    var Response1 = Facade.GenerateExcelDeliveryDetail($"BuyerCode{nowTicksA}", null, null, "", "", 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response1);
        //}

        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportDO(model.DONo, "", model.SupplierId, "", "", null, null, 1, 25, "{}", 7);
            Assert.NotEqual(-1, Response.Item2);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Null_Parameter()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportDO("", "", 0, null, null, null, null, 1, 25, "{}", 7);
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcelDO(model.DONo, "", model.SupplierId, null, null, null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel_Null_parameter()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcelDO("99999", null, 0, null, null, null, null, 0);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        // Buku Harian Pembelian
        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Data()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtil(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));
        //    //var garmentDeliveryOrder = await datautilDO.GetNewData();
        //    //foreach (var i in garmentDeliveryOrder.Items)
        //    //{
        //    //    foreach(var j in i.Details)
        //    //    {
        //    //        j.CodeRequirment = "BB";
        //    //    }
        //    //}

        //    //await facadeDO.Create(garmentDeliveryOrder, "Unit Test");

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    GarmentDailyPurchasingReportFacade DataSJ = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
        //    var dataDO = await datautilDO.GetTestData();
        //    //var dataDO = await datautilDO.GetNewData();
        //    //foreach (var i in dataDO.Items)
        //    //{
        //    //    foreach (var j in i.Details)
        //    //    {
        //    //        j.CodeRequirment = "BP";
        //    //    }
        //    //}

        //    //await facadeDO.Create(garmentDeliveryOrder, "Unit Test");

        //    //var dataDO2 = await datautilDO.GetNewData();
        //    //foreach (var i in dataDO2.Items)
        //    //{
        //    //    foreach (var j in i.Details)
        //    //    {
        //    //        j.CodeRequirment = "BE";
        //    //    }
        //    //}

        //    //await facadeDO.Create(garmentDeliveryOrder, "Unit Test");


        //    var dataBC = await datautilBC.GetTestData(USERNAME, dataDO);

        //    DateTime d1 = dataBC.BeacukaiDate.DateTime;
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime;

        //    var Response = DataSJ.GetGDailyPurchasingReport(null, true, null, null, null, null,7);
        //    Assert.NotNull(Response.Item1);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Null_Parameter()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtil(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);



        //    GarmentDailyPurchasingReportFacade DataSJ = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));


        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, dataDO);


        //    DateTime d1 = dataBC.BeacukaiDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime.AddDays(30);

        //    var Response = DataSJ.GetGDailyPurchasingReport(null, true, null, null, null,null, 7);
        //    Assert.NotNull(Response.Item1);
        //    Assert.NotEqual(-1, Response.Item2);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtil(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrection = new GarmentCorrectionNotePriceFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var dataUtilCorrection = new GarmentCorrectionNoteDataUtil(garmentCorrection, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataSJ = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, dataDO);

        //    var dataCorrect = await dataUtilCorrection.GetNewData(dataDO);
        //    dataCorrect.GarmentCorrectionNote.UseVat = true;
        //    dataCorrect.GarmentCorrectionNote.UseIncomeTax = true;
        //    dataCorrect.GarmentCorrectionNote.IncomeTaxId = (long)dataCorrect.GarmentDeliveryOrder.IncomeTaxId;
        //    dataCorrect.GarmentCorrectionNote.IncomeTaxName = dataCorrect.GarmentDeliveryOrder.IncomeTaxName;
        //    dataCorrect.GarmentCorrectionNote.IncomeTaxRate = (decimal)dataCorrect.GarmentDeliveryOrder.IncomeTaxRate;
        //    await garmentCorrection.Create(dataCorrect.GarmentCorrectionNote);

        //    DateTime d1 = dataBC.BeacukaiDate.DateTime;
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime;

        //    var Response = DataSJ.GenerateExcelGDailyPurchasingReport(null, true, null, null, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel_BP_Code()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtil(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrection = new GarmentCorrectionNotePriceFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var dataUtilCorrection = new GarmentCorrectionNoteDataUtil(garmentCorrection, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataSJ = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    //var dataDO = await datautilDO.GetTestData();

        //    var dataDO = await datautilDO.GetNewData();
        //    foreach (var i in dataDO.Items)
        //    {
        //        foreach (var j in i.Details)
        //        {
        //            j.CodeRequirment = "BP";
        //        }
        //    }

        //    await facadeDO.Create(dataDO, "Unit Test");
        //    //var dataBC = await datautilBC.GetTestData(USERNAME, dataDO);
        //    var dataBC = await datautilBC.GetNewData(USERNAME, dataDO);
        //    //dataBC.ArrivalDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1);
        //    dataBC.ArrivalDate = DateTime.Now.AddDays(-1);
        //    await garmentBeaCukaiFacade.Create(dataBC, USERNAME);


        //    DateTime d1 = dataBC.BeacukaiDate.DateTime;
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime;

        //    var Response = DataSJ.GenerateExcelGDailyPurchasingReport(null, true, null, null, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel_BB_Code()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtil(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrection = new GarmentCorrectionNotePriceFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var dataUtilCorrection = new GarmentCorrectionNoteDataUtil(garmentCorrection, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataSJ = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    //var dataDO = await datautilDO.GetTestData();

        //    var dataDO = await datautilDO.GetNewData();
        //    foreach (var i in dataDO.Items)
        //    {
        //        foreach (var j in i.Details)
        //        {
        //            j.CodeRequirment = "BB";
        //        }
        //    }
        //    await facadeDO.Create(dataDO, "Unit Test");
        //    //var dataBC = await datautilBC.GetTestData(USERNAME, dataDO);

        //    var dataBC = await datautilBC.GetNewData(USERNAME, dataDO);
        //    //dataBC.ArrivalDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1);
        //    dataBC.ArrivalDate = DateTime.Now.AddDays(-1);
        //    await garmentBeaCukaiFacade.Create(dataBC, USERNAME);



        //    DateTime d1 = dataBC.BeacukaiDate.DateTime;
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime;

        //    var Response = DataSJ.GenerateExcelGDailyPurchasingReport(null, true, null, null, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel_BE_Code()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtil(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));


        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrection = new GarmentCorrectionNotePriceFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var dataUtilCorrection = new GarmentCorrectionNoteDataUtil(garmentCorrection, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataSJ = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    //var dataDO = await datautilDO.GetTestData();

        //    var dataDO = await datautilDO.GetNewData();
        //    foreach (var i in dataDO.Items)
        //    {
        //        foreach (var j in i.Details)
        //        {
        //            j.CodeRequirment = "BE";
        //        }
        //    }
        //    await facadeDO.Create(dataDO, "Unit Test");

        //    //var dataBC = await datautilBC.GetTestData(USERNAME, dataDO);
        //    var dataBC = await datautilBC.GetNewData(USERNAME, dataDO);

        //    //int day = DateTime.Now.Day 

        //    //dataBC.ArrivalDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1);
        //    dataBC.ArrivalDate = DateTime.Now.AddDays(-1);
        //    await garmentBeaCukaiFacade.Create(dataBC, USERNAME);
        //    //var dataCorrect = await dataUtilCorrection.GetNewData(dataDO);
        //    //dataCorrect.GarmentCorrectionNote.UseVat = true;
        //    //dataCorrect.GarmentCorrectionNote.UseIncomeTax = true;
        //    //dataCorrect.GarmentCorrectionNote.IncomeTaxId = (long)dataCorrect.GarmentDeliveryOrder.IncomeTaxId;
        //    //dataCorrect.GarmentCorrectionNote.IncomeTaxName = dataCorrect.GarmentDeliveryOrder.IncomeTaxName;
        //    //dataCorrect.GarmentCorrectionNote.IncomeTaxRate = (decimal)dataCorrect.GarmentDeliveryOrder.IncomeTaxRate;
        //    //await garmentCorrection.Create(dataCorrect.GarmentCorrectionNote);

        //    DateTime d1 = dataBC.BeacukaiDate.DateTime;
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime;

        //    var Response = DataSJ.GenerateExcelGDailyPurchasingReport(null, true, null, null, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel_Null_Parameter()
        //{
        //    GarmentDeliveryOrderFacade facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var datautilDO = dataUtil(facadeDO, GetCurrentMethod());
        //    var garmentDeliveryOrder = await Task.Run(() => datautilDO.GetNewData("User"));

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var garmentCorrection = new GarmentCorrectionNotePriceFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var dataUtilCorrection = new GarmentCorrectionNoteDataUtil(garmentCorrection, datautilDO);

        //    GarmentDailyPurchasingReportFacade DataSJ = new GarmentDailyPurchasingReportFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    var dataDO = await datautilDO.GetTestData();
        //    var dataBC = await datautilBC.GetTestData(USERNAME, dataDO);

        //    var dataCorrect = await dataUtilCorrection.GetNewData(dataDO);
        //    dataCorrect.GarmentCorrectionNote.UseVat = true;
        //    dataCorrect.GarmentCorrectionNote.UseIncomeTax = true;
        //    dataCorrect.GarmentCorrectionNote.IncomeTaxId = (long)dataCorrect.GarmentDeliveryOrder.IncomeTaxId;
        //    dataCorrect.GarmentCorrectionNote.IncomeTaxName = dataCorrect.GarmentDeliveryOrder.IncomeTaxName;
        //    dataCorrect.GarmentCorrectionNote.IncomeTaxRate = (decimal)dataCorrect.GarmentDeliveryOrder.IncomeTaxRate;
        //    await garmentCorrection.Create(dataCorrect.GarmentCorrectionNote);

        //    DateTime d1 = dataBC.BeacukaiDate.DateTime.AddDays(30);
        //    DateTime d2 = dataBC.BeacukaiDate.DateTime.AddDays(30);

        //    var Response = DataSJ.GenerateExcelGDailyPurchasingReport(null, true, null, null, null,null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Book_Report()
        //{
        //    var httpClientService = new Mock<IHttpClientService>();
        //    httpClientService.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
        //    httpClientService
        //        .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });

        //    var serviceProviderMock = new Mock<IServiceProvider>();
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IdentityService)))
        //        .Returns(new IdentityService { Username = "Username", TimezoneOffset = 7 });
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(httpClientService.Object);


        //    var serviceProvider = GetServiceProvider().Object;
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProvider, dbContext);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentbeacukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var dataUtilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentbeacukaiFacade);

        //    var invoicefacade = new GarmentInvoiceFacade(dbContext, serviceProvider);
        //    var garmentInvoiceDetailDataUtil = new GarmentInvoiceDetailDataUtil();
        //    var garmentInvoiceItemDataUtil = new GarmentInvoiceItemDataUtil(garmentInvoiceDetailDataUtil);
        //    var dataUtilInvoice = new GarmentInvoiceDataUtil(garmentInvoiceItemDataUtil, garmentInvoiceDetailDataUtil, datautilDO, invoicefacade);

        //    var internnotefacade = new GarmentInternNoteFacades(dbContext, serviceProvider);
        //    var datautilinternnote = new GarmentInternNoteDataUtil(dataUtilInvoice, internnotefacade);

        //    var correctionfacade = new GarmentCorrectionNotePriceFacade(serviceProviderMock.Object, dbContext);

        //    var CorrectionNote = new GarmentCorrectionNoteDataUtil(correctionfacade, datautilDO);

        //    var dataDO = await datautilDO.GetNewData();
        //    //dataDO.IsCorrection = true;
        //    await facade.Create(dataDO, USERNAME);
        //    var dataBC = await dataUtilBC.GetTestData(USERNAME, dataDO);
        //    var dataCorrection = await CorrectionNote.GetTestData(dataDO);
        //    //await correctionfacade.Create(dataCorrection);
        //    var dataInvo = await dataUtilInvoice.GetTestData2(USERNAME, dataDO);
        //    var dataintern = await datautilinternnote.GetNewData(dataInvo);
        //    await internnotefacade.Create(dataintern, false, "Unit Test");
        //    //var g =  $"{nowTicksA}"
        //    var bookReportFacade = new GarmentPurchasingBookReportFacade(serviceProvider, dbContext);
        //    var Response = bookReportFacade.GetBookReport(7, null, null, null, 0, 0, "{}", null, null);
        //    Assert.NotNull(Response.Item1);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Excel_Book_Report()
        //{
        //    var httpClientService = new Mock<IHttpClientService>();
        //    httpClientService.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetResultFormatterOkString()) });
        //    var serviceProviderMock = new Mock<IServiceProvider>();
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IdentityService)))
        //        .Returns(new IdentityService { Username = "Username", TimezoneOffset = 7 });
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(httpClientService.Object);

        //    var serviceProvider = GetServiceProvider().Object;
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProvider, dbContext);
        //    var dataUtilDO = dataUtil(facade, GetCurrentMethod());
        //    var garmentBeacukaiFacade = new GarmentBeacukaiFacade(dbContext, serviceProvider);
        //    var dataUtilBC = new GarmentBeacukaiDataUtil(dataUtilDO, garmentBeacukaiFacade);
        //    var invoicefacade = new GarmentInvoiceFacade(dbContext, serviceProvider);
        //    var garmentInvoiceDetailDataUtil = new GarmentInvoiceDetailDataUtil();
        //    var garmentinvoiceItemDataUtil = new GarmentInvoiceItemDataUtil(garmentInvoiceDetailDataUtil);
        //    var dataUtilInvo = new GarmentInvoiceDataUtil(garmentinvoiceItemDataUtil, garmentInvoiceDetailDataUtil, dataUtilDO, invoicefacade);
        //    var internnotefacade = new GarmentInternNoteFacades(dbContext, serviceProvider);
        //    var dataUtilInternNote = new GarmentInternNoteDataUtil(dataUtilInvo, internnotefacade);
        //    var correctionfacade = new GarmentCorrectionNotePriceFacade(serviceProviderMock.Object, dbContext);
        //    var correctionDataUtil = new GarmentCorrectionNoteDataUtil(correctionfacade, dataUtilDO);

        //    var dataDO = await dataUtilDO.GetNewData();
        //    await facade.Create(dataDO, USERNAME);
        //    var dataBC = await dataUtilBC.GetTestData(USERNAME, dataDO);
        //    var dataCorrection = await correctionDataUtil.GetTestData(dataDO);
        //    var dataInvo = await dataUtilInvo.GetTestData2(USERNAME, dataDO);
        //    var dataIntern = await dataUtilInternNote.GetNewData(dataInvo);
        //    await internnotefacade.Create(dataIntern, false, "Unit Test");

        //    var bookReportFacade = new GarmentPurchasingBookReportFacade(serviceProvider, dbContext);
        //    var Response = bookReportFacade.GenerateExcelBookReport(null, null, null, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Excel_Book_Report_Null_Parameters()
        //{
        //    var httpClientService = new Mock<IHttpClientService>();
        //    httpClientService.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetResultFormatterOkString()) });
        //    var serviceProviderMock = new Mock<IServiceProvider>();
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IdentityService)))
        //        .Returns(new IdentityService { Username = "Username", TimezoneOffset = 7 });
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(httpClientService.Object);

        //    var serviceProvider = GetServiceProvider().Object;
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(serviceProvider, dbContext);
        //    var dataUtilDO = dataUtil(facade, GetCurrentMethod());
        //    var garmentBeacukaiFacade = new GarmentBeacukaiFacade(dbContext, serviceProvider);
        //    var dataUtilBC = new GarmentBeacukaiDataUtil(dataUtilDO, garmentBeacukaiFacade);
        //    var invoicefacade = new GarmentInvoiceFacade(dbContext, serviceProvider);
        //    var garmentInvoiceDetailDataUtil = new GarmentInvoiceDetailDataUtil();
        //    var garmentinvoiceItemDataUtil = new GarmentInvoiceItemDataUtil(garmentInvoiceDetailDataUtil);
        //    var dataUtilInvo = new GarmentInvoiceDataUtil(garmentinvoiceItemDataUtil, garmentInvoiceDetailDataUtil, dataUtilDO, invoicefacade);
        //    var internnotefacade = new GarmentInternNoteFacades(dbContext, serviceProvider);
        //    var dataUtilInternNote = new GarmentInternNoteDataUtil(dataUtilInvo, internnotefacade);
        //    var correctionfacade = new GarmentCorrectionNotePriceFacade(serviceProviderMock.Object, dbContext);
        //    var correctionDataUtil = new GarmentCorrectionNoteDataUtil(correctionfacade, dataUtilDO);

        //    var dataDO = await dataUtilDO.GetNewData();
        //    await facade.Create(dataDO, USERNAME);
        //    var dataBC = await dataUtilBC.GetTestData(USERNAME, dataDO);
        //    var dataCorrection = await correctionDataUtil.GetTestData(dataDO);
        //    var dataInvo = await dataUtilInvo.GetTestData2(USERNAME, dataDO);
        //    var dataIntern = await dataUtilInternNote.GetNewData(dataInvo);
        //    await internnotefacade.Create(dataIntern, false, "Unit Test");

        //    var bookReportFacade = new GarmentPurchasingBookReportFacade(serviceProvider, dbContext);
        //    var date1 = dataDO.ArrivalDate.AddDays(30);
        //    var date2 = dataDO.ArrivalDate.AddDays(30);
        //    var Response = bookReportFacade.GenerateExcelBookReport(null, null, null, date1.Date, date2.Date, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Debt_Report()
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    DebtBookReportFacade reportFacade = new DebtBookReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo123";
        //    data.SupplierCode = "SupplierCode";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo1234";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GetDebtBookReport(DateTime.Now.Month, DateTime.Now.Year, null, "");
        //    Assert.NotNull(result.Item1);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Xls_Debt_Report()
        //{

        //    DataTable dataTable2 = new DataTable();
        //    dataTable2.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable2.Columns.Add("Rate", typeof(decimal));
        //    dataTable2.Columns.Add("Rate1", typeof(decimal));
        //    dataTable2.Columns.Add("Nomor", typeof(string));
        //    dataTable2.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable2.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");


        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    DebtBookReportFacade reportFacade = new DebtBookReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo12356";
        //    data.SupplierCode = "SupplierCode";
        //    data.SupplierName = "SupplierName";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.ArrivalDate = new DateTime(2018, 05, 05);
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo12345";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.SupplierName = "SupplierName";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.ArrivalDate = new DateTime(2018, 05, 20);
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    //data3.SupplierName = "SupplierName";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(2018, 04, 20));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GenerateExcelDebtReport(5, 2018, null, "");
        //    Assert.IsType<System.IO.MemoryStream>(result);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Xls_Debt_Report_With_Date()
        //{

        //    DataTable dataTable2 = new DataTable();
        //    dataTable2.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable2.Columns.Add("Rate", typeof(decimal));
        //    dataTable2.Columns.Add("Rate1", typeof(decimal));
        //    dataTable2.Columns.Add("Nomor", typeof(string));
        //    dataTable2.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable2.Rows.Add(0, 12, 12, "Nomor", DateTime.Now.Date);


        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    DebtBookReportFacade reportFacade = new DebtBookReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo12356";
        //    data.SupplierCode = "SupplierCode";
        //    data.SupplierName = "SupplierName";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.ArrivalDate = new DateTime(2018, 05, 05);
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo12345";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.SupplierName = "SupplierName";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.ArrivalDate = new DateTime(2018, 05, 20);
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    //data3.SupplierName = "SupplierName";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(2018, 04, 20));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GenerateExcelDebtReport(5, 2018, null, "");
        //    Assert.IsType<System.IO.MemoryStream>(result);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Xls_Debt_Report_Null_Parameter()
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    DebtBookReportFacade reportFacade = new DebtBookReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo123";
        //    data.SupplierCode = "SupplierCode";
        //    data.SupplierName = "SupplierName";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.ArrivalDate = new DateTime(2018, 05, 05);
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo1234";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.SupplierName = "SupplierName";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.ArrivalDate = new DateTime(2018, 05, 20);
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    //data3.SupplierName = "SupplierName";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(2018, 04, 20));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GenerateExcelDebtReport(DateTime.Now.Month + 1, DateTime.Now.Year + 1, null, "");
        //    Assert.IsType<System.IO.MemoryStream>(result);
        //}
        //[Fact]
        //public void Create_Connection_Error()
        //{
        //    var result = Assert.ThrowsAny<Exception>(() => new LocalDbCashFlowDbContext(""));
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Debt_Balance_Report()
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    GarmentDebtBalanceReportFacade reportFacade = new GarmentDebtBalanceReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo123";
        //    data.SupplierCode = "SupplierCode";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo1234";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GetDebtBookReport(DateTime.Now.Month, DateTime.Now.Year, null, "");
        //    Assert.NotNull(result.Item1);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Xls_Debt_Balance_Report()
        //{

        //    DataTable dataTable2 = new DataTable();
        //    dataTable2.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable2.Columns.Add("Rate", typeof(decimal));
        //    dataTable2.Columns.Add("Rate1", typeof(decimal));
        //    dataTable2.Columns.Add("Nomor", typeof(string));
        //    dataTable2.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable2.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");


        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    GarmentDebtBalanceReportFacade reportFacade = new GarmentDebtBalanceReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo12356";
        //    data.SupplierCode = "SupplierCode";
        //    data.SupplierName = "SupplierName";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.ArrivalDate = new DateTime(2018, 05, 05);
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo12345";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.SupplierName = "SupplierName";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.ArrivalDate = new DateTime(2018, 05, 20);
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    //data3.SupplierName = "SupplierName";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(2018, 04, 20));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GenerateExcelDebtReport(5, 2018, null, "");
        //    Assert.IsType<System.IO.MemoryStream>(result);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Xls_Debt_Balance_Report_With_Date()
        //{

        //    DataTable dataTable2 = new DataTable();
        //    dataTable2.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable2.Columns.Add("Rate", typeof(decimal));
        //    dataTable2.Columns.Add("Rate1", typeof(decimal));
        //    dataTable2.Columns.Add("Nomor", typeof(string));
        //    dataTable2.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable2.Rows.Add(0, 12, 12, "Nomor", DateTime.Now.Date);


        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable2.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    GarmentDebtBalanceReportFacade reportFacade = new GarmentDebtBalanceReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo12356";
        //    data.SupplierCode = "SupplierCode";
        //    data.SupplierName = "SupplierName";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.ArrivalDate = new DateTime(2018, 05, 05);
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo12345";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.SupplierName = "SupplierName";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.ArrivalDate = new DateTime(2018, 05, 20);
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    //data3.SupplierName = "SupplierName";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(2018, 04, 20));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GenerateExcelDebtReport(5, 2018, null, "");
        //    Assert.IsType<System.IO.MemoryStream>(result);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Xls_Debt_Balance_Report_Null_Parameter()
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());
        //    GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    GarmentDebtBalanceReportFacade reportFacade = new GarmentDebtBalanceReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var datautilDO = dataUtil(facade, GetCurrentMethod());

        //    var garmentBeaCukaiFacade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var datautilBC = new GarmentBeacukaiDataUtil(datautilDO, garmentBeaCukaiFacade);

        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONo123";
        //    data.SupplierCode = "SupplierCode";
        //    data.SupplierName = "SupplierName";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.ArrivalDate = new DateTime(2018, 05, 05);
        //    await facade.Create(data, USERNAME);

        //    var dataBC = await datautilBC.GetTestData(USERNAME, data);

        //    var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONo1234";
        //    data2.SupplierCode = "SupplierCode";
        //    data2.SupplierName = "SupplierName";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.ArrivalDate = new DateTime(2018, 05, 20);
        //    await facade.Create(data2, USERNAME);

        //    var dataBC2 = await datautilBC.GetTestData(USERNAME, data2);

        //    var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewData3();
        //    data3.DONo = "DONo1234";
        //    data3.SupplierCode = "SupplierCode";
        //    //data3.SupplierName = "SupplierName";
        //    data3.DOCurrencyCode = "CurrencyCode123";
        //    data3.ArrivalDate = new DateTimeOffset(new DateTime(2018, 04, 20));
        //    await facade.Create(data3, USERNAME);

        //    var dataBC3 = await datautilBC.GetTestData(USERNAME, data3);

        //    var result = reportFacade.GenerateExcelDebtReport(DateTime.Now.Month + 1, DateTime.Now.Year + 1, null, "");
        //    Assert.IsType<System.IO.MemoryStream>(result);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Debt_Card_Report()
        //{
        //    DataTable dataTable = new DataTable();
        //    GarmentDeliveryOrderFacade deliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    //var datautilDO = dataUtil(deliveryOrderFacade, GetCurrentMethod());
        //    var data = await dataUtil(deliveryOrderFacade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONoTest123";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.SupplierCode = "SupplierCodeTest123";
        //    data.SupplierName = "SupplierNameTest123";
        //    data.ArrivalDate = new DateTime(2020, 03, 12);
        //    await deliveryOrderFacade.Create(data, "Unit Test");
        //    var data2 = await dataUtil(deliveryOrderFacade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONoTest12";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.SupplierCode = "SupplierCodeTest123";
        //    data2.SupplierName = "SupplierNameTest123";
        //    data2.ArrivalDate = new DateTime(2020, 04, 20);
        //    await deliveryOrderFacade.Create(data2, "Unit Test");

        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());
        //    DebtCardReportFacade cardReportFacade = new DebtCardReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var Response = cardReportFacade.GetDebtCardReport(4, 2020, data.SupplierCode, data.SupplierName, data.DOCurrencyCode, null, 7);
        //    Assert.NotNull(Response.Item1);
        //}
        //[Fact]
        //public async Task Should_Success_Get_Debt_Card_Report_With_Date() {
        //    DataTable dataTable = new DataTable();
        //    GarmentDeliveryOrderFacade deliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", DateTime.Now);
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());
        //    var data = await dataUtil(deliveryOrderFacade, GetCurrentMethod()).GetNewData3();
        //    data.SupplierCode = "SupplierCode123";
        //    data.SupplierName = "SupplierName234";
        //    data.DOCurrencyCode = "Currency123";
        //    data.ArrivalDate = new DateTime(2019, 03, 03);
        //    await deliveryOrderFacade.Create(data, GetCurrentMethod());

        //    DebtCardReportFacade debtCard = new DebtCardReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var Response = debtCard.GetDebtCardReport(3, 2019, data.SupplierCode, null, data.DOCurrencyCode, null, 7);
        //    Assert.NotNull(Response.Item1);
        //}
        //[Fact]
        //public async Task Should_Success_Generate_Debt_Card_Report_Excel() {
        //    DataTable dataTable = new DataTable();
        //    GarmentDeliveryOrderFacade deliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());

        //    var data = await dataUtil(deliveryOrderFacade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONoTest123";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.SupplierCode = "SupplierCodeTest123";
        //    data.SupplierName = "SupplierNameTest123";
        //    data.ArrivalDate = new DateTime(2020, 03, 12);
        //    await deliveryOrderFacade.Create(data, "Unit Test");
        //    var data2 = await dataUtil(deliveryOrderFacade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONoTest12";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.SupplierCode = "SupplierCodeTest123";
        //    data2.SupplierName = "SupplierNameTest123";
        //    data2.ArrivalDate = new DateTime(2020, 04, 20);
        //    await deliveryOrderFacade.Create(data2, "Unit Test");
        //    DebtCardReportFacade cardReportFacade = new DebtCardReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var Response = cardReportFacade.GenerateExcelCardReport(4, 2020, data.SupplierCode, data.SupplierName, data.DOCurrencyCode, "CurrencyTest", null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);

        //}

        //[Fact]
        //public async Task Should_Success_Generate_Debt_Card_Report_Excel_Null_Parameters()
        //{
        //    DataTable dataTable = new DataTable();
        //    GarmentDeliveryOrderFacade deliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    dataTable.Columns.Add("Jumlah", typeof(decimal));
        //    dataTable.Columns.Add("Rate", typeof(decimal));
        //    dataTable.Columns.Add("Rate1", typeof(decimal));
        //    dataTable.Columns.Add("Nomor", typeof(string));
        //    dataTable.Columns.Add("Tgl", typeof(DateTime));
        //    dataTable.Rows.Add(0, 12, 12, "Nomor", "1970,1,1");
        //    Mock<ILocalDbCashFlowDbContext> mockDbContext = new Mock<ILocalDbCashFlowDbContext>();
        //    mockDbContext.Setup(s => s.ExecuteReaderOnlyQuery(It.IsAny<string>()))
        //        .Returns(dataTable.CreateDataReader());
        //    mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
        //        .Returns(dataTable.CreateDataReader());

        //    var data = await dataUtil(deliveryOrderFacade, GetCurrentMethod()).GetNewData3();
        //    data.DONo = "DONoTest123";
        //    data.DOCurrencyCode = "CurrencyCode123";
        //    data.SupplierCode = "SupplierCodeTest123";
        //    data.SupplierName = "SupplierNameTest123";
        //    data.ArrivalDate = new DateTime(2020, 03, 12);
        //    await deliveryOrderFacade.Create(data, "Unit Test");
        //    var data2 = await dataUtil(deliveryOrderFacade, GetCurrentMethod()).GetNewData3();
        //    data2.DONo = "DONoTest12";
        //    data2.DOCurrencyCode = "CurrencyCode123";
        //    data2.SupplierCode = "SupplierCodeTest123";
        //    data2.SupplierName = "SupplierNameTest123";
        //    data2.ArrivalDate = new DateTime(2020, 04, 20);
        //    await deliveryOrderFacade.Create(data2, "Unit Test");
        //    DebtCardReportFacade cardReportFacade = new DebtCardReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()), mockDbContext.Object);
        //    var Response = cardReportFacade.GenerateExcelCardReport(4, 2020, null, null, null, "", null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);

        //}

    }
}
