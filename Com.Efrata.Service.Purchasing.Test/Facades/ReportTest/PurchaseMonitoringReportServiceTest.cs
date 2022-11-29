using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.ReportTest
{
    public class PurchaseMonitoringReportServiceTest
    {
        private const string ENTITY = "PurchaseMonitoringReports";
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }


        private PurchasingDbContext GetDbContext(string dbIdentity)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(dbIdentity)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private Mock<IServiceProvider> GetServiceProvider(string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, GetDbContext(testname)));

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(new MemoryCacheManager(memoryCache));

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            return serviceProvider;
        }

        private UnitPaymentPriceCorrectionNoteDataUtils CorrectionDataUtil(UnitPaymentPriceCorrectionNoteFacade facade, string testName, IServiceProvider serviceProvider)
        {


            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(serviceProvider, GetDbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(serviceProvider, GetDbContext(testName));
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(serviceProvider, GetDbContext(testName));
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(GetDbContext(testName), serviceProvider);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(serviceProvider, GetDbContext(testName));
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            UnitPaymentOrderFacade unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, GetDbContext(testName));
            UnitPaymentOrderDataUtil unitPaymentOrderDataUtil = new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, unitPaymentOrderFacade);

            return new UnitPaymentPriceCorrectionNoteDataUtils(unitPaymentOrderDataUtil, facade);
        }

        //[Fact]
        //public async Task ShouldSuccessGetReport()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentCorrectionFacade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, dbContext);

        //    var correctionDataUtil = CorrectionDataUtil(unitPaymentCorrectionFacade, GetCurrentMethod(), serviceProvider);

        //    var correctionTestData = await correctionDataUtil.GetTestData();

        //    var service = new PurchaseMonitoringService(dbContext);
        //    var result = await service.GetReport(null, null, null, null, 0, null, null, DateTimeOffset.MinValue, DateTimeOffset.Now, null,null, 0, null, 1, 25);

        //    Assert.NotEqual(0, result.Total);
        //    //Assert.NotNull(result.Data);
        //}

        //[Fact]
        //public async Task ShouldSuccessGetReport_CorrectionHargaSatuan()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentCorrectionFacade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, dbContext);

        //    var correctionDataUtil = CorrectionDataUtil(unitPaymentCorrectionFacade, GetCurrentMethod(), serviceProvider);

        //    var correctionTestData = await correctionDataUtil.GetNewData();

        //    correctionTestData.CorrectionType = "Harga Satuan";

        //    await unitPaymentCorrectionFacade.Create(correctionTestData, true, "Unit Test", 7);

        //    var service = new PurchaseMonitoringService(dbContext);
        //    var result = await service.GetReport(null, null, null, null, 0, null, null, DateTimeOffset.MinValue, DateTimeOffset.Now, null, null, 0, null, 1, 25);

        //    Assert.NotEqual(0, result.Total);
        //    //Assert.NotNull(result.Data);
        //}

        //[Fact]
        //public async Task ShouldSuccessGetReport_Quantity()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentCorrectionFacade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, dbContext);

        //    var correctionDataUtil = CorrectionDataUtil(unitPaymentCorrectionFacade, GetCurrentMethod(), serviceProvider);

        //    var correctionTestData = await correctionDataUtil.GetNewData();

        //    correctionTestData.CorrectionType = "Jumlah";

        //    await unitPaymentCorrectionFacade.Create(correctionTestData, true, "Unit Test", 7);

        //    var service = new PurchaseMonitoringService(dbContext);
        //    var result = await service.GetReport(null, null, null, null, 0, null, null, DateTimeOffset.MinValue, DateTimeOffset.Now, null, null, 0, null, 1, 25);

        //    Assert.NotEqual(0, result.Total);
        //    //Assert.NotNull(result.Data);
        //}
        //[Fact]
        //public async Task ShouldSuccessGetReport_noCorrection()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentCorrectionFacade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, dbContext);

        //    var correctionDataUtil = CorrectionDataUtil(unitPaymentCorrectionFacade, GetCurrentMethod(), serviceProvider);

        //    var correctionTestData = await correctionDataUtil.GetNewData();

        //    correctionTestData.CorrectionType = "";

        //    await unitPaymentCorrectionFacade.Create(correctionTestData, true, "Unit Test", 7);

        //    var service = new PurchaseMonitoringService(dbContext);
        //    var result = await service.GetReport(null, null, null, null, 0, null, null, DateTimeOffset.MinValue, DateTimeOffset.Now, null, null, 0, null, 1, 25);

        //    Assert.NotEqual(0, result.Total);
        //    //Assert.NotNull(result.Data);
        //}

        //[Fact]
        //public async Task ShouldSuccessGetReportXls()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentCorrectionFacade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, dbContext);

        //    var correctionDataUtil = CorrectionDataUtil(unitPaymentCorrectionFacade, GetCurrentMethod(), serviceProvider);

        //    var correctionTestData = await correctionDataUtil.GetTestData();

        //    var service = new PurchaseMonitoringService(dbContext);
        //    var result = await service.GenerateExcel(null, null, null, null, 0, null, null, DateTimeOffset.MinValue, DateTimeOffset.Now, null, null, 0, null, 1);

        //    Assert.NotNull(result);
        //    //Assert.IsType<System.IO.MemoryStream>(result);
        //}

        //[Fact]
        //public async Task ShouldSuccessGetReportXlsNullReport()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentCorrectionFacade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, dbContext);

        //    var correctionDataUtil = CorrectionDataUtil(unitPaymentCorrectionFacade, GetCurrentMethod(), serviceProvider);

        //    var correctionTestData = await correctionDataUtil.GetTestData();

        //    var service = new PurchaseMonitoringService(dbContext);
        //    var result = await service.GenerateExcel(null, null, null, null, 0, null, null, DateTimeOffset.MinValue, DateTimeOffset.Now, null, null, 0, "200", 1);

        //    Assert.NotNull(result);
        //    //Assert.IsType<System.IO.MemoryStream>(result);
        //}

        //[Fact]
        //public async Task ShouldSuccessGetReportWithFilter()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentCorrectionFacade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, dbContext);

        //    var correctionDataUtil = CorrectionDataUtil(unitPaymentCorrectionFacade, GetCurrentMethod(), serviceProvider);

        //    var correctionTestData = await correctionDataUtil.GetTestData();

        //    var purchaseRequest = await dbContext.PurchaseRequests.FirstOrDefaultAsync();
        //    var internalPurchaseOrder = await dbContext.InternalPurchaseOrders.FirstOrDefaultAsync(f => f.PRId == purchaseRequest.Id.ToString());

        //    var service = new PurchaseMonitoringService(dbContext);
        //    var result = await service.GetReport(purchaseRequest.UnitId, purchaseRequest.CategoryId, purchaseRequest.DivisionId, purchaseRequest.BudgetId, purchaseRequest.Id, internalPurchaseOrder.CreatedBy, null, DateTimeOffset.MinValue, DateTimeOffset.Now, null, null, 0, null, 1, 25);

        //    Assert.NotEqual(0, result.Total);
        //    //Assert.NotNull(result.Data);
        //}
    }
}
