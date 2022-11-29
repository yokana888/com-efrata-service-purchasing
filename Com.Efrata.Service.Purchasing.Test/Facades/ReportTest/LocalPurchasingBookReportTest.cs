using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.Report;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.ReportTest
{
    //[Collection("ServiceProviderFixture Collection")]
    public class LocalPurchasingBookReportTest
    {
        private const string ENTITY = "LocalPurchasingReport";

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

        private UnitPaymentOrderDataUtil _dataUtil(UnitPaymentOrderFacade facade, PurchasingDbContext dbContext, string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var distributionCache = new Mock<IDistributedCache>();
            //var emptyArray = new Mock<string>();

            //distributionCache
            //    .Setup(x => x.GetString(It.IsAny<string>()))
            //    .Returns(string.Empty);
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testname)));

            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(distributionCache.Object);

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
            mockCurrencyProvider
               .Setup(x => x.GetCurrencyByCurrencyCodeDate(It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
               .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(serviceProvider.Object, dbContext);
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(serviceProvider.Object, dbContext);
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(serviceProvider.Object, dbContext);
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, serviceProvider.Object);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(serviceProvider.Object, dbContext);
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            return new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, facade);
        }

        private Mock<IServiceProvider> _getServiceProvider(string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();

            var mockIDistributeCache = new Mock<IDistributedCache>();

            //var mockIdentityService = new Mock<IdentityService>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { TimezoneOffset = 7, Token="test",Username="UnitTest"});

            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCodeDateList(It.IsAny<IEnumerable<Tuple<string, DateTimeOffset>>>()))
                .ReturnsAsync(new List<Currency> { new Currency { UId = "1", Code = "Currency", Date=DateTime.Now,Rate=1} });

            mockCurrencyProvider
                .Setup(x => x.GetUnitsIdsByAccountingUnitId(It.IsAny<int>()))
                .ReturnsAsync(new List<string>());

            mockCurrencyProvider
                .Setup(x => x.GetCategoryIdsByAccountingCategoryId(It.IsAny<int>()))
                .ReturnsAsync(new List<string>());

            mockCurrencyProvider
                .Setup(x => x.GetUnitsByUnitIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Unit> { new Unit { Id = 1,AccountingUnitId=1} });

            mockCurrencyProvider
                .Setup(x => x.GetAccountingUnitsByUnitIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<AccountingUnit> { new AccountingUnit { Id = 1, Name ="AccountingUnit",Code ="AccoutingUnit"} });

            mockCurrencyProvider
                .Setup(x => x.GetCategoriesByCategoryIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category> { new Category { Id = 1,AccountingCategoryId=1 } });

            mockCurrencyProvider
                .Setup(x => x.GetAccountingCategoriesByCategoryIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<AccountingCategory> { new AccountingCategory { Id=1,Code="AccoutingCategory",AccountingLayoutIndex=1,Name="AccoutingCategory"} });

            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);
            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(mockIDistributeCache.Object);
            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testname)));

            return serviceProvider;
        }

        [Fact]
        public async Task Should_Success_GetReport_FalseIsValas()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

            var result = await facade.GetReportDataV2(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), false, It.IsAny<int>());
            Assert.NotNull(result);

            //result = await facade.GetReport(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            //Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_GetReport_TrueIsValas()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestImportDataValas();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

            var result = await facade.GetReportDataV2(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            Assert.NotNull(result);

            //result = await facade.GetReport(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            //Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_GetReport_XLS()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

            //var result = await facade.GetReportDataV2(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), false, It.IsAny<int>());
            //Assert.NotNull(result);
            var resultExcel = await facade.GenerateExcel(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), false, It.IsAny<int>());
            Assert.NotNull(resultExcel);

            //result = await facade.GetReport(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            //Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_Generate_Pdf()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

            var result = await facade.GetReportDataV2(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), false, It.IsAny<int>());
            result.Reports[0].DataSourceSort = 1;

            var localPdf = LocalPurchasingBookReportPdfTemplate.Generate(result, 1, null, null);
            Assert.NotNull(localPdf);

            var localCurrencyPdf = LocalPurchasingForeignCurrencyBookReportPdfTemplate.Generate(result, 1, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            Assert.NotNull(localCurrencyPdf);

            var importPdf = ImportPurchasingBookReportPdfTemplate.Generate(result, 1, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            Assert.NotNull(importPdf);
        }

        [Fact]
        public async Task Should_Success_Generate_Pdf_LocalValasCorrection()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

            var result = await facade.GetReportDataV2(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), false, It.IsAny<int>());
            result.Reports[0].DataSourceSort = 2;

            var localPdf = LocalPurchasingBookReportPdfTemplate.Generate(result, 1, null, null);
            Assert.NotNull(localPdf);

            var localCurrencyPdf = LocalPurchasingForeignCurrencyBookReportPdfTemplate.Generate(result, 1, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            Assert.NotNull(localCurrencyPdf);

            var importPdf = ImportPurchasingBookReportPdfTemplate.Generate(result, 1, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            Assert.NotNull(importPdf);
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

            var result = await facade.GetReportV2(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            Assert.NotNull(result);

            result = await facade.GetReportV2(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            Assert.NotNull(result);
        }


        [Fact]
        public async Task Should_Success_GetReport_PurchaseRequest()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new PurchaseRequestFacade(serviceProvider, dbContext);

            var result =  facade.GetReport(pr.No, pr.UnitId, pr.CategoryId, pr.BudgetId, "", "", "ProductId", null, null, 1, 25, "{}", 7, "Unit Test");
            Assert.NotNull(result);

            //result = await facade.GetReport(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            //Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_GetXls_PurchaseRequest()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new PurchaseRequestFacade(serviceProvider, dbContext);

            var resultExcel = facade.GenerateExcel(pr.No, pr.UnitId, pr.CategoryId, pr.BudgetId, "", "", "", null, null,7, "Unit Test");
            Assert.NotNull(resultExcel);

            //result = await facade.GetReport(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
            //Assert.NotNull(result);
        }

        //[Fact]
        //public async Task Should_Success_Get_Data_Empty()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
        //    var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

        //    var urnId = dataUtil.Items.FirstOrDefault().URNId;
        //    var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
        //    var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
        //    var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

        //    var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

        //    var result = await facade.GetReport("Invalid URNNo", Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), false, It.IsAny<int>());
        //    Assert.Empty(result.Reports);
        //}

        //[Fact]
        //public async Task Should_Success_GenerateExcel_Data()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
        //    var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

        //    var urnId = dataUtil.Items.FirstOrDefault().URNId;
        //    var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
        //    var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
        //    var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

        //    var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

        //    var result = await facade.GenerateExcel(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), false, It.IsAny<int>());
        //    Assert.NotNull(result);

        //    result = await facade.GenerateExcel(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public async Task Should_Success_Generate_Pdf()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
        //    var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestLocalData();

        //    var urnId = dataUtil.Items.FirstOrDefault().URNId;
        //    var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
        //    var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
        //    var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

        //    var facade = new LocalPurchasingBookReportFacade(serviceProvider, dbContext);

        //    var result = await facade.GetReport(urn.URNNo, Convert.ToInt32(urn.UnitId), Convert.ToInt32(pr.CategoryId), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), true, It.IsAny<int>());

        //    var localPdf = LocalPurchasingBookReportPdfTemplate.Generate(result, 1, null, null);
        //    Assert.NotNull(localPdf);

        //    var localCurrencyPdf = LocalPurchasingForeignCurrencyBookReportPdfTemplate.Generate(result, 1, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
        //    Assert.NotNull(localCurrencyPdf);

        //    var importPdf = ImportPurchasingBookReportPdfTemplate.Generate(result, 1, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
        //    Assert.NotNull(importPdf);
        //}
    }
}
