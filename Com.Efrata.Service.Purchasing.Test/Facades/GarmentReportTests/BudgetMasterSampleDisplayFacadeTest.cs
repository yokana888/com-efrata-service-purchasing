using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentReportTests
{
    public class BudgetMasterSampleDisplayFacadeTest
    {
        private const string ENTITY = "BudgetMasterSampleDisplay";

        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private Mock<IServiceProvider> GetMockServiceProvider()
        {

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            var mockServiceProvider = new Mock<IServiceProvider>();
            var httpClientServiceMock = new Mock<IHttpClientService>();

            httpClientServiceMock
                .Setup(x => x.GetAsync(It.IsRegex($"^master/garment-buyer-brands/all")))
                .ReturnsAsync(message);

            httpClientServiceMock
                .Setup(x => x.GetAsync(It.IsRegex($"^master/garment-buyers/all")))
                .ReturnsAsync(message);

            mockServiceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });

            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientServiceMock.Object);

            return mockServiceProvider;
        }

        private PurchasingDbContext GetDbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging();

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private GarmentPurchaseRequestDataUtil GetDataUtil(GarmentPurchaseRequestFacade facade)
        {
            return new GarmentPurchaseRequestDataUtil(facade);
        }

        [Fact]
        public async void Should_Success_Get_Monitoring()
        {
            var mockServiceProvider = GetMockServiceProvider();

            var dbContext = GetDbContext(GetCurrentMethod());

            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, dbContext);
            var dataUtil = GetDataUtil(garmentPurchaseRequestFacade);
            var dataGarmentPurchaseRequest = await dataUtil.GetTestData();

            var facade = new BudgetMasterSampleDisplayFacade(mockServiceProvider.Object, dbContext);

            var Response = facade.GetMonitoring(dataGarmentPurchaseRequest.Id, "{}");
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async void Should_Success_Get_Excel()
        {
            var mockServiceProvider = GetMockServiceProvider();

            var dbContext = GetDbContext(GetCurrentMethod());

            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, dbContext);
            var dataUtil = GetDataUtil(garmentPurchaseRequestFacade);
            var dataGarmentPurchaseRequest = await dataUtil.GetTestData();

            var facade = new BudgetMasterSampleDisplayFacade(mockServiceProvider.Object, dbContext);

            var Response = facade.GenerateExcel(dataGarmentPurchaseRequest.Id);
            Assert.NotNull(Response);
        }
    }
}
