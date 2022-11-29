using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPOMasterDistributionFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPOMasterDistributionDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentReportTests
{
    public class MonitoringROMasterFacadeTest
    {
        private const string ENTITY = "MonitoringROMaster";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private Mock<IHttpClientService> GetMockHttpClientService()
        {
            var mockHttpClientService = new Mock<IHttpClientService>();

            mockHttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}") });
            mockHttpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            mockHttpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("cost-calculation-garments"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CostCalculationGarmentDataUtil().GetResultFormatterOkString()) });
            mockHttpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOkString()) });

            return mockHttpClientService;
        }

        private Mock<IServiceProvider> GetMockServiceProvider()
        {
            var httpClientService = GetMockHttpClientService();

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder());

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);


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

        private async Task<DataUtilResult> GetDataUtil(GarmentPOMasterDistributionFacade facade, PurchasingDbContext dbContext)
        {
            var mockServiceProvider = GetMockServiceProvider();
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, dbContext);
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);
            var garmentPurchaseRequestData = garmentPurchaseRequestDataUtil.GetNewData();

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(dbContext);
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);
            var garmentInternalPurchaseOrderData = await garmentInternalPurchaseOrderDataUtil.GetNewData(garmentPurchaseRequestData);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(mockServiceProvider.Object, dbContext);
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);
            var garmentExternalPurchaseOrderData = await garmentExternalPurchaseOrderDataUtil.GetDataForDo2(garmentInternalPurchaseOrderData);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(mockServiceProvider.Object, dbContext);
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);
            var garmentDeliveryOrderData = await garmentDeliveryOrderDataUtil.GetNewData4(garmentExternalPurchaseOrderData);

            return new DataUtilResult(new BasicDataUtil(facade, garmentDeliveryOrderDataUtil), garmentPurchaseRequestData, garmentInternalPurchaseOrderData, garmentExternalPurchaseOrderData, garmentDeliveryOrderData);
        }

        private class DataUtilResult {
            public DataUtilResult(BasicDataUtil dataUtil, GarmentPurchaseRequest purchaseRequest, List<GarmentInternalPurchaseOrder> internalPurchaseOrders, GarmentExternalPurchaseOrder externalPurchaseOrder, GarmentDeliveryOrder deliveryOrder)
            {
                this.dataUtil = dataUtil;
                this.purchaseRequest = purchaseRequest;
                this.internalPurchaseOrders = internalPurchaseOrders;
                this.externalPurchaseOrder = externalPurchaseOrder;
                this.deliveryOrder = deliveryOrder;
            }

            public BasicDataUtil dataUtil { get; }
            public GarmentPurchaseRequest purchaseRequest { get; }
            public List<GarmentInternalPurchaseOrder> internalPurchaseOrders { get; }
            public GarmentExternalPurchaseOrder externalPurchaseOrder { get; }
            public GarmentDeliveryOrder deliveryOrder { get; }
        }

        //[Fact]
        //public async void Should_Success_Get_Monitoring()
        //{
        //    var mockServiceProvider = GetMockServiceProvider();

        //    var dbContext = GetDbContext(GetCurrentMethod());

        //    var garmentPOMasterDistributionFacade = new GarmentPOMasterDistributionFacade(mockServiceProvider.Object, dbContext);
        //    var dataUtil = await GetDataUtil(garmentPOMasterDistributionFacade, dbContext);
        //    var dataGarmentPOMasterDistribution = await dataUtil.dataUtil.GetNewData(dataUtil.deliveryOrder);
        //    await dataUtil.dataUtil.GetTestData(dataGarmentPOMasterDistribution);

        //    var facade = new MonitoringROMasterFacade(mockServiceProvider.Object, dbContext);

        //    var Response = facade.GetMonitoring(dataUtil.purchaseRequest.Id);
        //    Assert.NotEmpty(Response);
        //    Assert.NotEqual(0, Response.Sum(d => d.DeliveryOrders.Sum(gdo => gdo.Distributions.Count())));
        //}

        //[Fact]
        //public async void Should_Success_Get_Excel()
        //{
        //    var mockServiceProvider = GetMockServiceProvider();

        //    var dbContext = GetDbContext(GetCurrentMethod());

        //    var garmentPOMasterDistributionFacade = new GarmentPOMasterDistributionFacade(mockServiceProvider.Object, dbContext);
        //    var dataUtil = await GetDataUtil(garmentPOMasterDistributionFacade, dbContext);
        //    var dataGarmentPOMasterDistribution = await dataUtil.dataUtil.GetNewData(dataUtil.deliveryOrder);
        //    await dataUtil.dataUtil.GetTestData(dataGarmentPOMasterDistribution);

        //    var facade = new MonitoringROMasterFacade(mockServiceProvider.Object, dbContext);

        //    var Response = facade.GetExcel(dataUtil.purchaseRequest.Id);
        //    Assert.NotNull(Response.Item2);
        //}

        [Fact]
        public async void Should_Success_Get_Excel_Empty()
        {
            var mockServiceProvider = GetMockServiceProvider();

            var dbContext = GetDbContext(GetCurrentMethod());

            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, dbContext);
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);
            var garmentPurchaseRequestData = await garmentPurchaseRequestDataUtil.GetTestData();

            var facade = new MonitoringROMasterFacade(mockServiceProvider.Object, dbContext);

            var Response = facade.GetExcel(garmentPurchaseRequestData.Id);
            Assert.NotNull(Response.Item2);
        }

        [Fact]
        public async void Should_Success_Get_Excel_No_DeliveryOrders()
        {
            var mockServiceProvider = GetMockServiceProvider();

            var dbContext = GetDbContext(GetCurrentMethod());

            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, dbContext);
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);
            var garmentPurchaseRequestData = garmentPurchaseRequestDataUtil.GetNewData();

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(dbContext);
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);
            var garmentInternalPurchaseOrderData = await garmentInternalPurchaseOrderDataUtil.GetNewData(garmentPurchaseRequestData);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(mockServiceProvider.Object, dbContext);
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);
            var garmentExternalPurchaseOrderData = await garmentExternalPurchaseOrderDataUtil.GetDataForDo2(garmentInternalPurchaseOrderData);
            await garmentExternalPurchaseOrderDataUtil.GetTestDataForDo2(garmentExternalPurchaseOrderData);

            var facade = new MonitoringROMasterFacade(mockServiceProvider.Object, dbContext);

            var Response = facade.GetExcel(garmentPurchaseRequestData.Id);
            Assert.NotNull(Response.Item2);
        }

        //[Fact]
        //public async void Should_Success_Get_Excel_No_Distributions()
        //{
        //    var mockServiceProvider = GetMockServiceProvider();

        //    var dbContext = GetDbContext(GetCurrentMethod());

        //    var garmentPOMasterDistributionFacade = new GarmentPOMasterDistributionFacade(mockServiceProvider.Object, dbContext);
        //    var dataUtil = await GetDataUtil(garmentPOMasterDistributionFacade, dbContext);
        //    var dataGarmentPOMasterDistribution = await dataUtil.dataUtil.GetNewData(dataUtil.deliveryOrder);

        //    var facade = new MonitoringROMasterFacade(mockServiceProvider.Object, dbContext);

        //    var Response = facade.GetExcel(dataUtil.purchaseRequest.Id);
        //    Assert.NotNull(Response.Item2);
        //}

        //[Fact]
        //public async void Should_Success_Get_Excel_Multiple_Distributions()
        //{
        //    var mockServiceProvider = GetMockServiceProvider();

        //    var dbContext = GetDbContext(GetCurrentMethod());

        //    var garmentPOMasterDistributionFacade = new GarmentPOMasterDistributionFacade(mockServiceProvider.Object, dbContext);
        //    var dataUtil = await GetDataUtil(garmentPOMasterDistributionFacade, dbContext);
        //    var dataGarmentPOMasterDistribution = await dataUtil.dataUtil.GetNewData(dataUtil.deliveryOrder);
        //    await dataUtil.dataUtil.GetTestData(dataGarmentPOMasterDistribution);

        //    var dataGarmentPOMasterDistributionDuplicate = dataUtil.dataUtil.CopyData(dataGarmentPOMasterDistribution);
        //    dataGarmentPOMasterDistributionDuplicate.Id = 0;
        //    dataGarmentPOMasterDistributionDuplicate.Items = new List<GarmentPOMasterDistributionItem>();
        //    foreach (var item in dataGarmentPOMasterDistribution.Items)
        //    {
        //        var itemDuplicate = dataUtil.dataUtil.CopyDataItem(item);
        //        itemDuplicate.Id = 0;
        //        itemDuplicate.Details = new List<GarmentPOMasterDistributionDetail>();
        //        foreach (var detail in item.Details)
        //        {
        //            var detailDuplicate = dataUtil.dataUtil.CopyDataDetail(detail);
        //            detailDuplicate.Id = 0;
        //            itemDuplicate.Details.Add(detailDuplicate);
        //        }
        //        dataGarmentPOMasterDistributionDuplicate.Items.Add(itemDuplicate);
        //    }
        //    await dataUtil.dataUtil.GetTestData(dataGarmentPOMasterDistributionDuplicate);

        //    var facade = new MonitoringROMasterFacade(mockServiceProvider.Object, dbContext);

        //    var Response = facade.GetExcel(dataUtil.purchaseRequest.Id);
        //    Assert.NotNull(Response.Item2);
        //}
    }
}
